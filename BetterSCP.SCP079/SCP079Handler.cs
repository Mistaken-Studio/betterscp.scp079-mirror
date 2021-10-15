// -----------------------------------------------------------------------
// <copyright file="SCP079Handler.cs" company="Mistaken">
// Copyright (c) Mistaken. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using Exiled.API.Features;
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
        public static void GainXP(Player player, float ap)
        {
            player.Energy -= ap;
            var id = player.Level;
            if (id >= player.Levels.Length)
                id = (byte)(player.Levels.Length - 1);
            else if (id < 0)
                id = 0;
            float num4 = 1f / Mathf.Clamp(player.Levels[id].manaPerSecond / 1.5f, 1f, 5f);
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

            Exiled.Events.Handlers.Server.RoundStarted += this.Handle(() => this.Server_RoundStarted(), "RoundStart");
        }

        public override void OnDisable()
        {
            SCPGUIHandler.SCPMessages.Remove(RoleType.Scp079);

            Exiled.Events.Handlers.Server.RoundStarted -= this.Handle(() => this.Server_RoundStarted(), "RoundStart");
        }

        private void Server_RoundStarted()
        {
            this.RunCoroutine(this.UpdateGeneratorsTimer(), "UpdateGeneratorsTimer");
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
                    else if (Events.Handlers.CustomEvents.SCP079.IsBeingRecontained)
                        msg = PluginHandler.Instance.Translation.RecontainmentReady;
                }

                foreach (var player in RealPlayers.List.Where(p => p.Role != RoleType.Scp079))
                {
                    player.SetGUI("scp079", PseudoGUIPosition.MIDDLE, null);
                    player.SetGUI("scp079_message", PseudoGUIPosition.MIDDLE, null);
                }

                foreach (var player in RealPlayers.Get(RoleType.Scp079))
                {
                    string fakeSCP = PluginHandler.Instance.Translation.Ready;
                    string fakeMTF = PluginHandler.Instance.Translation.Ready;
                    string fakeCI = PluginHandler.Instance.Translation.Ready;
                    string fakeTesla = PluginHandler.Instance.Translation.Ready;
                    string scan = PluginHandler.Instance.Translation.Ready;
                    string fullScan = PluginHandler.Instance.Translation.Ready;
                    string blackout = PluginHandler.Instance.Translation.Ready;
                    string warheadStop = PluginHandler.Instance.Translation.Ready;
                    string cassie = PluginHandler.Instance.Translation.Ready;

                    if (FakeSCPCommand.ReqLvl > player.Level + 1)
                        fakeSCP = string.Format(PluginHandler.Instance.Translation.RequireLevel, FakeSCPCommand.ReqLvl);
                    else if (!FakeSCPCommand.IsReady)
                        fakeSCP = string.Format(PluginHandler.Instance.Translation.RequireCooldown, Math.Round(new TimeSpan(FakeSCPCommand.TimeLeft).TotalSeconds));
                    else if (FakeSCPCommand.Cost > player.Energy)
                        fakeSCP = string.Format(PluginHandler.Instance.Translation.RequireAP, FakeSCPCommand.Cost);

                    if (FakeMTFCommand.ReqLvl > player.Level + 1)
                        fakeMTF = string.Format(PluginHandler.Instance.Translation.RequireLevel, FakeMTFCommand.ReqLvl);
                    else if (!FakeMTFCommand.IsReady)
                        fakeMTF = string.Format(PluginHandler.Instance.Translation.RequireCooldown, Math.Round(new TimeSpan(FakeMTFCommand.TimeLeft).TotalSeconds));
                    else if (FakeMTFCommand.Cost > player.Energy)
                        fakeMTF = string.Format(PluginHandler.Instance.Translation.RequireAP, FakeMTFCommand.Cost);

                    if (FakeCICommand.ReqLvl > player.Level + 1)
                        fakeCI = string.Format(PluginHandler.Instance.Translation.RequireLevel, FakeCICommand.ReqLvl);
                    else if (!FakeCICommand.IsReady)
                        fakeCI = string.Format(PluginHandler.Instance.Translation.RequireCooldown, Math.Round(new TimeSpan(FakeCICommand.TimeLeft).TotalSeconds));
                    else if (FakeCICommand.Cost > player.Energy)
                        fakeCI = string.Format(PluginHandler.Instance.Translation.RequireAP, FakeCICommand.Cost);

                    if (FakeTeslaCommand.ReqLvl > player.Level + 1)
                        fakeTesla = string.Format(PluginHandler.Instance.Translation.RequireLevel, FakeTeslaCommand.ReqLvl);
                    else if (!FakeTeslaCommand.IsReady)
                        fakeTesla = string.Format(PluginHandler.Instance.Translation.RequireCooldown, Math.Round(new TimeSpan(FakeTeslaCommand.TimeLeft).TotalSeconds));
                    else if (FakeTeslaCommand.Cost > player.Energy)
                        fakeTesla = string.Format(PluginHandler.Instance.Translation.RequireAP, FakeTeslaCommand.Cost);

                    if (ScanCommand.ReqLvl > player.Level + 1)
                        scan = string.Format(PluginHandler.Instance.Translation.RequireLevel, ScanCommand.ReqLvl);
                    else if (!ScanCommand.IsReady)
                        scan = string.Format(PluginHandler.Instance.Translation.RequireCooldown, Math.Round(new TimeSpan(ScanCommand.TimeLeft).TotalSeconds));
                    else if (ScanCommand.Cost > player.Energy)
                        scan = string.Format(PluginHandler.Instance.Translation.RequireAP, ScanCommand.Cost);

                    if (FullScanCommand.ReqLvl > player.Level + 1)
                        fullScan = string.Format(PluginHandler.Instance.Translation.RequireLevel, FullScanCommand.ReqLvl);
                    else if (!FullScanCommand.IsReady)
                        fullScan = string.Format(PluginHandler.Instance.Translation.RequireCooldown, Math.Round(new TimeSpan(FullScanCommand.TimeLeft).TotalSeconds));
                    else if (FullScanCommand.Cost > player.Energy)
                        fullScan = string.Format(PluginHandler.Instance.Translation.RequireAP, FullScanCommand.Cost);

                    if (BlackoutCommand.ReqLvl > player.Level + 1)
                        blackout = string.Format(PluginHandler.Instance.Translation.RequireLevel, BlackoutCommand.ReqLvl);
                    else if (!BlackoutCommand.IsReady)
                        blackout = string.Format(PluginHandler.Instance.Translation.RequireCooldown, Math.Round(new TimeSpan(BlackoutCommand.TimeLeft).TotalSeconds));
                    else
                        blackout = string.Format(PluginHandler.Instance.Translation.MaxBlackout, Math.Floor(player.Energy / BlackoutCommand.Cost));

                    if (StopWarheadCommand.ReqLvl > player.Level + 1)
                        warheadStop = string.Format(PluginHandler.Instance.Translation.RequireLevel, StopWarheadCommand.ReqLvl);
                    else if (!StopWarheadCommand.IsReady)
                        warheadStop = string.Format(PluginHandler.Instance.Translation.RequireCooldown, Math.Round(new TimeSpan(StopWarheadCommand.TimeLeft).TotalSeconds));
                    else if (StopWarheadCommand.Cost > player.Energy)
                        warheadStop = string.Format(PluginHandler.Instance.Translation.RequireAP, StopWarheadCommand.Cost);
                    else if (!Warhead.IsInProgress)
                        warheadStop = PluginHandler.Instance.Translation.WarheadDetonating;

                    if (CassieCommand.ReqLvl > player.Level + 1)
                        cassie = string.Format(PluginHandler.Instance.Translation.RequireLevel, CassieCommand.ReqLvl);
                    else if (!CassieCommand.IsReady)
                        cassie = string.Format(PluginHandler.Instance.Translation.RequireCooldown, Math.Round(new TimeSpan(CassieCommand.TimeLeft).TotalSeconds));
                    else if (CassieCommand.Cost > player.Energy)
                        cassie = string.Format(PluginHandler.Instance.Translation.RequireAP, CassieCommand.Cost);

                    string sumMessage = $@"
<size=50%>
<align=left>Fake SCP</align><line-height=1px><br></line-height><align=right>{fakeSCP}</align>
<align=left>Fake MTF</align><line-height=1px><br></line-height><align=right>{fakeMTF}</align>
<align=left>Fake CI</align><line-height=1px><br></line-height><align=right>{fakeCI}</align>
<align=left>Fake Tesla</align><line-height=1px><br></line-height><align=right>{fakeTesla}</align>
<align=left>Scan</align><line-height=1px><br></line-height><align=right>{scan}</align>
<align=left>FullScan</align><line-height=1px><br></line-height><align=right>{fullScan}</align>
<align=left>Blackout</align><line-height=1px><br></line-height><align=right>{blackout}</align>
<align=left>Warhead Stop</align><line-height=1px><br></line-height><align=right>{warheadStop}</align>
<align=left>Cassie</align><line-height=1px><br></line-height><align=right>{cassie}</align>
</size>";
                    player.SetGUI("scp079", PseudoGUIPosition.MIDDLE, sumMessage);
                    player.SetGUI("scp079_message", PseudoGUIPosition.BOTTOM, msg);
                }

                yield return MEC.Timing.WaitForSeconds(1);
            }
        }
    }
}
