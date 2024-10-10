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

        }

        // Starting point for bad events
        public int InitalCrEPoints = 50;



    }

}
