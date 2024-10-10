using HarmonyLib;
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
            CrE_GameComponent.CrE_Points = Settings.InitalCrEPoints;
        }

        // Change points
        public static void ChangeCrEPoints(int points)
        {
            CrE_GameComponent.CrE_Points += points; // Just use negative numbers for decrease
        }

        public static int CrE_Points;


    }

}
