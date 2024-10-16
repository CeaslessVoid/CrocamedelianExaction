using HarmonyLib;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime;
using System.Text;
using System.Threading.Tasks;
using Verse;
using Verse.Noise;
using UnityEngine;

namespace CrocamedelianExaction
{
    // Players keep an internal colony points system. The move submissive the player is, the more bad quest will happen
    // Also works vice-versa (not yet implemented)

    internal class CrE_GameComponent : GameComponent
    {
        // Load Settings
        public static Settings Settings { get; private set; }
        public CrE_GameComponent(Game game)
        {
            CrE_GameComponent.Settings = LoadedModManager.GetMod<CrEMod>().GetSettings<Settings>();
        }

        public static void InitOnNewGame()
        {
            CrE_GameComponent.CrE_Points = 0;
            CrE_GameComponent.has_pawn_out = false;

            CrE_GameComponent.CapturedPawnsQue.Clear();
            CrE_GameComponent.CurrentCrEPawn = null;
            CrE_GameComponent.CrE_Pawn_Return_Time = -1;
        }

        public static void InitOnLoad()
        {
            if (CrE_GameComponent.CapturedPawnsQue == null)
            {
                CrE_GameComponent.CapturedPawnsQue = new List<Pawn>();
            }
        }

        public override void GameComponentTick() // Every day
        {
            base.GameComponentTick();
            if (GenTicks.IsTickInterval(60000))
            {
                PerformDailyPawnCheck();
            }
        }

        public static void MovePawnFromWorld(Pawn pawn)
        {
            if (pawn != null)
            {
                if (Find.WorldPawns.Contains(pawn))
                {
                    // If the pawn is used before the move, they will dissapear but the chance of this happening is low
                    Find.WorldPawns.RemovePawn(pawn);
                    Util.Msg("Move Pawns Out of WorldPawns.");
                }
            }
        }

        public static void DoPirateTakePawn()
        {
            MovePawnFromWorld(CurrentCrEPawn);
            int minDays = Settings.minDaysBetweenEvents * 60000;
            int maxDays = Settings.maxDaysBetweenEvents * 60000;

            CrE_Pawn_Return_Time = Find.TickManager.TicksGame + UnityEngine.Random.Range(minDays, maxDays);
        }

        private void PerformDailyPawnCheck()
        {

            // What to do with sent pawns
            if (Find.TickManager.TicksGame >= CrE_GameComponent.CrE_Pawn_Return_Time && CrE_GameComponent.CrE_Pawn_Return_Time != -1 && CrE_GameComponent.CurrentCrEPawn != null)
            {
                // All these actions will set the timer back down
                CrE_GameComponent.CrE_Pawn_Return_Time = -1;
                has_pawn_out = false;

                if (Rand.Chance(Settings.CrE_ExtortLossChance))
                {
                    CrE_PiratePawn_NoReturn.Do();
                }
                else
                {
                    CrE_PiratePawn_Return.Do();
                }

                return;
            }

            // Forces events to happen -----------------------------------------------------------------------------------------------
            float chance = 0.05f + (float)Math.Round(Math.Exp(2 * ((1 / (1 + Mathf.Exp(-0.02f * CrE_GameComponent.CrE_Points))) - 0.5f)) - 1, 2);

            if (CrE_Pawn_Return_Time == -1 && Rand.Chance(Mathf.Clamp(chance, 0.0f, 1.0f)) && Find.TickManager.TicksGame >= 60000 * 15 && !has_pawn_out)
            {
                IncidentDef incidentDef = DefDatabase<IncidentDef>.GetNamed("CrE_PiratePawn_Extort", true);

                var incidentParms = StorytellerUtility.DefaultParmsNow(IncidentCategoryDefOf.Misc, Find.AnyPlayerHomeMap);
                incidentParms.forced = true;
                incidentParms.target = Find.AnyPlayerHomeMap;

                bool result = incidentDef.Worker.TryExecute(incidentParms);

                return;
            }
        }

        // Change points
        public static void ChangeCrEPoints(int points)
        {
            CrE_GameComponent.CrE_Points += points; // Just use negative numbers for decrease
        }

        public static List<Pawn> CapturedPawnsQue = new List<Pawn>();
        public static int CrE_Pawn_Return_Time = -1; // Time to return
        public static Pawn CurrentCrEPawn = null;

        public static int CrE_Points; // CrE Points
        public static bool has_pawn_out; // If a pawn has already been taken

        public static Pawn GetRandomPawnForEvent()
        {
            List<Pawn> allPawns = PawnsFinder.AllMapsCaravansAndTravelingTransportPods_Alive_Colonists.ToList();

            // Filter based on settings
            IEnumerable<Pawn> validPawns = allPawns.Where(pawn =>
                (Settings.CrE_Male || pawn.gender != Gender.Male) &&
                (Settings.CrE_Female || pawn.gender != Gender.Female));

            if (!validPawns.Any())
            {
                return null;
            }

            return validPawns.RandomElement();
        }

        public static bool isValidPawn(Pawn pawn)
        {
            return (Settings.CrE_Male || pawn.gender != Gender.Male) && (Settings.CrE_Female || pawn.gender != Gender.Female);
        }


        public override void ExposeData()
        {
            base.ExposeData();

            Scribe_Values.Look(ref CrE_Points, "CrE_Points", 0, true);
            Scribe_Values.Look(ref has_pawn_out, "has_pawn_out", false, true);

            Scribe_Collections.Look<Pawn>(ref CrE_GameComponent.CapturedPawnsQue, "CapturedPawnsQue", LookMode.Deep, Array.Empty<object>());
            Scribe_References.Look(ref CurrentCrEPawn, "CurrentCrEPawn");
            Scribe_Values.Look(ref CrE_Pawn_Return_Time, "CrE_Pawn_Return_Time", -1, true);

        }

    }

}
