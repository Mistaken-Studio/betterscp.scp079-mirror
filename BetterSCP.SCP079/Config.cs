// -----------------------------------------------------------------------
// <copyright file="Config.cs" company="Mistaken">
// Copyright (c) Mistaken. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.ComponentModel;
using Mistaken.Updater.Config;

namespace Mistaken.BetterSCP.SCP079
{
    /// <inheritdoc/>
    internal class Config : IAutoUpdatableConfig
    {
        /// <inheritdoc/>
        public bool IsEnabled { get; set; } = true;

        /// <summary>
        /// Gets or sets a value indicating whether debug should be displayed.
        /// </summary>
        [Description("If true then debug will be displayed")]
        public bool VerbouseOutput { get; set; }

        public int RequiedLvl { get; set; } = 3;

        public int ApCost { get; set; } = 100;

        public int Cooldown { get; set; } = 180;

        public int RequiedLvlScan { get; set; } = 2;

        public int ApCostScan { get; set; } = 100;

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

        /// <inheritdoc/>
        [Description("Auto Update Settings")]
        public System.Collections.Generic.Dictionary<string, string> AutoUpdateConfig { get; set; } = new System.Collections.Generic.Dictionary<string, string>
        {
            { "Url", "https://git.mistaken.pl/api/v4/projects/45" },
            { "Token", string.Empty },
            { "Type", "GITLAB" },
            { "VerbouseOutput", "false" },
        };
    }
}
