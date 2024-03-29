﻿// -----------------------------------------------------------------------
// <copyright file="FakeSCPCommand.cs" company="Mistaken">
// Copyright (c) Mistaken. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using CommandSystem;
using Exiled.API.Features;
using Exiled.API.Features.Roles;
using Mistaken.API.Commands;
using Mistaken.API.Extensions;
using Mistaken.RoundLogger;
using Utils.Networking;

namespace Mistaken.BetterSCP.SCP079.Commands
{
    [CommandHandler(typeof(ClientCommandHandler))]
    internal sealed class FakeSCPCommand : IBetterCommand
    {
        public override string Command => "fakescp";

        public override string Description => "Fake SCP";

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

            if (!SCP079Handler.IsGlobalReady)
                return new string[] { PluginHandler.Instance.Translation.FailedGlobalCooldown.Replace("${time}", SCP079Handler.GlobalCooldown.ToString()) };

            if (!IsReady)
                return new string[] { PluginHandler.Instance.Translation.FailedCooldown.Replace("${time}", Cooldown.ToString()) };

            if (Generator.List.All(x => x.IsEngaged))
                return new string[] { PluginHandler.Instance.Translation.FailedAllGeneratorsEngaged };

            if (args.Length == 0 || !int.TryParse(args[0], out int rawReason) || rawReason < 1 || rawReason > 6)
            {
                return new string[]
                {
                    ".fakescp [przyczyna] [scp]",
                    "Podaj przyczynę śmierci:",
                    "1. Tesla",
                    "2. CI",
                    "3. Klasa D",
                    "4. Nieznany",
                    "5. Zrekontaminowany (Działa tylko na SCP-106)",
                    "6. Śmierć w Dekontaminacji LCZ",
                };
            }

            FakeSCPDeathCause reason = (FakeSCPDeathCause)rawReason;

            args = args.Skip(1).ToArray();

            if (args.Length == 0)
            {
                int max = short.MaxValue;
                return new string[] { PluginHandler.Instance.Translation.FailedNoNumber.Replace("${max}", max.ToString()) };
            }

            string processed = string.Empty;
            foreach (char item in string.Join(string.Empty, args).ToCharArray())
            {
                if (item == ' ') continue;
                processed += item;
            }

            if (processed.Length <= 5)
            {
                if (!short.TryParse(processed, out _))
                    return new string[] { PluginHandler.Instance.Translation.FailedWrongNumber.Replace("${max}", short.MaxValue.ToString()) };

                string processedtonumber = " ";

                foreach (char item in processed.ToCharArray())
                {
                    if (item != ' ')
                    {
                        try
                        {
                            if (short.TryParse(item.ToString(), out short num)) processedtonumber += num + " ";
                        }
                        catch
                        {
                        }
                    }
                }

                Events.EventHandler.OnUseFakeSCP(new Events.SCP079UseFakeSCPEventArgs(player, processedtonumber, reason));

                List<Subtitles.SubtitlePart> subtitles = new(1);

                if (reason == FakeSCPDeathCause.RECONTAINMENT)
                    processedtonumber = "1 0 6";

                subtitles.Add(new Subtitles.SubtitlePart(Subtitles.SubtitleType.SCP, new string[] { processedtonumber.Replace(" ", string.Empty) }));

                switch (reason)
                {
                    case FakeSCPDeathCause.TESLA:
                        Cassie.Message("SCP " + processedtonumber + " SUCCESSFULLY TERMINATED BY AUTOMATIC SECURITY SYSTEM");
                        subtitles.Add(new Subtitles.SubtitlePart(Subtitles.SubtitleType.TerminatedBySecuritySystem, new string[0]));
                        break;
                    case FakeSCPDeathCause.CHAOS:
                        Cassie.Message("SCP " + processedtonumber + " CONTAINEDSUCCESSFULLY BY CHAOSINSURGENCY");
                        subtitles.Add(new Subtitles.SubtitlePart(Subtitles.SubtitleType.ContainedByChaos, new string[0]));
                        break;
                    case FakeSCPDeathCause.CLASSD:
                        Cassie.Message("SCP " + processedtonumber + " CONTAINEDSUCCESSFULLY BY CLASSD PERSONNEL");
                        subtitles.Add(new Subtitles.SubtitlePart(Subtitles.SubtitleType.ContainedByClassD, new string[0]));
                        break;
                    case FakeSCPDeathCause.UNKNOWN:
                        Cassie.Message("SCP " + processedtonumber + " SUCCESSFULLY TERMINATED . TERMINATION CAUSE UNSPECIFIED");
                        subtitles.Add(new Subtitles.SubtitlePart(Subtitles.SubtitleType.TerminationCauseUnspecified, new string[0]));
                        break;
                    case FakeSCPDeathCause.RECONTAINMENT:
                        Cassie.Message("SCP 1 0 6 RECONTAINED SUCCESSFULLY");
                        subtitles.Add(new Subtitles.SubtitlePart(Subtitles.SubtitleType.Custom, new string[] { "RECONTAINED SUCCESSFULLY" }));
                        break;
                    case FakeSCPDeathCause.DECONTAMINATION:
                        Cassie.Message("SCP " + processedtonumber + " LOST IN DECONTAMINATION SEQUENCE");
                        subtitles.Add(new Subtitles.SubtitlePart(Subtitles.SubtitleType.LostInDecontamination, new string[0]));
                        break;
                }

                new Subtitles.SubtitleMessage(subtitles.ToArray()).SendToAuthenticated(0);

                SCP079Handler.LastGlobalUse = DateTime.Now;
                scp.Energy -= Cost;
                _lastUse = DateTime.Now;

                RLogger.Log("SCP079 EVENT", "FAKESCP", $"{player.PlayerToString()} requested fakescp of SCP {processedtonumber} with reason: {reason}");

                success = true;
                return new string[] { PluginHandler.Instance.Translation.Success };
            }
            else
            {
                int max = short.MaxValue;
                return new string[] { PluginHandler.Instance.Translation.FailedWrongNumber.Replace("${max}", max.ToString()) };
            }
        }

        internal static float Cooldown => PluginHandler.Instance.Config.Cooldown;

        internal static float Cost => PluginHandler.Instance.Config.ApCost;

        internal static float ReqLvl => PluginHandler.Instance.Config.RequiedLvl;

        internal static bool IsReady => _lastUse.AddSeconds(Cooldown).Ticks <= DateTime.Now.Ticks;

        internal static long TimeLeft => _lastUse.AddSeconds(Cooldown).Ticks - DateTime.Now.Ticks;

        private static DateTime _lastUse = default;
    }
}
