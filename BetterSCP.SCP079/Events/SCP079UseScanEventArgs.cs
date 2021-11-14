// -----------------------------------------------------------------------
// <copyright file="SCP079UseScanEventArgs.cs" company="Mistaken">
// Copyright (c) Mistaken. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using Exiled.API.Features;
using Exiled.Events.Extensions;

namespace Mistaken.BetterSCP.SCP079.Events
{
    public class SCP079UseScanEventArgs : SCP079UseEventArgs
    {
        internal SCP079UseScanEventArgs(Player scp079, bool isFullScan, bool isAllowed = true)
            : base(scp079, isAllowed)
        {
            this.IsFullScan = isFullScan;
        }

        public bool IsFullScan { get; }
    }
}
