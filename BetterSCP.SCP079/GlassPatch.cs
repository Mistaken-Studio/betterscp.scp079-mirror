// -----------------------------------------------------------------------
// <copyright file="GlassPatch.cs" company="Mistaken">
// Copyright (c) Mistaken. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using Exiled.API.Features;
using HarmonyLib;
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
            _window = Object.FindObjectOfType<Recontainer079>()._activatorGlass;
        }

        private static BreakableWindow _window;

        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Shared.Rent(instructions);
            Label continueLabel = generator.DefineLabel();
            Label label = generator.DefineLabel();

            newInstructions[0].WithLabels(continueLabel);

            newInstructions.InsertRange(0, new CodeInstruction[]
            {
                new CodeInstruction(OpCodes.Ldarg_0),
                new CodeInstruction(OpCodes.Ldsfld, AccessTools.Field(typeof(GlassPatch), nameof(_window))),
                new CodeInstruction(OpCodes.Beq_S, label),
                new CodeInstruction(OpCodes.Br_S, continueLabel),
                new CodeInstruction(OpCodes.Nop).WithLabels(label),

                new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(GlassPatch), nameof(GlassPatch.ActiveGenerators))),
                new CodeInstruction(OpCodes.Ldc_I4_3),
                new CodeInstruction(OpCodes.Beq_S, continueLabel),
                new CodeInstruction(OpCodes.Ret),
            });

            foreach (var instruction in newInstructions)
                yield return instruction;

            ListPool<CodeInstruction>.Shared.Return(newInstructions);
        }

        private static int ActiveGenerators()
            => Generator.List.Where(x => x.IsEngaged).Count();
    }
}
