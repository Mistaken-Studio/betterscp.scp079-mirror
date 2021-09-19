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
                        msg = $"<color=#00008f><b>Helicopter is landing</b></color> in {seconds:00}s";
                    else
                        msg = $"<color=#008f00><b>Car is arriving</b></color> in {seconds:00}s";
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
                    {
                        var seconds = nearestGenerator.Network_syncTime;
                        msg = $"<color=yellow>{generators}</color> generator{(generators > 1 ? "s are" : " is")} being activated<br>Time left: <color=yellow>{seconds:00}</color>s<br><size=50%><color=yellow>{string.Join("<br>", gens.Select(i => "[UNKNOWN]"))}</color></size>";
                    }
                    else if (Events.Handlers.CustomEvents.SCP079.IsBeingRecontained)
                        msg = $"Recontainment is <color=yellow>ready</color>";
                }

                foreach (var player in RealPlayers.List.Where(p => p.Role != RoleType.Scp079))
                {
                    player.SetGUI("scp079", PseudoGUIPosition.MIDDLE, null);
                    player.SetGUI("scp079_message", PseudoGUIPosition.MIDDLE, null);
                }

                foreach (var player in RealPlayers.Get(RoleType.Scp079))
                {
                    string fakeSCP = $"<color=yellow>READY</color>";
                    string fakeMTF = $"<color=yellow>READY</color>";
                    string fakeCI = $"<color=yellow>READY</color>";
                    string fakeTesla = $"<color=yellow>READY</color>";
                    string scan = $"<color=yellow>READY</color>";
                    string fullScan = $"<color=yellow>READY</color>";
                    string blackout = $"<color=yellow>READY</color>";
                    string warheadStop = $"<color=yellow>READY</color>";
                    string cassie = $"<color=yellow>READY</color>";

                    if (FakeSCPCommand.ReqLvl > player.Level + 1)
                        fakeSCP = $"<color=red>Require <color=yellow>{FakeSCPCommand.ReqLvl}</color> lvl</color>";
                    else if (!FakeSCPCommand.IsReady)
                        fakeSCP = $"<color=red>Require <color=yellow>{Math.Round(new TimeSpan(FakeSCPCommand.TimeLeft).TotalSeconds)}</color>s</color>";
                    else if (FakeSCPCommand.Cost > player.Energy)
                        fakeSCP = $"<color=red>Require <color=yellow>{FakeSCPCommand.Cost}</color> AP</color>";

                    if (FakeMTFCommand.ReqLvl > player.Level + 1)
                        fakeMTF = $"<color=red>Require <color=yellow>{FakeMTFCommand.ReqLvl}</color> lvl</color>";
                    else if (!FakeMTFCommand.IsReady)
                        fakeMTF = $"<color=red>Require <color=yellow>{Math.Round(new TimeSpan(FakeMTFCommand.TimeLeft).TotalSeconds)}</color>s</color>";
                    else if (FakeMTFCommand.Cost > player.Energy)
                        fakeMTF = $"<color=red>Require <color=yellow>{FakeMTFCommand.Cost}</color> AP</color>";

                    if (FakeCICommand.ReqLvl > player.Level + 1)
                        fakeCI = $"<color=red>Require <color=yellow>{FakeCICommand.ReqLvl}</color> lvl</color>";
                    else if (!FakeCICommand.IsReady)
                        fakeCI = $"<color=red>Require <color=yellow>{Math.Round(new TimeSpan(FakeCICommand.TimeLeft).TotalSeconds)}</color>s</color>";
                    else if (FakeCICommand.Cost > player.Energy)
                        fakeCI = $"<color=red>Require <color=yellow>{FakeCICommand.Cost}</color> AP</color>";

                    if (FakeTeslaCommand.ReqLvl > player.Level + 1)
                        fakeTesla = $"<color=red>Require <color=yellow>{FakeTeslaCommand.ReqLvl}</color> lvl</color>";
                    else if (!FakeTeslaCommand.IsReady)
                        fakeTesla = $"<color=red>Require <color=yellow>{Math.Round(new TimeSpan(FakeTeslaCommand.TimeLeft).TotalSeconds)}</color>s</color>";
                    else if (FakeTeslaCommand.Cost > player.Energy)
                        fakeTesla = $"<color=red>Require <color=yellow>{FakeTeslaCommand.Cost}</color> AP</color>";

                    if (ScanCommand.ReqLvl > player.Level + 1)
                        scan = $"<color=red>Require <color=yellow>{ScanCommand.ReqLvl}</color> lvl</color>";
                    else if (!ScanCommand.IsReady)
                        scan = $"<color=red>Require <color=yellow>{Math.Round(new TimeSpan(ScanCommand.TimeLeft).TotalSeconds)}</color>s</color>";
                    else if (ScanCommand.Cost > player.Energy)
                        scan = $"<color=red>Require <color=yellow>{ScanCommand.Cost}</color> AP</color>";

                    if (FullScanCommand.ReqLvl > player.Level + 1)
                        fullScan = $"<color=red>Require <color=yellow>{FullScanCommand.ReqLvl}</color> lvl</color>";
                    else if (!FullScanCommand.IsReady)
                        fullScan = $"<color=red>Require <color=yellow>{Math.Round(new TimeSpan(FullScanCommand.TimeLeft).TotalSeconds)}</color>s</color>";
                    else if (FullScanCommand.Cost > player.Energy)
                        fullScan = $"<color=red>Require <color=yellow>{FullScanCommand.Cost}</color> AP</color>";

                    if (BlackoutCommand.ReqLvl > player.Level + 1)
                        blackout = $"<color=red>Require <color=yellow>{BlackoutCommand.ReqLvl}</color> lvl</color>";
                    else if (!BlackoutCommand.IsReady)
                        blackout = $"<color=red>Require <color=yellow>{Math.Round(new TimeSpan(BlackoutCommand.TimeLeft).TotalSeconds)}</color>s</color>";
                    else
                        blackout = $"Max <color=yellow>{Math.Floor(player.Energy / BlackoutCommand.Cost)}</color> seconds of blackout";

                    if (StopWarheadCommand.ReqLvl > player.Level + 1)
                        warheadStop = $"<color=red>Require <color=yellow>{StopWarheadCommand.ReqLvl}</color> lvl</color>";
                    else if (!StopWarheadCommand.IsReady)
                        warheadStop = $"<color=red>Require <color=yellow>{Math.Round(new TimeSpan(StopWarheadCommand.TimeLeft).TotalSeconds)}</color>s</color>";
                    else if (StopWarheadCommand.Cost > player.Energy)
                        warheadStop = $"<color=red>Require <color=yellow>{StopWarheadCommand.Cost}</color> AP</color>";
                    else if (!Warhead.IsInProgress)
                        warheadStop = "<color=red>Warhead is not detonating</color>";

                    if (CassieCommand.ReqLvl > player.Level + 1)
                        cassie = $"<color=red>Require <color=yellow>{CassieCommand.ReqLvl}</color> lvl</color>";
                    else if (!CassieCommand.IsReady)
                        cassie = $"<color=red>Require <color=yellow>{Math.Round(new TimeSpan(CassieCommand.TimeLeft).TotalSeconds)}</color>s</color>";
                    else if (CassieCommand.Cost > player.Energy)
                        cassie = $"<color=red>Require <color=yellow>{CassieCommand.Cost}</color> AP</color>";

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
