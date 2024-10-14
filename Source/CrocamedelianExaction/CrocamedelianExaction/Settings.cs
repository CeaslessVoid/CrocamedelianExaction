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
            Scribe_Values.Look<int>(ref this.InitalCrEPoints, "InitalCrEPoints", 50, true);
            Scribe_Values.Look<bool>(ref this.CrE_Male, "CrE_Male", true, true);
            Scribe_Values.Look<bool>(ref this.CrE_Female, "CrE_Female", true, true);

        }

        // Starting point for bad events
        public int InitalCrEPoints = 50;

        // Disable for male / female
        public bool CrE_Male = true;
        public bool CrE_Female = true;





        //--------------------------------------
        // Data Store

        public bool CrE_has_pawn_pirate = false;


    }

}
