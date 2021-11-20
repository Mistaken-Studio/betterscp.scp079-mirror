// -----------------------------------------------------------------------
// <copyright file="SCP079UseEventArgs.cs" company="Mistaken">
// Copyright (c) Mistaken. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using Exiled.API.Features;
using Exiled.Events.Extensions;

namespace Mistaken.BetterSCP.SCP079.Events
{
    public class SCP079UseEventArgs : EventArgs
    {
        public Player SCP079 { get; }

        public bool IsAllowed { get; }

        internal SCP079UseEventArgs(Player scp079, bool isAllowed = true)
        {
            this.SCP079 = scp079;
            this.IsAllowed = isAllowed;
        }
    }
}
