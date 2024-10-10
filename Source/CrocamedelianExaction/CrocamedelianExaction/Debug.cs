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
        [DebugAction(null, null, false, false, false, false, 0, false, category = "Crocamedelian's Exaction", name = "Print Current Points", requiresRoyalty = false, requiresIdeology = false, requiresBiotech = false, actionType = 0, allowedGameStates = LudeonTK.AllowedGameStates.Playing)]
        private static void PrintCrEPoints() // Prints current CrE points
        {
            Util.Msg(CrE_GameComponent.CrE_Points);
        }

        private const string CATEGORY = "Crocamedelian's Exaction";
    }
}
