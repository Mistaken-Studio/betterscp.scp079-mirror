// -----------------------------------------------------------------------
// <copyright file="FakeMTFCommand.cs" company="Mistaken">
// Copyright (c) Mistaken. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using CommandSystem;
using Exiled.API.Features;
using Exiled.API.Features.Roles;
using Mistaken.API;
using Mistaken.API.Commands;
using Mistaken.API.Diagnostics;
using Mistaken.API.Extensions;
using Mistaken.RoundLogger;
using Utils.Networking;

namespace Mistaken.BetterSCP.SCP079.Commands
{
    [CommandHandler(typeof(ClientCommandHandler))]
    internal sealed class FakeMTFCommand : IBetterCommand
    {
        public override string Command => "fakemtf";

        public override string Description => "Fake MTF";

        public override string[] Execute(ICommandSender sender, string[] args, out bool success)
        {
            success = false;
            var player = sender.GetPlayer();
            var scp = (Scp079Role)player.Role;

            if (player.Role.Type != RoleType.Scp079)
                return new string[] { "Only SCP 079" };

            if (scp.Level < ReqLvl - 1)
                return new string[] { PluginHandler.Instance.Translation.FailedLvl.Replace("${lvl}", ReqLvl.ToString()) };

            if (scp.Energy < Cost)
                return new string[] { PluginHandler.Instance.Translation.FailedAP.Replace("${ap}", Cost.ToString()) };

            if (!SCP079Handler.IsGlobalReady)
                return new string[] { PluginHandler.Instance.Translation.FailedGlobalCooldown.Replace("${time}", SCP079Handler.GlobalCooldown.ToString()) };

            if (!IsReady)
                return new string[] { PluginHandler.Instance.Translation.FailedCooldown.Replace("${time}", Cooldown.ToString()) };

            Events.EventHandler.OnUseFakeMTF(new Events.SCP079UseFakeMTFEventArgs(player));

            Respawning.NamingRules.UnitNamingRules.TryGetNamingRule(Respawning.SpawnableTeamType.NineTailedFox, out var ntfRule);
            ntfRule.GenerateNew(Respawning.SpawnableTeamType.NineTailedFox, out string unitName);
            string number = unitName.Split('-')[1];
            char letter = unitName[0];

            if (_lastFakeUnitIndex != -1)
            {
                Respawning.NamingRules.UnitNamingRule.UsedCombinations.Remove(_lastFakeUnit);
                Respawning.RespawnManager.Singleton.NamingManager.AllUnitNames.RemoveAt(_lastFakeUnitIndex);
            }

            _lastFakeUnit = unitName;
            _lastFakeUnitIndex = Respawning.RespawnManager.Singleton.NamingManager.AllUnitNames.Count - 1;
            string tmp = Respawning.RespawnManager.Singleton.NamingManager.AllUnitNames[Respawning.RespawnManager.Singleton.NamingManager.AllUnitNames.Count - 2].UnitName;
            int colorIndex = tmp.IndexOf("<color=");

            if (colorIndex != -1)
            {
                string color = tmp.Substring(colorIndex + 7, tmp.IndexOf('>', colorIndex) - (colorIndex + 7));
                Log.Debug(color, PluginHandler.Instance.Config.VerboseOutput);
                Map.ChangeUnitColor(_lastFakeUnitIndex, color);

                Module.CallSafeDelayed(2, () => SCPGUIHandler.ResyncAllUnits(), "FAKEMTF.ResyncAllUnits");
            }

            int scps = RealPlayers.List.Where(p => p.Role.Team == Team.SCP && p.Role.Type != RoleType.Scp0492).Count(); // Can't be 0 because there has to be 079
            Cassie.Message($"MTFUNIT EPSILON 11 DESIGNATED NATO_{letter} {number} HASENTERED ALLREMAINING AWAITINGRECONTAINMENT {scps} SCPSUBJECT{(scps == 1 ? string.Empty : "S")}");
            List<Subtitles.SubtitlePart> list = new()
            {
                new Subtitles.SubtitlePart(Subtitles.SubtitleType.NTFEntrance, new string[] { unitName }),
            };

            if (scps == 1)
                list.Add(new Subtitles.SubtitlePart(Subtitles.SubtitleType.AwaitContainSingle, null));
            else
                list.Add(new Subtitles.SubtitlePart(Subtitles.SubtitleType.AwaitContainPlural, new string[] { scps.ToString() }));

            new Subtitles.SubtitleMessage(list.ToArray()).SendToAuthenticated(0);
            scp.Energy -= Cost;
            SCP079Handler.LastGlobalUse = DateTime.Now;
            _lastUse = DateTime.Now;

            RLogger.Log("SCP079 EVENT", "FAKEMTF", $"{player.PlayerToString()} requested fakemtf");

            success = true;
            return new string[] { PluginHandler.Instance.Translation.Success };
        }

        internal static float Cooldown => PluginHandler.Instance.Config.Cooldown;

        internal static float Cost => PluginHandler.Instance.Config.ApCost;

        internal static float ReqLvl => PluginHandler.Instance.Config.RequiedLvl;

        internal static bool IsReady => _lastUse.AddSeconds(Cooldown).Ticks <= DateTime.Now.Ticks;

        internal static long TimeLeft => _lastUse.AddSeconds(Cooldown).Ticks - DateTime.Now.Ticks;

        private static DateTime _lastUse = default;
        private static string _lastFakeUnit = null;
        private static int _lastFakeUnitIndex = -1;
    }
}
