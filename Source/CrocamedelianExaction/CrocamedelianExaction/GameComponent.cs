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
            CrE_GameComponent.Settings = LoadedModManager.GetMod<Mod>().GetSettings<Settings>();
        }

        public static void InitOnNewGame()
        {
            CrE_GameComponent.CrE_Points = 0;
            CrE_GameComponent.has_pawn_out = false;
        }

        // Change points
        public static void ChangeCrEPoints(int points)
        {
            CrE_GameComponent.CrE_Points += points; // Just use negative numbers for decrease
        }


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

        }

    }

}
