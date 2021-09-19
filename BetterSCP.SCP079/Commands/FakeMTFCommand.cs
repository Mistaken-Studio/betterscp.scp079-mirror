// -----------------------------------------------------------------------
// <copyright file="FakeMTFCommand.cs" company="Mistaken">
// Copyright (c) Mistaken. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Linq;
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
    public class FakeMTFCommand : IBetterCommand
    {
        /// <inheritdoc/>
        public override string Command => "fakemtf";

        /// <inheritdoc/>
        public override string[] Aliases => new string[] { };

        /// <inheritdoc/>
        public override string Description => "Fake MTF";

        /// <inheritdoc/>
        public override string[] Execute(ICommandSender sender, string[] args, out bool success)
        {
            success = false;
            var player = sender.GetPlayer();
            if (player.Role != RoleType.Scp079)
                return new string[] { "Only SCP 079" };
            if (player.Level >= ReqLvl - 1)
            {
                if (player.Energy >= Cost)
                {
                    if (IsReady)
                    {
                        int number = UnityEngine.Random.Range(0, 20);
                        char letter = this.RandomChar();
                        int scps = RealPlayers.List.Where(p => p.Team == Team.SCP && p.Role != RoleType.Scp0492).Count();
                        Cassie.Message($"MTFUNIT EPSILON 11 DESIGNATED NATO_{letter} {number} HASENTERED ALLREMAINING AWAITINGRECONTAINMENT {scps} SCPSUBJECT{(scps == 1 ? string.Empty : "S")}");
                        SCP079Handler.GainXP(player, Cost);
                        lastUse = DateTime.Now;

                        RLogger.Log("SCP079 EVENT", "FAKEMTF", $"{player.PlayerToString()} requested fakemtf");

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
                return new string[] { PluginHandler.Instance.Translation.FailedLvl.Replace("${lvl}", ReqLvl.ToString()) };
        }

        internal static float Cooldown => PluginHandler.Instance.Config.Cooldown;

        internal static float Cost => PluginHandler.Instance.Config.ApCost;

        internal static float ReqLvl => PluginHandler.Instance.Config.RequiedLvl;

        internal static bool IsReady => lastUse.AddSeconds(Cooldown).Ticks <= DateTime.Now.Ticks;

        internal static long TimeLeft => lastUse.AddSeconds(Cooldown).Ticks - DateTime.Now.Ticks;

        private static DateTime lastUse = default(DateTime);

        private string GetUsage()
        {
            return ".fakemtf";
        }

        private char RandomChar()
        {
            string alpha = "ABCDEFGHIJKLMNOPQRSTWYXZ";
            return alpha[UnityEngine.Random.Range(0, alpha.Length)];
        }
    }
}
