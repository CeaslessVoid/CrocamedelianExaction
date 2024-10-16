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
using MoreFactionInteraction;

namespace CrocamedelianExaction
{
    public class CrE_ChoiceLetter_ExtortionDemand : ChoiceLetter_ExtortionDemand
    {
        public override IEnumerable<DiaOption> Choices
        {
            get
            {
                if (ArchivedOnly)
                {
                    yield return Option_Close;
                }
                else
                {
                    var accept = new DiaOption("RansomDemand_Accept".Translate())
                    {
                        action = () =>
                        {
                            completed = true;
                            TradeUtility.LaunchSilver(map, fee);
                            Find.LetterStack.RemoveLetter(this);
                            CrE_GameComponent.ChangeCrEPoints(Rand.Range(2, 3));
                        },
                        resolveTree = true
                    };
                    if (!TradeUtility.ColonyHasEnoughSilver(map, fee))
                    {
                        accept.Disable("NeedSilverLaunchable".Translate(fee.ToString()));
                    }

                    yield return accept;

                    var reject = new DiaOption("RansomDemand_Reject".Translate())
                    {
                        action = () =>
                        {
                            completed = true;
                            var incidentParms =
                                StorytellerUtility.DefaultParmsNow(IncidentCategoryDefOf.ThreatBig, map);
                            incidentParms.forced = true;
                            incidentParms.faction = faction;
                            incidentParms.raidStrategy = RaidStrategyDefOf.ImmediateAttack;
                            incidentParms.raidArrivalMode = PawnsArrivalModeDefOf.EdgeWalkIn;
                            incidentParms.target = map;
                            if (outpost)
                            {
                                incidentParms.points *= 0.7f;
                            }

                            IncidentDefOf.RaidEnemy.Worker.TryExecute(incidentParms);
                            Find.LetterStack.RemoveLetter(this);
                        },
                        resolveTree = true
                    };
                    yield return reject;
                    yield return Option_Postpone;
                }
            }
        }

        public override bool CanShowInLetterStack => Find.Maps.Contains(map);

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_References.Look(ref map, "map");
            Scribe_References.Look(ref faction, "faction");
            Scribe_Values.Look(ref fee, "fee");
            Scribe_Values.Look(ref completed, "completed");
        }


    }

}
