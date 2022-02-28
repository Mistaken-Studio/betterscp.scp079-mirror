// -----------------------------------------------------------------------
// <copyright file="BlackoutCommand.cs" company="Mistaken">
// Copyright (c) Mistaken. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using CommandSystem;
using Exiled.API.Features;
using Mistaken.API.Commands;
using Mistaken.API.Extensions;
using Mistaken.RoundLogger;

namespace Mistaken.BetterSCP.SCP079.Commands
{
    /// <inheritdoc/>
    [CommandSystem.CommandHandler(typeof(CommandSystem.ClientCommandHandler))]
    public class BlackoutCommand : IBetterCommand
    {
        /// <inheritdoc/>
        public override string Command => "blackout";

        /// <inheritdoc/>
        public override string[] Aliases => new string[] { };

        /// <inheritdoc/>
        public override string Description => "Do blackout";

        /// <inheritdoc/>
        public override string[] Execute(ICommandSender sender, string[] args, out bool success)
        {
            var player = sender.GetPlayer();
            success = false;
            if (player.Role != RoleType.Scp079)
                return new string[] { "Only SCP 079" };
            if (API.Utilities.Map.Overheat.LockBlackout)
                return new string[] { "Access denied\nFacility blackout system lockdown is active" };
            if (player.Level >= ReqLvl - 1)
            {
                if (IsReady)
                {
                    if (args.Length == 0)
                        return new string[] { "Usage: " + this.GetUsage() };
                    else
                    {
                        if (int.TryParse(args[0], out int duration) || args[0].ToString().ToLower().Contains("max"))
                        {
                            if (args[0].ToString().ToLower().Contains("max"))
                            {
                                duration = player.Energy.ToString()[0];
                            }
                            var toDrain = duration * Cost;
                            float cooldown = duration * BlackoutCommand.Cooldown;

                            if (player.Energy >= toDrain)
                            {
                                Events.EventHandler.OnUseBlackout(new Events.SCP079UseBlackoutEventArgs(player, toDrain));

                                Map.TurnOffAllLights(duration);

                                SCP079Handler.GainXP(player, toDrain);
                                lastUse = DateTime.Now.AddSeconds(cooldown);
                                this.lastCooldown = cooldown;
                                RLogger.Log("SCP079 EVENT", "BLACKOUT", $"{player.PlayerToString()} requested blackout for {duration}s");
                                success = true;
                                return new string[] { PluginHandler.Instance.Translation.Success };
                            }
                            else
                                return new string[] { PluginHandler.Instance.Translation.FailedAP.Replace("${ap}", toDrain.ToString()) };
                        }
                        else
                        {
                            float max = float.MaxValue;
                            string toreturn = PluginHandler.Instance.Translation.FailedNoNumberBlackout.Replace("${max}", max.ToString());
                            return new string[] { toreturn };
                        }
                    }
                }
                else
                {
                    return new string[] { PluginHandler.Instance.Translation.FailedCooldownBlackout.Replace("${time}", this.lastCooldown.ToString()).Replace("${leftS}", (lastUse - DateTime.Now).TotalSeconds.ToString()) };
                }
            }
            else
            {
                return new string[] { PluginHandler.Instance.Translation.FailedLvl.Replace("${lvl}", ReqLvl.ToString()) };
            }
        }

        internal static float Cooldown => PluginHandler.Instance.Config.CooldownBlackout;

        internal static float Cost => PluginHandler.Instance.Config.ApCostBlackout;

        internal static float ReqLvl => PluginHandler.Instance.Config.RequiedLvlBlackout;

        internal static bool IsReady => lastUse.Ticks <= DateTime.Now.Ticks;

        internal static long TimeLeft => lastUse.Ticks - DateTime.Now.Ticks;

        private static DateTime lastUse = default(DateTime);
        private float lastCooldown;

        private string GetUsage()
        {
            return ".blackout [duration]";
        }
    }
}
