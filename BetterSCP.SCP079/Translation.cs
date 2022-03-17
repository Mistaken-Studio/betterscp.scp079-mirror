// -----------------------------------------------------------------------
// <copyright file="Translation.cs" company="Mistaken">
// Copyright (c) Mistaken. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using Exiled.API.Interfaces;

namespace Mistaken.BetterSCP.SCP079
{
    /// <inheritdoc/>
    internal class Translation : ITranslation
    {
        public string Success { get; set; } = "Done";

        public string FailedLvl { get; set; } = "You must have at least ${lvl} level";

        public string FailedAP { get; set; } = "You must have at least ${ap} AP";

        public string FailedGlobalCooldown { get; set; } = "You have to wait ${time} seconds before sending another command because there's cooldown";

        public string FailedCooldown { get; set; } = "You have to wait ${time} seconds because there's cooldown";

        public string FailedCooldownBlackout { get; set; } = "You have to wait ${time} seconds because there's cooldown, ${leftS} left";

        public string FailedNoNumber { get; set; } = "Input SCP number. Max ${max}";

        public string FailedNoNumberBlackout { get; set; } = "Input number of seconds blackout will last";

        public string FailedWrongNumber { get; set; } = "Input a SCP <b>NUMBER</b. Max ${max}";

        public string Scan { get; set; } = "Scan completed:|Entrance Zone:{ez}|Heavy Containment Zone:{hcz}|Light Containment Zone:{lcz}|Alpha Warhead silo:{nuke}|Containment chamber of SCP 049:{049}|Surface:{surface}|Pocket:{pocket}";

        public string StartMessage { get; set; } = "<color=red><b><size=500%>UWAGA</size></b></color><br><br><size=90%>Rozgrywka jako <color=red>SCP 079</color> na tym serwerze jest lekko zmodyfikowana, <color=red>SCP 079</color> posiada dodatkowe możliwość:<size=75%><br><b><color=yellow>.scan</color></b> - Pokazuje w jakiej strefie znajdują się gracze(nie licząc martwych)<br>- wymaga: <color=yellow>2</color> poziomu oraz <color=yellow>100</color> AP<br><b><color=yellow>.fullscan</color></b> - Działa jak <color=yellow>.scan</color> ale wynik podaje dodatkowo jako wiadomość CASSIE<br>- wymaga: <color=yellow>3</color> poziomu oraz <color=yellow>100</color> AP<br><b><color=yellow>.fakemtf</color></b> - Wysyła fałszywą wiadomość o przyjeździe <color=blue>MFO</color><br>- wymaga: <color=yellow>3</color> poziomu oraz <color=yellow>100</color> AP<br><b><color=yellow>.fakescp [numer SCP]</color></b> - Wysyła fałszywą wiadomość o śmierci podanego <color=red>SCP</color>, przyczyna jest losowa<br>- wymaga: <color=yellow>3</color> poziomu oraz <color=yellow>100</color> AP<br><b><color=yellow>.blackout [długość w sekundach]</color></b> - Gasi światła w placówce<br>- wymaga: <color=yellow>2</color> poziomu oraz <color=yellow>ilość sekund razy 10</color> AP</size></size>";

        public string RequireLevel { get; set; } = "<color=red>Require <color=yellow>{0}</color> lvl</color>";

        public string RequireAP { get; set; } = "<color=red>Require <color=yellow>{0}</color> AP</color>";

        public string RequireCooldown { get; set; } = "<color=red>Require <color=yellow>{0}</color>s</color>";

        public string MaxBlackout { get; set; } = "Max <color=yellow>{0}</color> seconds of blackout";

        public string WarheadDetonating { get; set; } = "<color=red>Warhead is not detonating</color>";

        public string Ready { get; set; } = "<color=yellow>READY</color>";

        public string RecontainmentReady { get; set; } = "Recontainment is <color=yellow>ready</color>";

        public string GeneratorTime { get; set; } = "<color=yellow>{0}</color> generator being activated<br>Time left: <color=yellow>{1}</color>s<br><size=50%><color=yellow>{2}</color></size>";

        public string HelicopterLanding { get; set; } = "<color=#00008f><b>Helicopter is landing</b></color> in {0}s";

        public string CarArriving { get; set; } = "<color=#008f00><b>Car is arriving</b></color> in {0}s";

        public string SCP106Opened { get; set; } = "<color=#red><b>Warning! SCP-106 Chamber has been opened!</b></color>";
    }
}
