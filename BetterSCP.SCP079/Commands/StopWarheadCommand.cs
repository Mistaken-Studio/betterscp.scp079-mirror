// -----------------------------------------------------------------------
// <copyright file="StopWarheadCommand.cs" company="Mistaken">
// Copyright (c) Mistaken. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using CommandSystem;
using Exiled.API.Features;
using Exiled.API.Features.Roles;
using Mistaken.API.Commands;
using Mistaken.API.Extensions;
using Mistaken.RoundLogger;

namespace Mistaken.BetterSCP.SCP079.Commands
{
    [CommandHandler(typeof(ClientCommandHandler))]
    internal sealed class StopWarheadCommand : IBetterCommand
    {
        public override string Command => "stop";

        public override string[] Aliases => new string[] { "warheadstop", "stopwarhead", "stopwh", "swarhead" };

        public override string Description => "Stop Warhead";

        public override string[] Execute(ICommandSender sender, string[] args, out bool success)
        {
            var player = sender.GetPlayer();
            var scp = (Scp079Role)player.Role;
            success = false;

            if (player.Role.Type != RoleType.Scp079)
                return new string[] { "Only SCP 079" };

            if (!Warhead.IsInProgress)
                return new string[] { "Warhead is not detonating" };

            if (Warhead.IsLocked || API.Handlers.BetterWarheadHandler.Warhead.StopLock)
                return new string[] { "Warhead is locked" };

            if (scp.Level < ReqLvl - 1)
                return new string[] { PluginHandler.Instance.Translation.FailedLvl.Replace("${lvl}", ReqLvl.ToString()) };

            if (scp.Energy < Cost)
                return new string[] { PluginHandler.Instance.Translation.FailedAP.Replace("${ap}", Cost.ToString()) };

            if (!IsReady)
                return new string[] { PluginHandler.Instance.Translation.FailedCooldown.Replace("${time}", Cooldown.ToString()) };

            Events.EventHandler.OnUseWarheadStop(new Events.SCP079UseStopEventArgs(player));
            Warhead.Stop();
            Warhead.LeverStatus = false;
            Respawning.RespawnEffectsController.PlayCassieAnnouncement("PITCH_0.8 You jam_070_3 will jam_050_5 .g5 no jam_040_9 detonate me", false, false, true);
            scp.Energy -= Cost;
            _lastUse = DateTime.Now;

            RLogger.Log("SCP079 EVENT", "STOPWARHEAD", $"{player.PlayerToString()} requested stopwarhead");

            success = true;
            return new string[] { PluginHandler.Instance.Translation.Success };
        }

        internal static float Cooldown => PluginHandler.Instance.Config.CooldownStopWarhead;

        internal static float Cost => PluginHandler.Instance.Config.ApCostStopWarhead;

        internal static float ReqLvl => PluginHandler.Instance.Config.RequiedLvlStopWarhead;

        internal static bool IsReady => _lastUse.AddSeconds(Cooldown).Ticks <= DateTime.Now.Ticks;

        internal static long TimeLeft => _lastUse.AddSeconds(Cooldown).Ticks - DateTime.Now.Ticks;

        private static DateTime _lastUse = default;
    }
}
