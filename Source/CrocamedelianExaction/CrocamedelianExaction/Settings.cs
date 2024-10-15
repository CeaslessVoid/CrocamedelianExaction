using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace CrocamedelianExaction
{
    // Players keep an internal colony points system. The move submissive the player is, the more bad quest will happen
    // Also works vice-versa (not yet implemented)

    public class Settings : ModSettings
    {
        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look<bool>(ref this.CrE_Male, "CrE_Male", true, true);
            Scribe_Values.Look<bool>(ref this.CrE_Female, "CrE_Female", true, true);

            Scribe_Values.Look<float>(ref this.CrE_ExtortLossChance, "CrE_ExtortLossChance", 0.3f, true);

            Scribe_Values.Look<int>(ref this.minDaysBetweenEvents, "minDaysBetweenEvents", 10, true);
            Scribe_Values.Look<int>(ref this.maxDaysBetweenEvents, "maxDaysBetweenEvents", 25, true);

            Scribe_Values.Look<float>(ref this.CrE_PawnLossChance, "CrE_PawnLossChance", 0.3f, true);

        }

        // Disable for male / female
        public bool CrE_Male = true;
        public bool CrE_Female = true;

        // Lose Pawn when extort
        public float CrE_ExtortLossChance = 0.3f;

        // Lose Pawn Days
        public int minDaysBetweenEvents = 10; // 10
        public int maxDaysBetweenEvents = 25; // 25

        // Chance Pawn is kept by pirates
        public float CrE_PawnLossChance = 0.2f;

    }

}
