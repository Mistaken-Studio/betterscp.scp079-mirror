// -----------------------------------------------------------------------
// <copyright file="StopWarheadCommand.cs" company="Mistaken">
// Copyright (c) Mistaken. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using CommandSystem;
using Exiled.API.Features;
using Mistaken.API;
using Mistaken.API.Commands;
using Mistaken.API.Extensions;
using Mistaken.RoundLogger;

namespace Mistaken.BetterSCP.SCP079.Commands
{
    /// <inheritdoc/>
    [CommandSystem.CommandHandler(typeof(CommandSystem.ClientCommandHandler))]
    public class StopWarheadCommand : IBetterCommand
    {
        /// <inheritdoc/>
        public override string Command => "stop";

        /// <inheritdoc/>
        public override string[] Aliases => new string[] { "warheadstop", "stopwarhead", "stopwh", "swarhead" };

        /// <inheritdoc/>
        public override string Description => "Stop Warhead";

        /// <inheritdoc/>
        public override string[] Execute(ICommandSender sender, string[] args, out bool success)
        {
            var player = sender.GetPlayer();
            success = false;
            if (player.Role != RoleType.Scp079)
                return new string[] { "Only SCP 079" };
            if (!Warhead.IsInProgress)
                return new string[] { "Warhead is not detonating" };
            if (Warhead.IsLocked || BetterWarheadHandler.Warhead.StopLock)
                return new string[] { "Warhead is locked" };
            if (player.Level >= ReqLvl - 1)
            {
                if (player.Energy >= Cost)
                {
                    if (IsReady)
                    {
                        Events.EventHandler.OnUseWarheadStop(new Events.SCP079UseStopEventArgs(player));
                        Warhead.Stop();
                        Warhead.LeverStatus = false;
                        Respawning.RespawnEffectsController.PlayCassieAnnouncement("PITCH_0.8 You jam_070_3 will jam_050_5 .g5 no jam_040_9 detonate me", false, false, true);
                        SCP079Handler.GainXP(player, Cost);
                        lastUse = DateTime.Now;

                        RLogger.Log("SCP079 EVENT", "STOPWARHEAD", $"{player.PlayerToString()} requested stopwarhead");

                        success = true;
                        return new string[] { PluginHandler.Instance.Translation.Success };
                    }
                    else
                    {
                        return new string[] { PluginHandler.Instance.Translation.FailedCooldown.Replace("${time}", Cooldown.ToString()) };
                    }
                }
                else
                {
                    return new string[] { PluginHandler.Instance.Translation.FailedAP.Replace("${ap}", Cost.ToString()) };
                }
            }
            else
            {
                return new string[] { PluginHandler.Instance.Translation.FailedLvl.Replace("${lvl}", ReqLvl.ToString()) };
            }
        }

        internal static float Cooldown => PluginHandler.Instance.Config.CooldownStopWarhead;

        internal static float Cost => PluginHandler.Instance.Config.ApCostStopWarhead;

        internal static float ReqLvl => PluginHandler.Instance.Config.RequiedLvlStopWarhead;

        internal static bool IsReady => lastUse.AddSeconds(Cooldown).Ticks <= DateTime.Now.Ticks;

        internal static long TimeLeft => lastUse.AddSeconds(Cooldown).Ticks - DateTime.Now.Ticks;

        private static DateTime lastUse = default(DateTime);

        private string GetUsage()
        {
            return ".stop";
        }
    }
}
