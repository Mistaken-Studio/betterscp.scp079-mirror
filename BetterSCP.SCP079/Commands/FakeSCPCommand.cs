// -----------------------------------------------------------------------
// <copyright file="FakeSCPCommand.cs" company="Mistaken">
// Copyright (c) Mistaken. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Linq;
using CommandSystem;
using Exiled.API.Features;
using Mistaken.API.Commands;
using Mistaken.API.Extensions;
using Mistaken.RoundLogger;

namespace Mistaken.BetterSCP.SCP079.Commands
{
    /// <inheritdoc/>
    [CommandSystem.CommandHandler(typeof(CommandSystem.ClientCommandHandler))]
    public class FakeSCPCommand : IBetterCommand
    {
        /// <summary>
        /// Cause of death used for fake SCP.
        /// </summary>
        public enum FakeSCPDeathCause
        {
#pragma warning disable CS1591 // Brak komentarza XML dla widocznego publicznie typu lub składowej
            TESLA = 1,
            CHAOS,
            CLASSD,
            UNKNOWN,
            RECONTAINMENT,
            DECONTAMINATION,
#pragma warning restore CS1591 // Brak komentarza XML dla widocznego publicznie typu lub składowej
        }

        /// <inheritdoc/>
        public override string Command => "fakescp";

        /// <inheritdoc/>
        public override string[] Aliases => new string[] { };

        /// <inheritdoc/>
        public override string Description => "Fake SCP";

        /// <inheritdoc/>
        public override string[] Execute(ICommandSender sender, string[] args, out bool success)
        {
            var player = sender.GetPlayer();
            success = false;
            if (player.Role != RoleType.Scp079)
                return new string[] { "Only SCP 079" };
            if (player.Level >= ReqLvl - 1)
            {
                if (player.Energy >= Cost)
                {
                    if (IsReady)
                    {
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

                            switch (reason)
                            {
                                case FakeSCPDeathCause.TESLA:
                                    Cassie.Message("SCP " + processedtonumber + " SUCCESSFULLY TERMINATED BY AUTOMATIC SECURITY SYSTEM");
                                    break;
                                case FakeSCPDeathCause.CHAOS:
                                    Cassie.Message("SCP " + processedtonumber + " SUCCESSFULLY TERMINATED BY CHAOSINSURGENCY");
                                    break;
                                case FakeSCPDeathCause.CLASSD:
                                    Cassie.Message("SCP " + processedtonumber + " CONTAINEDSUCCESSFULLY BY CLASSD PERSONNEL");
                                    break;
                                case FakeSCPDeathCause.UNKNOWN:
                                    Cassie.Message("SCP " + processedtonumber + " SUCCESSFULLY TERMINATED . TERMINATION CAUSE UNSPECIFIED");
                                    break;
                                case FakeSCPDeathCause.RECONTAINMENT:
                                    Cassie.Message("SCP 1 0 6 RECONTAINED SUCCESSFULLY");
                                    break;
                                case FakeSCPDeathCause.DECONTAMINATION:
                                    Cassie.Message("SCP " + processedtonumber + " LOST IN DECONTAMINATION SEQUENCE");
                                    break;
                            }

                            SCP079Handler.GainXP(player, Cost);
                            lastUse = DateTime.Now;

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
            return ".fakescp [powód(liczba)] [scp]";
        }
    }
}
