// -----------------------------------------------------------------------
// <copyright file="GlassPatch.cs" company="Mistaken">
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
    [HarmonyPatch(typeof(BreakableWindow), nameof(BreakableWindow.Damage))]
    internal static class GlassPatch
    {
        internal static void Reload()
        {
            window = GameObject.FindObjectOfType<Recontainer079>()._activatorGlass;
        }

        private static BreakableWindow window;

        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Shared.Rent(instructions);
            Label continueLabel = generator.DefineLabel();
            Label label = generator.DefineLabel();

            newInstructions[0].WithLabels(continueLabel);
            newInstructions.InsertRange(
                0,
                new CodeInstruction[]
                {
                    new CodeInstruction(OpCodes.Ldarg_0),
                    new CodeInstruction(OpCodes.Ldsfld, AccessTools.Field(typeof(GlassPatch), nameof(GlassPatch.window))),
                    new CodeInstruction(OpCodes.Beq_S, label),
                    new CodeInstruction(OpCodes.Br_S, continueLabel),
                    new CodeInstruction(OpCodes.Nop).WithLabels(label),

                    new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(GlassPatch), nameof(GlassPatch.ActivatedGenerators))),
                    new CodeInstruction(OpCodes.Ldc_I4_3),
                    new CodeInstruction(OpCodes.Beq_S, continueLabel),
                    new CodeInstruction(OpCodes.Ret),
                });

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Shared.Return(newInstructions);
            yield break;
        }

        private static int ActivatedGenerators()
            => Generator.List.Count(x => x.IsEngaged);
    }
}
