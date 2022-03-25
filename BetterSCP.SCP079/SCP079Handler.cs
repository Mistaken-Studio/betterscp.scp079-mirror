// -----------------------------------------------------------------------
// <copyright file="SCP079Handler.cs" company="Mistaken">
// Copyright (c) Mistaken. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using Exiled.API.Features;
using Exiled.API.Features.Roles;
using Mistaken.API;
using Mistaken.API.Diagnostics;
using Mistaken.API.Extensions;
using Mistaken.API.GUI;
using Mistaken.BetterSCP.SCP079.Commands;
using UnityEngine;

namespace Mistaken.BetterSCP.SCP079
{
    internal class SCP079Handler : Module
    {
        public static float GlobalCooldown => PluginHandler.Instance.Config.GlobalCooldown;

        public static bool IsGlobalReady => lastGlobalUse.AddSeconds(GlobalCooldown).Ticks <= DateTime.Now.Ticks;

        public static long GlobalTimeLeft => lastGlobalUse.AddSeconds(GlobalCooldown).Ticks - DateTime.Now.Ticks;

        public static DateTime lastGlobalUse = default(DateTime);

        public static void GainXP(Player player, float ap)
        {
            var scp = (Scp079Role)player.Role;
            scp.Energy -= ap;
            var id = scp.Level;
            if (id >= scp.Levels.Length)
                id = (byte)(scp.Levels.Length - 1);
            else if (id < 0)
                id = 0;
            float num4 = 1f / Mathf.Clamp(scp.Levels[id].manaPerSecond / 1.5f, 1f, 5f);
            ap = Mathf.Round(ap * num4 * 10f) / 10f;
            player.ReferenceHub.scp079PlayerScript.AddExperience(ap);
        }

        public SCP079Handler(PluginHandler plugin)
            : base(plugin)
        {
        }

        public override string Name => nameof(SCP079Handler);

        public override void OnEnable()
        {
            SCPGUIHandler.SCPMessages[RoleType.Scp079] = PluginHandler.Instance.Translation.StartMessage;

            Exiled.Events.Handlers.Server.RoundStarted += this.Server_RoundStarted;
            Exiled.Events.Handlers.Player.VoiceChatting += this.Player_VoiceChatting;
        }

        public override void OnDisable()
        {
            SCPGUIHandler.SCPMessages.Remove(RoleType.Scp079);

            Exiled.Events.Handlers.Server.RoundStarted -= this.Server_RoundStarted;
            Exiled.Events.Handlers.Player.VoiceChatting -= this.Player_VoiceChatting;
        }

        private static readonly HashSet<Player> PressingAltVCKey = new HashSet<Player>();

        private static float MapScan_CostPerStart => PluginHandler.Instance.Config.ApStartCostAdvancedScan;

        private static float MapScan_CostPerUpdate => PluginHandler.Instance.Config.ApCostAdvancedScan;

        private static float MapScan_RequiedLvl => PluginHandler.Instance.Config.RequiedLvlAdvancedScan;

        private static float MapScan_DisableDelay => PluginHandler.Instance.Config.AdvancedScanDisableDelay;

        private static float MapScan_UpdateRate => PluginHandler.Instance.Config.AdvancedScanUpdateRate;

        private static IEnumerator<float> HandleNewGUI(Player player)
        {
            var scp = (Scp079Role)player.Role;
            if (scp.Level < MapScan_RequiedLvl)
                yield break;

            if (scp.Energy < MapScan_CostPerStart)
                yield break;
            PressingAltVCKey.Add(player);
            scp.Energy -= MapScan_CostPerStart;
            List<Vector3> lastData = new List<Vector3>();
            while (PressingAltVCKey.Contains(player))
            {
                if (scp.Energy < MapScan_CostPerUpdate)
                    break;
                scp.Energy -= MapScan_CostPerUpdate;
                lastData = RealPlayers.List.Where(x => x.IsAlive).Select(x => x.Position).ToList();
                player.ReferenceHub.scp079PlayerScript.TargetSetupIndicators(player.Connection, lastData);
                yield return MEC.Timing.WaitForSeconds(MapScan_UpdateRate);
            }

            PressingAltVCKey.Remove(player);
            for (int i = 0; i < MapScan_DisableDelay / MapScan_UpdateRate; i++)
            {
                player.ReferenceHub.scp079PlayerScript.TargetSetupIndicators(player.Connection, lastData);
                yield return MEC.Timing.WaitForSeconds(MapScan_UpdateRate);
            }

            player.ReferenceHub.scp079PlayerScript.TargetSetupIndicators(player.Connection, new List<Vector3>());
        }

