﻿// -----------------------------------------------------------------------
// <copyright file="FullScanCommand.cs" company="Mistaken">
// Copyright (c) Mistaken. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Linq;
using CommandSystem;
using Exiled.API.Features;
using Exiled.API.Features.Roles;
using Mistaken.API;
using Mistaken.API.Commands;
using Mistaken.API.Extensions;
using Mistaken.RoundLogger;

namespace Mistaken.BetterSCP.SCP079.Commands
{
    [CommandHandler(typeof(ClientCommandHandler))]
    internal sealed class FullScanCommand : IBetterCommand
    {
        public override string Command => "fullscan";

        public override string Description => "Full Scanning";

        public override string[] Execute(ICommandSender sender, string[] args, out bool success)
        {
            var player = sender.GetPlayer();
            var scp = (Scp079Role)player.Role;
            success = false;

            if (player.Role.Type != RoleType.Scp079)
                return new string[] { "Only SCP 079" };

            if (scp.Level < ReqLvl - 1)
                return new string[] { PluginHandler.Instance.Translation.FailedLvl.Replace("${lvl}", ReqLvl.ToString()) };

            if (scp.Energy < Cost)
                return new string[] { PluginHandler.Instance.Translation.FailedAP.Replace("${ap}", Cost.ToString()) };

            if (!IsReady)
                return new string[] { PluginHandler.Instance.Translation.FailedCooldown.Replace("${time}", Cooldown.ToString()) };

            if (Generator.List.All(x => x.IsEngaged))
                return new string[] { PluginHandler.Instance.Translation.FailedAllGeneratorsEngaged };

            int ez = 0;
            int hcz = 0;
            int lcz = 0;
            int nuke = 0;
            int scp049 = 0;
            int pocket = 0;
            int surface = 0;

            foreach (var item in RealPlayers.List.Where(x => x.IsAlive && x.Role.Type != RoleType.Tutorial))
            {
                if (item.IsInPocketDimension)
                    pocket++;
                else if (item.Position.y > 900)
                    surface++;
                else if (item.Position.y > -100)
                    lcz++;
                else if (item.Position.y > -600)
                    nuke++;
                else if (item.Position.y > -800)
                    scp049++;
                else if (item.CurrentRoom.Zone == Exiled.API.Enums.ZoneType.Entrance)
                    ez++;
                else if (item.CurrentRoom.Zone == Exiled.API.Enums.ZoneType.HeavyContainment)
                    hcz++;
            }

            string message = "Full facility scan initiated";
            if (surface != 0)
                message += string.Format(" . {0} SURFACE", surface);
            if (ez != 0)
                message += string.Format(" . {0} Entrance Zone", ez);
            if (hcz != 0)
                message += string.Format(" . {0} Heavy containment zone", hcz);
            if (lcz != 0)
                message += string.Format(" . {0} Light containment zone", lcz);
            if (nuke != 0)
                message += string.Format(" . {0} Alpha Warhead", nuke);
            if (scp049 != 0)
                message += string.Format(" . {0} SCP 0 4 9 containment chamber", scp049);
            if (pocket != 0)
                message += string.Format(" . {0} Unknown", pocket);

            Events.EventHandler.OnUseScan(new Events.SCP079UseScanEventArgs(player, true));

            if (message != "Full facility scan initiated")
                Respawning.RespawnEffectsController.PlayCassieAnnouncement(message, false, false, true);
            else
                Respawning.RespawnEffectsController.PlayCassieAnnouncement("DETECTED UNKNOWN SECURITY SYSTEM ERROR . FAILED TO SCAN", false, false, true);

            scp.Energy -= Cost;
            _lastUse = DateTime.Now;

            RLogger.Log("SCP079 EVENT", "FULLSCAN", $"{player.PlayerToString()} requested fullscan");
            success = true;
            return new string[] { PluginHandler.Instance.Translation.Success };
        }

        internal static float Cooldown => PluginHandler.Instance.Config.Cooldown;

        internal static float Cost => PluginHandler.Instance.Config.ApCost;

        internal static float ReqLvl => PluginHandler.Instance.Config.RequiedLvl;

        internal static bool IsReady => _lastUse.AddSeconds(Cooldown).Ticks <= DateTime.Now.Ticks;

        internal static long TimeLeft => _lastUse.AddSeconds(Cooldown).Ticks - DateTime.Now.Ticks;

        private static DateTime _lastUse = default;
    }
}
