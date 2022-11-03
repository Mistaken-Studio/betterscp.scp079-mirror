// -----------------------------------------------------------------------
// <copyright file="SCP079UseFakeTeslaEventArgs.cs" company="Mistaken">
// Copyright (c) Mistaken. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using Exiled.API.Features;

namespace Mistaken.BetterSCP.SCP079.Events
{
    public sealed class SCP079UseFakeTeslaEventArgs : SCP079UseEventArgs
    {
        internal SCP079UseFakeTeslaEventArgs(Player scp079, bool isAllowed = true)
            : base(scp079, isAllowed)
        {
        }
    }
}
