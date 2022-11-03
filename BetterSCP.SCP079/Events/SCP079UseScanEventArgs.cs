// -----------------------------------------------------------------------
// <copyright file="SCP079UseScanEventArgs.cs" company="Mistaken">
// Copyright (c) Mistaken. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using Exiled.API.Features;

namespace Mistaken.BetterSCP.SCP079.Events
{
    public sealed class SCP079UseScanEventArgs : SCP079UseEventArgs
    {
        public bool IsFullScan { get; }

        internal SCP079UseScanEventArgs(Player scp079, bool isFullScan, bool isAllowed = true)
            : base(scp079, isAllowed)
        {
            this.IsFullScan = isFullScan;
        }
    }
}
