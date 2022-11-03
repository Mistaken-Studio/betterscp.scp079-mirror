// -----------------------------------------------------------------------
// <copyright file="Config.cs" company="Mistaken">
// Copyright (c) Mistaken. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.ComponentModel;
using Exiled.API.Interfaces;

namespace Mistaken.BetterSCP.SCP079
{
    internal sealed class Config : IConfig
    {
        public bool IsEnabled { get; set; } = true;

        [Description("If true then debug will be displayed")]
        public bool VerboseOutput { get; set; }

        public int RequiedLvl { get; set; } = 3;

        public int ApCost { get; set; } = 100;

        public int Cooldown { get; set; } = 180;

        public int RequiedLvlScan { get; set; } = 2;

        public int ApCostScan { get; set; } = 30;

        public int CooldownScan { get; set; } = 60;

        public int RequiedLvlBlackout { get; set; } = 2;

        public int ApCostBlackout { get; set; } = 10;

        public int CooldownBlackout { get; set; } = 5;

        public int RequiedLvlStopWarhead { get; set; } = 5;

        public int ApCostStopWarhead { get; set; } = 200;

        public int CooldownStopWarhead { get; set; } = 600;

        public int RequiedLvlCassie { get; set; } = 5;

        public int ApCostCassie { get; set; } = 200;

        public int CooldownCassie { get; set; } = 300;

        public int RequiedLvlAdvancedScan { get; set; } = 2;

        public float ApStartCostAdvancedScan { get; set; } = 30f;

        public float ApCostAdvancedScan { get; set; } = 1.5f;

        public float AdvancedScanDisableDelay { get; set; } = 2;

        public float AdvancedScanUpdateRate { get; set; } = 0.1f;

        public int GlobalCooldown { get; set; } = 10;
    }
}
