// -----------------------------------------------------------------------
// <copyright file="SCP079UseFakeSCPEventArgs.cs" company="Mistaken">
// Copyright (c) Mistaken. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using Exiled.API.Features;

using static Mistaken.BetterSCP.SCP079.Commands.FakeSCPCommand;

namespace Mistaken.BetterSCP.SCP079.Events
{
    public sealed class SCP079UseFakeSCPEventArgs : SCP079UseEventArgs
    {
        /// <summary>
        /// Gets sCP Number.
        /// </summary>
        public string SCP { get; }

        /// <summary>
        /// Gets cause of death.
        /// </summary>
        public FakeSCPDeathCause Cause { get; }

        internal SCP079UseFakeSCPEventArgs(Player scp079, string scp, FakeSCPDeathCause cause, bool isAllowed = true)
            : base(scp079, isAllowed)
        {
            this.SCP = scp;
            this.Cause = cause;
        }
    }
}
