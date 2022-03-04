// -----------------------------------------------------------------------
// <copyright file="CassieCommand.cs" company="Mistaken">
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
    public class CassieCommand : IBetterCommand
    {
        /// <inheritdoc/>
        public override string Command => "cassie";

        /// <inheritdoc/>
        public override string[] Aliases => new string[] { };

        /// <inheritdoc/>
        public override string Description => "Play custom cassie";

        /// <inheritdoc/>
        public override string[] Execute(ICommandSender sender, string[] args, out bool success)
        {
            var player = sender.GetPlayer();
            success = false;
            if (player.Role != RoleType.Scp079)
                return new string[] { "Only SCP 079" };

            if (player.Level < ReqLvl - 1)
                return new string[] { PluginHandler.Instance.Translation.FailedLvl.Replace("${lvl}", ReqLvl.ToString()) };

            if (player.Energy < Cost)
                return new string[] { PluginHandler.Instance.Translation.FailedAP.Replace("${ap}", Cost.ToString()) };

            if (!IsReady)
                return new string[] { PluginHandler.Instance.Translation.FailedCooldown.Replace("${time}", Cooldown.ToString()) };

            if (args.Length == 0)
                return new string[] { "Usage: " + this.GetUsage() };

            string message = string.Join(" ", args);
            if (args.Length > 20 || message.Length > 250)
                return new string[] { "Wiadomość nie może być dłuższa niż 20 słów i max 250 znaków licząc spację" };
            message = message.ToLower();
            while (message.Contains("jam_"))
                message = message.Replace("jam_", string.Empty);
            while (message.Contains(".g"))
                message = message.Replace(".g", string.Empty);
            while (message.Contains("yield_"))
                message = message.Replace("yield_", string.Empty);
            while (message.Contains("pitch_"))
                message = message.Replace("pitch_", string.Empty);
            while (message.Contains("xmas_"))
                message = message.Replace("xmas_", string.Empty);
            while (message.Contains("bell_"))
                message = message.Replace("bell_", string.Empty);

            Events.EventHandler.OnUseCassie(new Events.SCP079UseCassieEventArgs(player, message));

            Cassie.Message("PITCH_0.9 SCP 0 PITCH_0.9 7 PITCH_0.9 9 PITCH_0.9 jam_050_5 OVERRIDE PITCH_1 . . . " + message);
            SCP079Handler.GainXP(player, Cost);
            lastUse = DateTime.Now;
            RLogger.Log("SCP079 EVENT", "CASSIE", $"{player.PlayerToString()} requested cassie \"{message}\"");

            success = true;
            return new string[] { PluginHandler.Instance.Translation.Success };
        }

        internal static float Cooldown => PluginHandler.Instance.Config.CooldownCassie;

        internal static float Cost => PluginHandler.Instance.Config.ApCostCassie;

        internal static float ReqLvl => PluginHandler.Instance.Config.RequiedLvlCassie;

        internal static bool IsReady => lastUse.AddSeconds(Cooldown).Ticks <= DateTime.Now.Ticks;

        internal static long TimeLeft => lastUse.AddSeconds(Cooldown).Ticks - DateTime.Now.Ticks;

        private static DateTime lastUse = default(DateTime);

        private string GetUsage()
        {
            return ".cassie [MESSAGE]";
        }
    }
}
