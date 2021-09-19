// -----------------------------------------------------------------------
// <copyright file="FakeCICommand.cs" company="Mistaken">
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
    public class FakeCICommand : IBetterCommand
    {
        /// <inheritdoc/>
        public override string Command => "fakeci";

        /// <inheritdoc/>
        public override string[] Aliases => new string[] { };

        /// <inheritdoc/>
        public override string Description => "Fake CI";

        /// <inheritdoc/>
        public override string[] Execute(ICommandSender sender, string[] args, out bool success)
        {
            success = false;
            return new string[] { "Command Disabled" };
            /*var player = sender.GetPlayer();
            if (player.Role != RoleType.Scp079)
                return new string[] { "Only SCP 079" };
            if (player.Level >= ReqLvl - 1)
            {
                if (player.Energy >= Cost)
                {
                    if (IsReady)
                    {
                        Cassie.Message(BetterRP.Handler.CIAnnouncments[UnityEngine.Random.Range(0, BetterRP.Handler.CIAnnouncments.Length)]);
                        SCP079Handler.GainXP(player, Cost);
                        Lastuse = DateTime.Now;
                        //CustomAchievements.RoundEventHandler.AddProggress("Manipulator", player);

                        RLogger.Log("SCP079 EVENT", "FAKECI", $"{player.PlayerToString()} requested fakeci");

                        success = true;
                        return new string[] { PluginHandler.Instance.Translation.Success };
                    }
                    else
                        return new string[] { PluginHandler.Instance.Translation.FailedCooldown.Replace("${time}", Cooldown.ToString()) };
                }
                else
                    return new string[] { PluginHandler.Instance.Translation.FailedAP.Replace("${ap}", Cost.ToString()) };
            }
            else
                return new string[] { PluginHandler.Instance.Translation.FailedLvl.Replace("${lvl}", ReqLvl.ToString()) };*/
        }

        internal static float Cooldown => PluginHandler.Instance.Config.Cooldown;

        internal static float Cost => PluginHandler.Instance.Config.ApCost;

        internal static float ReqLvl => PluginHandler.Instance.Config.RequiedLvl;

        internal static bool IsReady => lastUse.AddSeconds(Cooldown).Ticks <= DateTime.Now.Ticks;

        internal static long TimeLeft => lastUse.AddSeconds(Cooldown).Ticks - DateTime.Now.Ticks;

        private static DateTime lastUse = default(DateTime);

        private string GetUsage()
        {
            return ".fakeci";
        }
    }
}
