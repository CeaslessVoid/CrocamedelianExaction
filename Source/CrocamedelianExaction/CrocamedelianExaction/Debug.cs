using HarmonyLib;
using LudeonTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace CrocamedelianExaction
{
    public static class Debug
    {
        [DebugAction(null, null, false, false, false, false, 0, false, category = "Exaction", name = "Print Current Points", requiresRoyalty = false, requiresIdeology = false, requiresBiotech = false, actionType = 0, allowedGameStates = LudeonTK.AllowedGameStates.Playing)]
        private static void PrintCrEPoints() // Prints current CrE points
        {
            Util.Msg(CrE_GameComponent.CrE_Points);
        }

        [DebugAction(null, null, false, false, false, false, 0, false, category = "Exaction", name = "Print Random Valid Pawn", requiresRoyalty = false, requiresIdeology = false, requiresBiotech = false, actionType = 0, allowedGameStates = LudeonTK.AllowedGameStates.Playing)]
        private static void PrintRandomValidPawn() // Prints current CrE points
        {
            Util.Msg(CrE_GameComponent.GetRandomPawnForEvent());
        }

        private const string CATEGORY = "Exaction";
    }
}
