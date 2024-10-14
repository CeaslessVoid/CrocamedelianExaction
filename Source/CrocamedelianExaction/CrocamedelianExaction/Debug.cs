using HarmonyLib;
using LudeonTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace CrocamedelianExaction
{
    public static class Debug
    {
        [DebugAction(null, null, false, false, false, false, 0, false, category = "Exaction", name = "Print Current Points", requiresRoyalty = false, requiresIdeology = false, requiresBiotech = false, actionType = 0, allowedGameStates = LudeonTK.AllowedGameStates.Playing)]
        private static void PrintCrEPoints() // Prints current CrE points
        {
            Util.Msg(CrE_GameComponent.CrE_Points);
            Util.Msg(CrE_GameComponent.has_pawn_out);
            Util.Msg((float)Math.Round(Math.Exp(2 * ((1 / (1 + Mathf.Exp(-0.02f * CrE_GameComponent.CrE_Points))) - 0.5f)) - 1, 2));
        }

        [DebugAction(null, null, false, false, false, false, 0, false, category = "Exaction", name = "Print Random Valid Pawn", requiresRoyalty = false, requiresIdeology = false, requiresBiotech = false, actionType = 0, allowedGameStates = LudeonTK.AllowedGameStates.Playing)]
        private static void PrintRandomValidPawn() // Prints current CrE points
        {
            Util.Msg(CrE_GameComponent.GetRandomPawnForEvent());
        }

        [DebugAction(null, null, false, false, false, false, 0, false, category = "Exaction", name = "Print Current Pawn", requiresRoyalty = false, requiresIdeology = false, requiresBiotech = false, actionType = 0, allowedGameStates = LudeonTK.AllowedGameStates.Playing)]
        private static void PrintCurrentVictim() // Prints current CrE vicitim pawn
        {
            Util.Msg(CrE_GameComponent.CurrentCrEPawn);
        }

        [DebugAction(null, null, false, false, false, false, 0, false, category = "Exaction", name = "REMOVE THIS LATER", requiresRoyalty = false, requiresIdeology = false, requiresBiotech = false, actionType = 0, allowedGameStates = LudeonTK.AllowedGameStates.Playing)]
        private static void TestDoInci() // Prints current CrE points
        {
            IncidentAllyPrisonerRescue.Do();
        }

        [DebugAction(null, null, false, false, false, false, 0, false, category = "Exaction", name = "REMOVE THIS LATER 2", requiresRoyalty = false, requiresIdeology = false, requiresBiotech = false, actionType = 0, allowedGameStates = LudeonTK.AllowedGameStates.Playing)]
        private static void TestDoInciReturn() // Prints current CrE points
        {
            CrE_PiratePawn_Return.Do();
        }

        private const string CATEGORY = "Exaction";
    }
}
