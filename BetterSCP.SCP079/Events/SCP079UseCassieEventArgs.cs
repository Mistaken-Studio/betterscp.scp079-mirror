// -----------------------------------------------------------------------
// <copyright file="SCP079UseCassieEventArgs.cs" company="Mistaken">
// Copyright (c) Mistaken. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using Exiled.API.Features;

namespace Mistaken.BetterSCP.SCP079.Events
{
    public sealed class SCP079UseCassieEventArgs : SCP079UseEventArgs
    {
        public string Message { get; }

        internal SCP079UseCassieEventArgs(Player scp079, string message, bool isAllowed = true)
            : base(scp079, isAllowed)
        {
            this.Message = message;
        }
    }
}
