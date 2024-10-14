using HarmonyLib;
using LudeonTK;
using MoreFactionInteraction;
using MoreFactionInteraction.General;
using MoreFactionInteraction.MoreFactionWar;
using RimWorld;
using RimWorld.Planet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;
using static System.Collections.Specialized.BitVector32;
using Verse.Noise;

namespace CrocamedelianExaction
{
    public class IncidentWorker_CrEPiratePawnExtort : IncidentWorker_DiplomaticMarriage
    {
        private const int TimeoutTicks = GenDate.TicksPerDay;
        private Pawn vicitim;
        private Pawn pirateLeader;
        // Make sure not all your colinists are taken
        public float chance_modifier = (float)Math.Round(Math.Exp(2 * ((1 / (1 + Mathf.Exp(-0.02f * CrE_GameComponent.CrE_Points))) - 0.5f)) - 1,2);
        public override float BaseChanceThisGame => Math.Max(0.01f,
            Mathf.Clamp(base.BaseChanceThisGame - StorytellerUtilityPopulation.PopulationIntent + chance_modifier, 0.0f, 1.0f));

        public override bool CanFireNowSub(IncidentParms parms)
        {
            return base.CanFireNowSub(parms) && TryFindPirateLeader(out pirateLeader)
                                             && TryFindVictim(out vicitim)
                                             && !this.IsScenarioBlocked()
                                             && !CrE_GameComponent.has_pawn_out;
        }

        public override bool TryExecuteWorker(IncidentParms parms)
        {
            if (!TryFindPirateLeader(out pirateLeader) || !TryFindVictim(out vicitim))
            {
                return false;
            }

            var text = "CrE_PiratePawn_Extort"
                .Translate(pirateLeader.LabelShort, vicitim.LabelShort, pirateLeader.Faction.Name)
                .AdjustedFor(pirateLeader);

            PawnRelationUtility.TryAppendRelationsWithColonistsInfo(ref text, pirateLeader);

            var ChoiceLetter_CrE_Demand_Pawn =
                (ChoiceLetter_CrE_Demand_Pawn)LetterMaker.MakeLetter(def.letterLabel, text, def.letterDef);
            ChoiceLetter_CrE_Demand_Pawn.title =
                "CrE_PiratePawn_ExtortLabel".Translate(vicitim.LabelShort).CapitalizeFirst();
            ChoiceLetter_CrE_Demand_Pawn.radioMode = false;
            ChoiceLetter_CrE_Demand_Pawn.pirateLeader = pirateLeader;
            ChoiceLetter_CrE_Demand_Pawn.vicitim = vicitim;
            ChoiceLetter_CrE_Demand_Pawn.StartTimeout(TimeoutTicks);
            Find.LetterStack.ReceiveLetter(ChoiceLetter_CrE_Demand_Pawn);
            return true;
        }

        private bool TryFindVictim(out Pawn betrothed)
        {
            return (from potentialPartners in PawnsFinder
                    .AllMapsCaravansAndTravelingTransportPods_Alive_FreeColonistsAndPrisoners_NoCryptosleep
                    where CrE_GameComponent.isValidPawn(potentialPartners)
                    select potentialPartners).TryRandomElement(out betrothed);
        }

        private static bool TryFindPirateLeader(out Pawn pirateLeader)
        {
            return (from x in Find.WorldPawns.AllPawnsAlive
                    where x.Faction != null && !x.Faction.def.hidden && x.Faction.def.permanentEnemy && !x.Faction.IsPlayer
                          && !x.Faction.defeated
                          && !x.IsPrisoner && !x.Spawned && x.relations != null && x.RaceProps.Humanlike
                          && !SettlementUtility.IsPlayerAttackingAnySettlementOf(x.Faction)
                    select x).TryRandomElement(out pirateLeader);
        }
    }


    public class ChoiceLetter_CrE_Demand_Pawn : ChoiceLetter
    {
        public Pawn vicitim;
        private int goodWillGainedFromMarriage;
        public Pawn pirateLeader;

        public override bool CanShowInLetterStack => base.CanShowInLetterStack &&
                                                     PawnsFinder
                                                         .AllMapsCaravansAndTravelingTransportPods_Alive_FreeColonists
                                                         .Contains(value: vicitim);

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

                            CrE_GameComponent.ChangeCrEPoints(Rand.Range(5,7));
                            CrE_GameComponent.has_pawn_out = true;

                            var caravan = vicitim.GetCaravan();
                            if (caravan != null)
                            {
                                CaravanInventoryUtility.MoveAllInventoryToSomeoneElse(vicitim, caravan.PawnsListForReading);
                                caravan.RemovePawn(vicitim);
                            }

                            DetermineAndDoOutcome(pirateLeader, vicitim);
                            Find.LetterStack.RemoveLetter(this);
                        }
                    };
                    var dialogueNodeAccept = new DiaNode("MFI_AcceptedProposal"
                        .Translate(vicitim, pirateLeader.Faction).CapitalizeFirst().AdjustedFor(pirateLeader));
                    dialogueNodeAccept.options.Add(Option_Close);
                    accept.link = dialogueNodeAccept;

                    var reject = new DiaOption("RansomDemand_Reject".Translate())
                    {
                        action = () =>
                        {
                            CrE_GameComponent.ChangeCrEPoints(Rand.Range(-2, -5));
                            Find.LetterStack.RemoveLetter(this);

                            var incidentParms =
                            StorytellerUtility.DefaultParmsNow(IncidentCategoryDefOf.ThreatBig, Find.AnyPlayerHomeMap);
                            incidentParms.forced = true;
                            incidentParms.faction = pirateLeader.Faction;
                            incidentParms.raidStrategy = RaidStrategyDefOf.ImmediateAttack;
                            incidentParms.raidArrivalMode = PawnsArrivalModeDefOf.EdgeWalkIn;
                            incidentParms.target = Find.AnyPlayerHomeMap;

                            IncidentDefOf.RaidEnemy.Worker.TryExecute(incidentParms);

                        }
                    };
                    var dialogueNodeReject = new DiaNode("MFI_DejectedProposal"
                        .Translate(pirateLeader.LabelCap, pirateLeader.Faction).CapitalizeFirst()
                        .AdjustedFor(pirateLeader));
                    dialogueNodeReject.options.Add(Option_Close);
                    reject.link = dialogueNodeReject;

                    yield return accept;
                    yield return reject;
                    yield return Option_Postpone;
                }
            }
        }

        private static void DetermineAndDoOutcome(Pawn marriageSeeker, Pawn betrothed)
        {

            betrothed.SetFaction(!marriageSeeker.HostileTo(Faction.OfPlayer)
                ? marriageSeeker.Faction
                : null);

            Faction.OfPlayer.ideos?.RecalculateIdeosBasedOnPlayerPawns();

        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_References.Look(ref vicitim, "vicitim");
            Scribe_References.Look(ref pirateLeader, "pirateLeader");
        }
    }
}
