using HarmonyLib;
using LudeonTK;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace CrocamedelianExaction
{
    public static class IncidentAllyPrisonerRescue
    {
        public static bool Do()
        {
            QuestScriptDef named = DefDatabase<QuestScriptDef>.GetNamed("CrE_PawnLend", true);
            float num = StorytellerUtility.DefaultThreatPointsNow(Current.Game.AnyPlayerHomeMap);
            QuestUtility.SendLetterQuestAvailable(QuestUtility.GenerateQuestAndMakeAvailable(named, num));
            return true;
        }
    }

}
