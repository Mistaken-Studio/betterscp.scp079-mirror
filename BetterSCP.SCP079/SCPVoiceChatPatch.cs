// -----------------------------------------------------------------------
// <copyright file="SCPVoiceChatPatch.cs" company="Mistaken">
// Copyright (c) Mistaken. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using Assets._Scripts.Dissonance;
using Exiled.API.Features;
using HarmonyLib;
using MEC;
using Mistaken.API;
using NorthwoodLib.Pools;
using UnityEngine;

#pragma warning disable SA1118 // Parameter should not span multiple lines

namespace Mistaken.BetterSCP.SCP079
{
    [HarmonyPatch(typeof(Radio), nameof(Radio.UserCode_CmdSyncTransmissionStatus))]
    internal static class SCPVoiceChatPatch
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Shared.Rent(instructions);
            Label continueLabel = generator.DefineLabel();
            Label endLabel = generator.DefineLabel();

            LocalBuilder player = generator.DeclareLocal(typeof(Player));

            int startIndex = newInstructions.Count - 1;

            newInstructions.InsertRange(
                startIndex,
                new CodeInstruction[]
                {
                    // var player = Player.Get(this._hub);
                    new CodeInstruction(OpCodes.Ldarg_0).MoveBlocksFrom(newInstructions[startIndex]), // [Radio]
                    new CodeInstruction(OpCodes.Ldfld, AccessTools.Field(typeof(Radio), nameof(Radio._hub))), // [ReferenceHub]
                    new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(Player), nameof(Player.Get), new System.Type[] { typeof(ReferenceHub) })), // [Player]
                    new CodeInstruction(OpCodes.Stloc, player), // []

                    // if (player == null) return;
                    new CodeInstruction(OpCodes.Ldloc, player), // [Player]
                    new CodeInstruction(OpCodes.Brfalse_S, continueLabel), // []

                    // if (player.Role != RoleType.SCP079) return
                    new CodeInstruction(OpCodes.Ldloc, player), // [Player]
                    new CodeInstruction(OpCodes.Callvirt, AccessTools.PropertyGetter(typeof(Player), nameof(Player.Role))), // [RoleType]
                    new CodeInstruction(OpCodes.Conv_I1), // [sbyte]
                    new CodeInstruction(OpCodes.Ldc_I4_7), // [int, sbyte]
                    new CodeInstruction(OpCodes.Conv_I1), // [sbyte, sbyte]
                    new CodeInstruction(OpCodes.Beq_S, continueLabel), // []
                    new CodeInstruction(OpCodes.Br_S, endLabel), // []
                    new CodeInstruction(OpCodes.Nop).WithLabels(continueLabel), // []

                    new CodeInstruction(OpCodes.Ldloc, player), // [Player]
                    new CodeInstruction(OpCodes.Ldarg_1, player), // [bool, Player]
                    new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(SCP079Handler), nameof(SCP079Handler.HandleMapScan))), // []
                    new CodeInstruction(OpCodes.Nop).WithLabels(endLabel),
                });

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Shared.Return(newInstructions);
            yield break;
        }
    }
}
