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
        private Pawn betrothed;
        private Pawn marriageSeeker;

        // Make sure not all your colinists are taken
        private bool has_pawn_out = false;

        public float chance_modifier = 2 * ((1 / (1 + Mathf.Exp(-0.1f * (CrE_GameComponent.CrE_Points - 50)))) - 0.5f);
        public override float BaseChanceThisGame => Math.Max(0.01f,
            Mathf.Clamp(base.BaseChanceThisGame - StorytellerUtilityPopulation.PopulationIntent + chance_modifier, 0.0f, 1.0f));

        public override bool CanFireNowSub(IncidentParms parms)
        {
            return base.CanFireNowSub(parms) && TryFindMarriageSeeker(out marriageSeeker)
                                             && TryFindBetrothed(out betrothed)
                                             && !this.IsScenarioBlocked()
                                             && !has_pawn_out;
        }

        public override bool TryExecuteWorker(IncidentParms parms)
        {
            if (!TryFindMarriageSeeker(out marriageSeeker) || !TryFindBetrothed(out betrothed))
            {
                return false;
            }

            var text = "MFI_DiplomaticMarriage"
                .Translate(marriageSeeker.LabelShort, betrothed.LabelShort, marriageSeeker.Faction.Name)
                .AdjustedFor(marriageSeeker);

            PawnRelationUtility.TryAppendRelationsWithColonistsInfo(ref text, marriageSeeker);

            var choiceLetterDiplomaticMarriage =
                (ChoiceLetter_DiplomaticMarriage)LetterMaker.MakeLetter(def.letterLabel, text, def.letterDef);
            choiceLetterDiplomaticMarriage.title =
                "MFI_DiplomaticMarriageLabel".Translate(betrothed.LabelShort).CapitalizeFirst();
            choiceLetterDiplomaticMarriage.radioMode = false;
            choiceLetterDiplomaticMarriage.marriageSeeker = marriageSeeker;
            choiceLetterDiplomaticMarriage.betrothed = betrothed;
            choiceLetterDiplomaticMarriage.StartTimeout(TimeoutTicks);
            Find.LetterStack.ReceiveLetter(choiceLetterDiplomaticMarriage);
            return true;
        }

        private bool TryFindBetrothed(out Pawn betrothed)
        {
            return (from potentialPartners in PawnsFinder
                    .AllMapsCaravansAndTravelingTransportPods_Alive_FreeColonistsAndPrisoners_NoCryptosleep
                    where !LovePartnerRelationUtility.HasAnyLovePartner(potentialPartners) ||
                          LovePartnerRelationUtility.ExistingMostLikedLovePartner(potentialPartners, false) ==
                          marriageSeeker
                    select potentialPartners).TryRandomElement(out betrothed);
        }

        private static bool TryFindMarriageSeeker(out Pawn marriageSeeker)
        {
            return (from x in Find.WorldPawns.AllPawnsAlive
                    where x.Faction != null && !x.Faction.def.hidden && x.Faction.def.permanentEnemy && !x.Faction.IsPlayer
                          && !x.Faction.defeated
                          && !x.IsPrisoner && !x.Spawned && x.relations != null && x.RaceProps.Humanlike
                          && !SettlementUtility.IsPlayerAttackingAnySettlementOf(x.Faction)
                    select x).TryRandomElement(out marriageSeeker);
        }
    }


    public class ChoiceLetter_CrE_DiplomaticMarriage : ChoiceLetter
    {
        public Pawn betrothed;
        private int goodWillGainedFromMarriage;
        public Pawn marriageSeeker;

        public override bool CanShowInLetterStack => base.CanShowInLetterStack &&
                                                     PawnsFinder
                                                         .AllMapsCaravansAndTravelingTransportPods_Alive_FreeColonists
                                                         .Contains(value: betrothed);

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
                            goodWillGainedFromMarriage =
                                (int)MFI_DiplomacyTunings.PawnValueInGoodWillAmountOut.Evaluate(betrothed.MarketValue);
                            marriageSeeker.Faction.SetRelationDirect(Faction.OfPlayer,
                                (FactionRelationKind)Math.Min((int)marriageSeeker.Faction.PlayerRelationKind + 1, 2),
                                true, "LetterLabelAcceptedProposal".Translate());
                            marriageSeeker.Faction.TryAffectGoodwillWith(Faction.OfPlayer, goodWillGainedFromMarriage,
                                false);
                            betrothed.relations.AddDirectRelation(PawnRelationDefOf.Fiance, marriageSeeker);

                            var caravan = betrothed.GetCaravan();
                            if (caravan != null)
                            {
                                CaravanInventoryUtility.MoveAllInventoryToSomeoneElse(betrothed, caravan.PawnsListForReading);
                                caravan.RemovePawn(betrothed);
                            }

                            DetermineAndDoOutcome(marriageSeeker, betrothed);
                            Find.LetterStack.RemoveLetter(this);
                        }
                    };
                    var dialogueNodeAccept = new DiaNode("MFI_AcceptedProposal"
                        .Translate(betrothed, marriageSeeker.Faction).CapitalizeFirst().AdjustedFor(marriageSeeker));
                    dialogueNodeAccept.options.Add(Option_Close);
                    accept.link = dialogueNodeAccept;

                    var reject = new DiaOption("RansomDemand_Reject".Translate())
                    {
                        action = () =>
                        {
                            Find.LetterStack.RemoveLetter(this);

                            var incidentParms =
                            StorytellerUtility.DefaultParmsNow(IncidentCategoryDefOf.ThreatBig, Find.AnyPlayerHomeMap);
                            incidentParms.forced = true;
                            incidentParms.faction = marriageSeeker.Faction;
                            incidentParms.raidStrategy = RaidStrategyDefOf.ImmediateAttack;
                            incidentParms.raidArrivalMode = PawnsArrivalModeDefOf.EdgeWalkIn;
                            incidentParms.target = Find.AnyPlayerHomeMap;

                            IncidentDefOf.RaidEnemy.Worker.TryExecute(incidentParms);

                        }
                    };
                    var dialogueNodeReject = new DiaNode("MFI_DejectedProposal"
                        .Translate(marriageSeeker.LabelCap, marriageSeeker.Faction).CapitalizeFirst()
                        .AdjustedFor(marriageSeeker));
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
            if (Prefs.LogVerbose)
            {
                Log.Warning("Determine and do outcome after marriage.");
            }

            betrothed.SetFaction(!marriageSeeker.HostileTo(Faction.OfPlayer)
                ? marriageSeeker.Faction
                : null);

            Faction.OfPlayer.ideos?.RecalculateIdeosBasedOnPlayerPawns();

            //todo: maybe plan visit, deliver dowry, do wedding.
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_References.Look(ref betrothed, "betrothed");
            Scribe_References.Look(ref marriageSeeker, "marriageSeeker");
        }
    }
}
