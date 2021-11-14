// -----------------------------------------------------------------------
// <copyright file="SCP079UseCassieEventArgs.cs" company="Mistaken">
// Copyright (c) Mistaken. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using Exiled.API.Features;
using Exiled.Events.Extensions;

namespace Mistaken.BetterSCP.SCP079.Events
{
    public class SCP079UseCassieEventArgs : SCP079UseEventArgs
    {
        internal SCP079UseCassieEventArgs(Player scp079, string message, bool isAllowed = true)
            : base(scp079, isAllowed)
        {
            this.Message = message;
        }

        public string Message { get; }
    }
}
