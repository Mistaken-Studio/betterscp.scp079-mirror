// -----------------------------------------------------------------------
// <copyright file="FakeCICommand.cs" company="Mistaken">
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
    internal sealed class FakeCICommand : IBetterCommand
    {
        public override string Command => "fakeci";

        public override string Description => "Fake CI";

        public override string[] Execute(ICommandSender sender, string[] args, out bool success)
        {
            try
            {
                return this.ExecuteInternal(sender, args, out success);
            }
            catch (Exception ex)
            {
                success = false;
                Log.Debug(ex);
                return new string[] { "Command Disabled" };
            }
        }

        internal static float Cooldown => PluginHandler.Instance.Config.Cooldown;

        internal static float Cost => PluginHandler.Instance.Config.ApCost;

        internal static float ReqLvl => PluginHandler.Instance.Config.RequiedLvl;

        internal static bool IsReady => _lastUse.AddSeconds(Cooldown).Ticks <= DateTime.Now.Ticks;

        internal static long TimeLeft => _lastUse.AddSeconds(Cooldown).Ticks - DateTime.Now.Ticks;

        internal string[] ExecuteInternal(ICommandSender sender, string[] args, out bool success)
        {
            success = false;
            var player = sender.GetPlayer();
            var scp = (Scp079Role)player.Role;

            if (player.Role != RoleType.Scp079)
                return new string[] { "Only SCP 079" };

            if (scp.Level < ReqLvl - 1)
                return new string[] { PluginHandler.Instance.Translation.FailedLvl.Replace("${lvl}", ReqLvl.ToString()) };

            if (scp.Energy < Cost)
                return new string[] { PluginHandler.Instance.Translation.FailedAP.Replace("${ap}", Cost.ToString()) };

            if (!SCP079Handler.IsGlobalReady)
                return new string[] { PluginHandler.Instance.Translation.FailedGlobalCooldown.Replace("${time}", SCP079Handler.GlobalCooldown.ToString()) };

            if (!IsReady)
                return new string[] { PluginHandler.Instance.Translation.FailedCooldown.Replace("${time}", Cooldown.ToString()) };

            Events.EventHandler.OnUseFakeCI(new Events.SCP079UseFakeCIEventArgs(player));

            Respawning.RespawnEffectsController.PlayCassieAnnouncement(BetterRP.BetterRPHandler.CIAnnouncments[UnityEngine.Random.Range(0, BetterRP.BetterRPHandler.CIAnnouncments.Length)], false, false, true);
            scp.Energy -= Cost;
            SCP079Handler.LastGlobalUse = DateTime.Now;
            _lastUse = DateTime.Now;

            RLogger.Log("SCP079 EVENT", "FAKECI", $"{player.PlayerToString()} requested fakeci");

            success = true;
            return new string[] { PluginHandler.Instance.Translation.Success };
        }

        private static DateTime _lastUse = default;
    }
}
