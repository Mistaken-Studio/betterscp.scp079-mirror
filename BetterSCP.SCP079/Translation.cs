// -----------------------------------------------------------------------
// <copyright file="Translation.cs" company="Mistaken">
// Copyright (c) Mistaken. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.API.Interfaces;

namespace Mistaken.BetterSCP.SCP079
{
    /// <inheritdoc/>
    public class Translation : ITranslation
    {
#pragma warning disable CS1591 // Brak komentarza XML dla widocznego publicznie typu lub składowej
        public string Success { get; set; } = "Done";

        public string FailedLvl { get; set; } = "You must have at least ${lvl} level";

        public string FailedAP { get; set; } = "You must have at least ${ap} AP";

        public string FailedCooldown { get; set; } = "You have to wait ${time} seconds because there's cooldown";

        public string FailedCooldownBlackout { get; set; } = "You have to wait ${time} seconds because there's cooldown, ${leftS} left";

        public string FailedNoNumber { get; set; } = "Input SCP number. Max ${max}";

        public string FailedNoNumberBlackout { get; set; } = "Input number of seconds blackout will last";

        public string FailedWrongNumber { get; set; } = "Input a SCP <b>NUMBER</b. Max ${max}";

        public string Scan { get; set; } = "Scan completed:|Entrance Zone:{ez}|Heavy Containment Zone:{hcz}|Light Containment Zone:{lcz}|Alpha Warhead silo:{nuke}|Containment chamber of SCP 049:{049}|Surface:{surface}|Pocket:{pocket}";

        public string StartMessage { get; set; } = "<color=red><b><size=500%>UWAGA</size></b></color><br><br><size=90%>Rozgrywka jako <color=red>SCP 079</color> na tym serwerze jest lekko zmodyfikowana, <color=red>SCP 079</color> posiada dodatkowe możliwość:<size=75%><br><b><color=yellow>.scan</color></b> - Pokazuje w jakiej strefie znajdują się gracze(nie licząc martwych)<br>- wymaga: <color=yellow>2</color> poziomu oraz <color=yellow>100</color> AP<br><b><color=yellow>.fullscan</color></b> - Działa jak <color=yellow>.scan</color> ale wynik podaje dodatkowo jako wiadomość CASSIE<br>- wymaga: <color=yellow>3</color> poziomu oraz <color=yellow>100</color> AP<br><b><color=yellow>.fakemtf</color></b> - Wysyła fałszywą wiadomość o przyjeździe <color=blue>MFO</color><br>- wymaga: <color=yellow>3</color> poziomu oraz <color=yellow>100</color> AP<br><b><color=yellow>.fakescp [numer SCP]</color></b> - Wysyła fałszywą wiadomość o śmierci podanego <color=red>SCP</color>, przyczyna jest losowa<br>- wymaga: <color=yellow>3</color> poziomu oraz <color=yellow>100</color> AP<br><b><color=yellow>.blackout [długość w sekundach]</color></b> - Gasi światła w placówce<br>- wymaga: <color=yellow>2</color> poziomu oraz <color=yellow>ilość sekund razy 10</color> AP</size></size>";
#pragma warning restore CS1591 // Brak komentarza XML dla widocznego publicznie typu lub składowej
    }
}
