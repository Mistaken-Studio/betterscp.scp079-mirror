// -----------------------------------------------------------------------
// <copyright file="EventHandler.cs" company="Mistaken">
// Copyright (c) Mistaken. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using Exiled.Events.Extensions;

namespace Mistaken.BetterSCP.SCP079.Events
{
    /// <summary>
    /// Class containing all SCP079 events.
    /// </summary>
    public static class EventHandler
    {
        public static event Exiled.Events.Events.CustomEventHandler<SCP079UseFakeSCPEventArgs> UseFakeSCP;

        public static event Exiled.Events.Events.CustomEventHandler<SCP079UseFakeMTFEventArgs> UseFakeMTF;

        public static event Exiled.Events.Events.CustomEventHandler<SCP079UseFakeCIEventArgs> UseFakeCI;

        public static event Exiled.Events.Events.CustomEventHandler<SCP079UseFakeTeslaEventArgs> UseFakeTesla;

        public static event Exiled.Events.Events.CustomEventHandler<SCP079UseStopEventArgs> UseWarheadStop;

        public static event Exiled.Events.Events.CustomEventHandler<SCP079UseCassieEventArgs> UseCassie;

        public static event Exiled.Events.Events.CustomEventHandler<SCP079UseScanEventArgs> UseScan;

        public static event Exiled.Events.Events.CustomEventHandler<SCP079UseBlackoutEventArgs> UseBlackout;

        internal static void OnUseFakeSCP(SCP079UseFakeSCPEventArgs ev)
        {
            UseFakeSCP.InvokeSafely(ev);
        }

        internal static void OnUseFakeMTF(SCP079UseFakeMTFEventArgs ev)
        {
            UseFakeMTF.InvokeSafely(ev);
        }

        internal static void OnUseFakeCI(SCP079UseFakeCIEventArgs ev)
        {
            UseFakeCI.InvokeSafely(ev);
        }

        internal static void OnUseFakeTesla(SCP079UseFakeTeslaEventArgs ev)
        {
            UseFakeTesla.InvokeSafely(ev);
        }

        internal static void OnUseWarheadStop(SCP079UseStopEventArgs ev)
        {
            UseWarheadStop.InvokeSafely(ev);
        }

        internal static void OnUseCassie(SCP079UseCassieEventArgs ev)
        {
            UseCassie.InvokeSafely(ev);
        }

        internal static void OnUseScan(SCP079UseScanEventArgs ev)
        {
            UseScan.InvokeSafely(ev);
        }

        internal static void OnUseBlackout(SCP079UseBlackoutEventArgs ev)
        {
            UseBlackout.InvokeSafely(ev);
        }
    }
}
