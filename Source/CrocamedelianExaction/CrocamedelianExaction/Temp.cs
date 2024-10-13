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
//using MoreFactionInteraction;
using RimWorld.Planet;

namespace CrocamedelianExaction
{
    public static class IncidentAllyPrisonerRescue
    {
        public static bool Do()
        {

            IncidentDef incidentDef = DefDatabase<IncidentDef>.GetNamed("CrE_PawnLend", true);
            //IncidentDef incidentDef = DefDatabase<IncidentDef>.GetNamed("MFI_PirateExtortion", true);

            // Create incident parameters
            var incidentParms = StorytellerUtility.DefaultParmsNow(IncidentCategoryDefOf.ThreatBig, Find.AnyPlayerHomeMap);
            incidentParms.forced = true;
            incidentParms.target = Find.AnyPlayerHomeMap;

            // Try to execute the incident
            bool result = incidentDef.Worker.TryExecute(incidentParms);


            return true;

        }

    }

}
