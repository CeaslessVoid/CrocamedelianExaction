using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Verse;
using RimWorld;


namespace CrocamedelianExaction
{
    //public class ChoiceLetter_CrEExtortionDemand : ChoiceLetter
    //{
    //    public bool completed;
    //    public Faction faction;
    //    public Map map;
    //    public bool outpost = false;

    //    public override IEnumerable<DiaOption> Choices
    //    {
    //        get
    //        {
    //            if (ArchivedOnly)
    //            {
    //                yield return Option_Close;
    //            }
    //            else
    //            {
    //                var accept = new DiaOption("Accept")
    //                {
    //                    action = () =>
    //                    {
    //                        completed = true;
    //                        Find.LetterStack.RemoveLetter(this);
    //                        // Put actions we want here

    //                    },
    //                    resolveTree = true
    //                };

    //                yield return accept;

    //                var reject = new DiaOption("Reject")
    //                {
    //                    action = () =>
    //                    {
    //                        completed = true;
    //                        var incidentParms =
    //                            StorytellerUtility.DefaultParmsNow(IncidentCategoryDefOf.ThreatBig, map);
    //                        incidentParms.forced = true;
    //                        incidentParms.faction = faction;
    //                        incidentParms.raidStrategy = RaidStrategyDefOf.ImmediateAttack;
    //                        incidentParms.raidArrivalMode = PawnsArrivalModeDefOf.EdgeWalkIn;
    //                        incidentParms.target = map;
    //                        if (outpost)
    //                        {
    //                            incidentParms.points *= 0.7f;
    //                        }

    //                        IncidentDefOf.RaidEnemy.Worker.TryExecute(incidentParms);
    //                        Find.LetterStack.RemoveLetter(this);
    //                    },
    //                    resolveTree = true
    //                };
    //                yield return reject;
    //                yield return Option_Postpone;
    //            }
    //        }
    //    }

    //    public override bool CanShowInLetterStack => Find.Maps.Contains(map);

    //    public override void ExposeData()
    //    {
    //        base.ExposeData();
    //        Scribe_References.Look(ref map, "map");
    //        Scribe_References.Look(ref faction, "faction");
    //        Scribe_Values.Look(ref completed, "completed");
    //    }
    //}

}
