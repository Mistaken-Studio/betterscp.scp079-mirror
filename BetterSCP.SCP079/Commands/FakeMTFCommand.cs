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
                        Events.EventHandler.OnUseFakeMTF(new Events.SCP079UseFakeMTFEventArgs(player));

                        Respawning.NamingRules.UnitNamingRules.TryGetNamingRule(Respawning.SpawnableTeamType.NineTailedFox, out var ntfRule);
                        ntfRule.GenerateNew(Respawning.SpawnableTeamType.NineTailedFox, out string unitName);
                        string number = unitName.Split('-')[1];
                        char letter = unitName[0];

                        if (lastFakeUnitIndex != -1)
                        {
                            Respawning.NamingRules.UnitNamingRule.UsedCombinations.Remove(lastFakeUnit);
                            Respawning.RespawnManager.Singleton.NamingManager.AllUnitNames.RemoveAt(lastFakeUnitIndex);
                        }

                        lastFakeUnit = unitName;
                        lastFakeUnitIndex = Respawning.RespawnManager.Singleton.NamingManager.AllUnitNames.Count - 1;
                        string tmp = Respawning.RespawnManager.Singleton.NamingManager.AllUnitNames[Respawning.RespawnManager.Singleton.NamingManager.AllUnitNames.Count - 2].UnitName;
                        int colorIndex = tmp.IndexOf("<color=");
                        if (colorIndex != -1)
                        {
                            string color = tmp.Substring(colorIndex + 8, tmp.IndexOf('>', colorIndex));
                            Log.Debug(color, PluginHandler.Instance.Config.VerbouseOutput);
                            Map.ChangeUnitColor(lastFakeUnitIndex, color);
                        }

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
        private static string lastFakeUnit = null;
        private static int lastFakeUnitIndex = -1;

        private string GetUsage()
        {
            return ".fakemtf";
        }
    }
}
