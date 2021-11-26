// -----------------------------------------------------------------------
// <copyright file="SCP079UseFakeTeslaEventArgs.cs" company="Mistaken">
// Copyright (c) Mistaken. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using Exiled.API.Features;
using Exiled.Events.Extensions;

namespace Mistaken.BetterSCP.SCP079.Events
{
    public class SCP079UseFakeTeslaEventArgs : SCP079UseEventArgs
    {
        internal SCP079UseFakeTeslaEventArgs(Player scp079, bool isAllowed = true)
            : base(scp079, isAllowed)
        {
        }
    }
}
