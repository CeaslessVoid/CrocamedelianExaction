﻿using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
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
            Scribe_Values.Look<bool>(ref this.CrE_PirateExtort, "CrE_PirateExtort", this.CrE_PirateExtort, true);
            Scribe_Values.Look<bool>(ref this.CrE_Male, "CrE_Male", this.CrE_Male, true);
            Scribe_Values.Look<bool>(ref this.CrE_Female, "CrE_Female", this.CrE_Female, true);

            Scribe_Values.Look<float>(ref this.CrE_ExtortLossChance, "CrE_ExtortLossChance", 0.3f, true);

            Scribe_Values.Look<int>(ref this.minDaysBetweenEvents, "minDaysBetweenEvents", 10, true);
            Scribe_Values.Look<int>(ref this.maxDaysBetweenEvents, "maxDaysBetweenEvents", 25, true);

        }

        public bool CrE_PirateExtort = true;

        // Disable for male / female
        public bool CrE_Male = true;
        public bool CrE_Female = true;

        // Lose Pawn when extort
        public float CrE_ExtortLossChance = 0.3f;

        // Lose Pawn Days
        public int minDaysBetweenEvents = 10; // 10
        public int maxDaysBetweenEvents = 25; // 25

    }


    internal class CrEMod : Mod
    {
        public CrEMod(ModContentPack content) : base(content)
        {
            this._settings = GetSettings<Settings>();
        }

        private Settings _settings;

        public override void DoSettingsWindowContents(Rect inRect)
        {
            Listing_Standard listing_Standard = new Listing_Standard();
            listing_Standard.Begin(inRect);

            listing_Standard.CheckboxLabeled("Allow Pirate Extortion for Pawns (Needs restart)", ref this._settings.CrE_PirateExtort);

            if (this._settings.CrE_PirateExtort)
            {


                string minDaysText = this._settings.minDaysBetweenEvents.ToString();
                listing_Standard.TextFieldNumericLabeled("Minimum Pirate Pawn Kept Day", ref this._settings.minDaysBetweenEvents, ref minDaysText, 0, 300);
                this._settings.minDaysBetweenEvents = Mathf.RoundToInt(listing_Standard.Slider(this._settings.minDaysBetweenEvents, 0f, 300f));
                listing_Standard.Gap(12f);

                string maxDaysText = this._settings.maxDaysBetweenEvents.ToString();
                listing_Standard.TextFieldNumericLabeled("Maximum Pirate Pawn Kept Day", ref this._settings.maxDaysBetweenEvents, ref maxDaysText, 1, 300);
                this._settings.maxDaysBetweenEvents = Mathf.RoundToInt(listing_Standard.Slider(this._settings.maxDaysBetweenEvents, 1f, 300f));

                if (this._settings.minDaysBetweenEvents >= this._settings.maxDaysBetweenEvents)
                {
                    this._settings.minDaysBetweenEvents = this._settings.maxDaysBetweenEvents - 1;
                }

                listing_Standard.Gap(12f);

                string extortLossChanceText = this._settings.CrE_ExtortLossChance.ToString("F2");
                listing_Standard.TextFieldNumericLabeled("Chance Pirate Keeps Pawn (x 100)", ref this._settings.CrE_ExtortLossChance, ref extortLossChanceText, 0f, 100f);
                this._settings.CrE_ExtortLossChance = listing_Standard.Slider(this._settings.CrE_ExtortLossChance, 0f, 1f);
                listing_Standard.Gap(12f);

                listing_Standard.CheckboxLabeled("Allow Male", ref this._settings.CrE_Male);
                listing_Standard.Gap(6f);

                listing_Standard.CheckboxLabeled("Allow Female", ref this._settings.CrE_Female);
                listing_Standard.Gap(6f);

                listing_Standard.End();
            }

            base.DoSettingsWindowContents(inRect);
            WriteSettings();
        }

        public override string SettingsCategory()
        {
            return "Crocamedelian's Exaction";
        }

        public override void WriteSettings()
        {
            base.WriteSettings();
            LoadedModManager.GetMod<CrEMod>().GetSettings<Settings>().Write();
        }
    }

}
