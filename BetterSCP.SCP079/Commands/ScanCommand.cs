// -----------------------------------------------------------------------
// <copyright file="ScanCommand.cs" company="Mistaken">
// Copyright (c) Mistaken. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Linq;
using CommandSystem;
using Exiled.API.Features.Roles;
using Mistaken.API;
using Mistaken.API.Commands;
using Mistaken.API.Extensions;
using Mistaken.RoundLogger;

namespace Mistaken.BetterSCP.SCP079.Commands
{
    [CommandHandler(typeof(ClientCommandHandler))]
    internal sealed class ScanCommand : IBetterCommand
    {
        public override string Command => "scan";

        public override string Description => "Scanning";

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

            Events.EventHandler.OnUseScan(new Events.SCP079UseScanEventArgs(player, false));

            string message = PluginHandler.Instance.Translation.Scan;
            message = message
                .Replace("{ez}", ez.ToString())
                .Replace("{hcz}", hcz.ToString())
                .Replace("{lcz}", lcz.ToString())
                .Replace("{nuke}", nuke.ToString())
                .Replace("{049}", scp049.ToString())
                .Replace("{pocket}", pocket.ToString())
                .Replace("{surface}", surface.ToString());

            string[] argsScan = message.Split('|');

            for (int i = 0; i < argsScan.Length; i++)
                player.SendConsoleMessage(argsScan[i], "red");

            scp.Energy -= Cost;
            _lastUse = DateTime.Now;

            RLogger.Log("SCP079 EVENT", "SCAN", $"{player.PlayerToString()} requested scan");

            success = true;
            return new string[] { PluginHandler.Instance.Translation.Success };
        }

        internal static float Cooldown => PluginHandler.Instance.Config.CooldownScan;

        internal static float Cost => PluginHandler.Instance.Config.ApCostScan;

        internal static float ReqLvl => PluginHandler.Instance.Config.RequiedLvlScan;

        internal static bool IsReady => _lastUse.AddSeconds(Cooldown).Ticks <= DateTime.Now.Ticks;

        internal static long TimeLeft => _lastUse.AddSeconds(Cooldown).Ticks - DateTime.Now.Ticks;

        private static DateTime _lastUse = default;
    }
}
