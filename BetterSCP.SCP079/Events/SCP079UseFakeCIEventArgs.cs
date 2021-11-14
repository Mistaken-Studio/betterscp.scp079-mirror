// -----------------------------------------------------------------------
// <copyright file="SCP079UseFakeCIEventArgs.cs" company="Mistaken">
// Copyright (c) Mistaken. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using Exiled.API.Features;
using Exiled.Events.Extensions;

namespace Mistaken.BetterSCP.SCP079.Events
{
    public class SCP079UseFakeCIEventArgs : SCP079UseEventArgs
    {
        internal SCP079UseFakeCIEventArgs(Player scp079, bool isAllowed = true)
            : base(scp079, isAllowed)
        {
        }
    }
}
