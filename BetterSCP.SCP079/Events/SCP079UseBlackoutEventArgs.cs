// -----------------------------------------------------------------------
// <copyright file="SCP079UseBlackoutEventArgs.cs" company="Mistaken">
// Copyright (c) Mistaken. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using Exiled.API.Features;
using Exiled.Events.Extensions;

namespace Mistaken.BetterSCP.SCP079.Events
{
    public class SCP079UseBlackoutEventArgs : SCP079UseEventArgs
    {
        public float Length { get; }

        internal SCP079UseBlackoutEventArgs(Player scp079, float length, bool isAllowed = true)
            : base(scp079, isAllowed)
        {
            this.Length = length;
        }
    }
}
