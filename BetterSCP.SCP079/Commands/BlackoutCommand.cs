// -----------------------------------------------------------------------
// <copyright file="BlackoutCommand.cs" company="Mistaken">
// Copyright (c) Mistaken. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using CommandSystem;
using Exiled.API.Features;
using Exiled.API.Features.Roles;
using Mistaken.API.Commands;
using Mistaken.API.Extensions;
using Mistaken.RoundLogger;

namespace Mistaken.BetterSCP.SCP079.Commands
{
    [CommandHandler(typeof(ClientCommandHandler))]
    internal sealed class BlackoutCommand : IBetterCommand
    {
        public override string Command => "blackout";

        public override string Description => "Do blackout";

        public override string[] Execute(ICommandSender sender, string[] args, out bool success)
        {
            var player = sender.GetPlayer();
            var scp = (Scp079Role)player.Role;
            success = false;

            if (player.Role.Type != RoleType.Scp079)
                return new string[] { "Only SCP 079" };

            if (API.Utilities.Map.Overheat.LockBlackout)
                return new string[] { "Access denied\nFacility blackout system lockdown is active" };

            if (scp.Level < ReqLvl - 1)
                return new string[] { PluginHandler.Instance.Translation.FailedLvl.Replace("${lvl}", ReqLvl.ToString()) };

            if (!IsReady)
                return new string[] { PluginHandler.Instance.Translation.FailedCooldownBlackout.Replace("${time}", _lastCooldown.ToString()).Replace("${leftS}", (_lastUse - DateTime.Now).TotalSeconds.ToString()) };

            if (args.Length == 0)
                return new string[] { "Usage: " + this.GetUsage() };

            int duration;

            if (args[0].ToLower() == "max")
                duration = (int)Math.Floor(scp.Energy / BlackoutCommand.Cost);
            else if (!int.TryParse(args[0], out duration))
            {
                float max = float.MaxValue;
                string toreturn = PluginHandler.Instance.Translation.FailedNoNumberBlackout.Replace("${max}", max.ToString());
                return new string[] { toreturn };
            }

            var toDrain = duration * Cost;
            float cooldown = duration * BlackoutCommand.Cooldown;

            if (scp.Energy < toDrain)
                return new string[] { PluginHandler.Instance.Translation.FailedAP.Replace("${ap}", toDrain.ToString()) };

            Events.EventHandler.OnUseBlackout(new Events.SCP079UseBlackoutEventArgs(player, toDrain));

            Map.TurnOffAllLights(duration);

            scp.Energy -= Cost;
            _lastUse = DateTime.Now.AddSeconds(cooldown);
            _lastCooldown = cooldown;
            RLogger.Log("SCP079 EVENT", "BLACKOUT", $"{player.PlayerToString()} requested blackout for {duration}s");
            success = true;

            return new string[] { PluginHandler.Instance.Translation.Success };
        }

        internal static float Cooldown => PluginHandler.Instance.Config.CooldownBlackout;

        internal static float Cost => PluginHandler.Instance.Config.ApCostBlackout;

        internal static float ReqLvl => PluginHandler.Instance.Config.RequiedLvlBlackout;

        internal static bool IsReady => _lastUse.Ticks <= DateTime.Now.Ticks;

        internal static long TimeLeft => _lastUse.Ticks - DateTime.Now.Ticks;

        private static DateTime _lastUse = default;
        private static float _lastCooldown;

        private string GetUsage()
        {
            return ".blackout [duration/max]";
        }
    }
}