        private void Server_RoundStarted()
        {
            GlassPatch.Reload();
            this.RunCoroutine(this.UpdateGeneratorsTimer(), "UpdateGeneratorsTimer");
        }

        private void Player_VoiceChatting(Exiled.Events.EventArgs.VoiceChattingEventArgs ev)
        {
            if (ev.Player == null)
                return;
            if (ev.Player.Role.Type != RoleType.Scp079)
                return;
            ev.DissonanceUserSetup.MimicAs939 = false;

            if (ev.IsVoiceChatting)
                MEC.Timing.RunCoroutine(HandleNewGUI(ev.Player));
            else
                PressingAltVCKey.Remove(ev.Player);
        }

        private IEnumerator<float> UpdateGeneratorsTimer()
        {
            yield return MEC.Timing.WaitForSeconds(30);
            int rid = RoundPlus.RoundId;
            while (Round.IsStarted && rid == RoundPlus.RoundId)
            {
                string msg = string.Empty;
                if (Respawning.RespawnManager.Singleton.NextKnownTeam != Respawning.SpawnableTeamType.None)
                {
                    var seconds = Mathf.RoundToInt(Respawning.RespawnManager.Singleton._timeForNextSequence - (float)Respawning.RespawnManager.Singleton._stopwatch.Elapsed.TotalSeconds);
                    if (Respawning.RespawnManager.Singleton.NextKnownTeam == Respawning.SpawnableTeamType.NineTailedFox)
                        msg = string.Format(PluginHandler.Instance.Translation.HelicopterLanding, seconds.ToString("00"));
                    else
                        msg = string.Format(PluginHandler.Instance.Translation.CarArriving, seconds.ToString("00"));
                }
                else
                {
                    MapGeneration.Distributors.Scp079Generator nearestGenerator = null;
                    var gens = new List<MapGeneration.Distributors.Scp079Generator>();
                    int generators = 0;

                    foreach (var generator in Recontainer079.AllGenerators)
                    {
                        if (generator.Activating)
                        {
                            generators++;
                            if ((nearestGenerator?.Network_syncTime ?? float.MaxValue) > generator.Network_syncTime)
                                nearestGenerator = generator;
                            gens.Add(generator);
                        }
                    }

                    if (nearestGenerator != null)
                        msg = string.Format(PluginHandler.Instance.Translation.GeneratorTime, generators, nearestGenerator.Network_syncTime.ToString("00"), string.Join("<br>", gens.Select(i => Map.FindParentRoom(i.gameObject)?.Type.ToString() ?? "[UNKNOWN]")));
                    else if (MapPlus.IsSCP079ReadyForRecontainment)
                        msg = PluginHandler.Instance.Translation.RecontainmentReady;
                }

                foreach (var player in RealPlayers.List.Where(p => p.Role != RoleType.Scp079))
                {
                    player.SetGUI("scp079", PseudoGUIPosition.MIDDLE, null);
                    player.SetGUI("scp079_message", PseudoGUIPosition.MIDDLE, null);
                }

                foreach (var player in RealPlayers.Get(RoleType.Scp079))
                {
                    var scp = (Scp079Role)player.Role;
                    string fakeSCP = PluginHandler.Instance.Translation.Ready;
                    string fakeMTF = PluginHandler.Instance.Translation.Ready;
                    string fakeCI = PluginHandler.Instance.Translation.Ready;
                    string fakeTesla = PluginHandler.Instance.Translation.Ready;
                    string scan = PluginHandler.Instance.Translation.Ready;
                    string fullScan = PluginHandler.Instance.Translation.Ready;
                    string blackout = PluginHandler.Instance.Translation.Ready;
                    string warheadStop = PluginHandler.Instance.Translation.Ready;
                    string cassie = PluginHandler.Instance.Translation.Ready;
                    string advancedScan = PluginHandler.Instance.Translation.Ready;

                    if (FakeSCPCommand.ReqLvl > scp.Level + 1)
                        fakeSCP = string.Format(PluginHandler.Instance.Translation.RequireLevel, FakeSCPCommand.ReqLvl);
                    else if (!FakeSCPCommand.IsReady)
                        fakeSCP = string.Format(PluginHandler.Instance.Translation.RequireCooldown, Math.Round(new TimeSpan(FakeSCPCommand.TimeLeft).TotalSeconds));
                    else if (FakeSCPCommand.Cost > scp.Energy)
                        fakeSCP = string.Format(PluginHandler.Instance.Translation.RequireAP, FakeSCPCommand.Cost);

                    if (FakeMTFCommand.ReqLvl > scp.Level + 1)
                        fakeMTF = string.Format(PluginHandler.Instance.Translation.RequireLevel, FakeMTFCommand.ReqLvl);
                    else if (!FakeMTFCommand.IsReady)
                        fakeMTF = string.Format(PluginHandler.Instance.Translation.RequireCooldown, Math.Round(new TimeSpan(FakeMTFCommand.TimeLeft).TotalSeconds));
                    else if (FakeMTFCommand.Cost > scp.Energy)
                        fakeMTF = string.Format(PluginHandler.Instance.Translation.RequireAP, FakeMTFCommand.Cost);

                    if (FakeCICommand.ReqLvl > scp.Level + 1)
                        fakeCI = string.Format(PluginHandler.Instance.Translation.RequireLevel, FakeCICommand.ReqLvl);
                    else if (!FakeCICommand.IsReady)
                        fakeCI = string.Format(PluginHandler.Instance.Translation.RequireCooldown, Math.Round(new TimeSpan(FakeCICommand.TimeLeft).TotalSeconds));
                    else if (FakeCICommand.Cost > scp.Energy)
                        fakeCI = string.Format(PluginHandler.Instance.Translation.RequireAP, FakeCICommand.Cost);

                    if (ScanCommand.ReqLvl > scp.Level + 1)
                        scan = string.Format(PluginHandler.Instance.Translation.RequireLevel, ScanCommand.ReqLvl);
                    else if (!ScanCommand.IsReady)
                        scan = string.Format(PluginHandler.Instance.Translation.RequireCooldown, Math.Round(new TimeSpan(ScanCommand.TimeLeft).TotalSeconds));
                    else if (ScanCommand.Cost > scp.Energy)
                        scan = string.Format(PluginHandler.Instance.Translation.RequireAP, ScanCommand.Cost);

                    if (FullScanCommand.ReqLvl > scp.Level + 1)
                        fullScan = string.Format(PluginHandler.Instance.Translation.RequireLevel, FullScanCommand.ReqLvl);
                    else if (!FullScanCommand.IsReady)
                        fullScan = string.Format(PluginHandler.Instance.Translation.RequireCooldown, Math.Round(new TimeSpan(FullScanCommand.TimeLeft).TotalSeconds));
                    else if (FullScanCommand.Cost > scp.Energy)
                        fullScan = string.Format(PluginHandler.Instance.Translation.RequireAP, FullScanCommand.Cost);

                    if (BlackoutCommand.ReqLvl > scp.Level + 1)
                        blackout = string.Format(PluginHandler.Instance.Translation.RequireLevel, BlackoutCommand.ReqLvl);
                    else if (!BlackoutCommand.IsReady)
                        blackout = string.Format(PluginHandler.Instance.Translation.RequireCooldown, Math.Round(new TimeSpan(BlackoutCommand.TimeLeft).TotalSeconds));
                    else
                        blackout = string.Format(PluginHandler.Instance.Translation.MaxBlackout, Math.Floor(scp.Energy / BlackoutCommand.Cost));

                    if (StopWarheadCommand.ReqLvl > scp.Level + 1)
                        warheadStop = string.Format(PluginHandler.Instance.Translation.RequireLevel, StopWarheadCommand.ReqLvl);
                    else if (!StopWarheadCommand.IsReady)
                        warheadStop = string.Format(PluginHandler.Instance.Translation.RequireCooldown, Math.Round(new TimeSpan(StopWarheadCommand.TimeLeft).TotalSeconds));
                    else if (StopWarheadCommand.Cost > scp.Energy)
                        warheadStop = string.Format(PluginHandler.Instance.Translation.RequireAP, StopWarheadCommand.Cost);
                    else if (!Warhead.IsInProgress)
                        warheadStop = PluginHandler.Instance.Translation.WarheadDetonating;

                    if (CassieCommand.ReqLvl > scp.Level + 1)
                        cassie = string.Format(PluginHandler.Instance.Translation.RequireLevel, CassieCommand.ReqLvl);
                    else if (!CassieCommand.IsReady)
                        cassie = string.Format(PluginHandler.Instance.Translation.RequireCooldown, Math.Round(new TimeSpan(CassieCommand.TimeLeft).TotalSeconds));
                    else if (CassieCommand.Cost > scp.Energy)
                        cassie = string.Format(PluginHandler.Instance.Translation.RequireAP, CassieCommand.Cost);

                    if (MapScan_RequiedLvl > scp.Level + 1)
                        advancedScan = string.Format(PluginHandler.Instance.Translation.RequireLevel, MapScan_RequiedLvl);
                    else if (MapScan_CostPerStart > scp.Energy)
                        advancedScan = string.Format(PluginHandler.Instance.Translation.RequireAP, MapScan_CostPerStart);

                    if (!IsGlobalReady)
                    {
                        fakeCI = string.Format(PluginHandler.Instance.Translation.RequireCooldown, Math.Round(new TimeSpan(GlobalTimeLeft).TotalSeconds));
                        fakeMTF = string.Format(PluginHandler.Instance.Translation.RequireCooldown, Math.Round(new TimeSpan(GlobalTimeLeft).TotalSeconds));
                        fakeSCP = string.Format(PluginHandler.Instance.Translation.RequireCooldown, Math.Round(new TimeSpan(GlobalTimeLeft).TotalSeconds));
                    }

                    string sumMessage = $@"<br><br><br><br><br>
<size=50%>
<align=left>Fake SCP</align><line-height=1px><br></line-height><align=right>{fakeSCP}</align>
<align=left>Fake MTF</align><line-height=1px><br></line-height><align=right>{fakeMTF}</align>
<align=left>Fake CI</align><line-height=1px><br></line-height><align=right>{fakeCI}</align>
<align=left>Scan</align><line-height=1px><br></line-height><align=right>{scan}</align>
<align=left>FullScan</align><line-height=1px><br></line-height><align=right>{fullScan}</align>
<align=left>Blackout</align><line-height=1px><br></line-height><align=right>{blackout}</align>
<align=left>Warhead Stop</align><line-height=1px><br></line-height><align=right>{warheadStop}</align>
<align=left>Cassie</align><line-height=1px><br></line-height><align=right>{cassie}</align>
<align=left>Advanced Scan</align><line-height=1px><br></line-height><align=right>{advancedScan}</align>
</size>";
                    player.SetGUI("scp079", PseudoGUIPosition.MIDDLE, sumMessage);
                    player.SetGUI("scp079_message", PseudoGUIPosition.BOTTOM, msg);
                }

                yield return MEC.Timing.WaitForSeconds(1);
            }
        }
    }
}
