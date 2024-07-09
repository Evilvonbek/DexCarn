using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using UnityModManagerNet;
using Kingmaker.Blueprints.JsonSystem;
using BlueprintCore.Utils;
using BlueprintCore.Blueprints.References;
using BlueprintCore.Blueprints.CustomConfigurators.Classes;
using Kingmaker.Blueprints;
using BlueprintCore.Blueprints.CustomConfigurators.Classes.Selection;
using Kingmaker.Blueprints.Classes.Prerequisites;

namespace DexCarn
{

#if DEBUG
    [EnableReloading]
#endif
    public static class Main
    {
        internal static Harmony HarmonyInstance;
        internal static UnityModManager.ModEntry.ModLogger log;

        public static bool Load(UnityModManager.ModEntry modEntry)
        {
            log = modEntry.Logger;
#if DEBUG
            modEntry.OnUnload = OnUnload;
#endif
            modEntry.OnGUI = OnGUI;
            HarmonyInstance = new Harmony(modEntry.Info.Id);
            HarmonyInstance.PatchAll(Assembly.GetExecutingAssembly());
            return true;
        }

        public static void OnGUI(UnityModManager.ModEntry modEntry)
        {

        }

#if DEBUG
        public static bool OnUnload(UnityModManager.ModEntry modEntry)
        {
            HarmonyInstance.UnpatchAll(modEntry.Info.Id);
            return true;
        }
#endif
        [HarmonyPatch(typeof(BlueprintsCache))]
        static class BlueprintsCaches_Patch
        {
            // Uses BlueprintCore's LogWrapper which uses Owlcat's internal logging.
            // Logs to `%APPDATA%\..\LocalLow\Owlcat Games\Pathfinder Wrath Of The Righteous\GameLogFull.txt` and the Mods
            // channel in RemoteConsole.
            private static readonly LogWrapper Logger = LogWrapper.Get("DexCarn");
            private static bool Initialized = false;

            [HarmonyPriority(Priority.First)]
            [HarmonyPatch(nameof(BlueprintsCache.Init)), HarmonyPostfix]
            static void Postfix()
            {
                try
                {
                    if (Initialized)
                    {
                        Logger.Info("Already initialized blueprints cache.");
                        return;
                    }
                    Initialized = true;

                    Logger.Info("Patching blueprints.");
                    //< INSERT YOUR whatever.configure() here >
                    DextrousCarnage.Configure();
                }
                catch (Exception e)
                {
                    Logger.Error("Failed to initialize.", e);
                }
            }
        }
    }

    class DextrousCarnage
    {
        public static void Configure()
        {
            var DextrousCarnage = FeatureConfigurator.New("Dextrous Carnage", "524D86A7-819B-4E4D-9C4F-A27975974E10")
                                                     .CopyFrom(FeatureRefs.DreadfulCarnage, c => c is not (PrerequisiteStatValue or PrerequisiteFeature))
//                                                     .SetDisplayName("DextrousCarnage.Name")
//                                                     .SetDescription("DextrousCarnage.Disc")
                                                     .AddPrerequisiteStatValue(Kingmaker.EntitySystem.Stats.StatType.Dexterity, 15)
                                                     .AddPrerequisiteFeature(FeatureRefs.DazzlingDisplayFeature.Reference.Get())
                                                     .AddPrerequisiteFeature(FeatureRefs.PiranhaStrikeFeature.Reference.Get())
                                                     .AddPrerequisiteStatValue(Kingmaker.EntitySystem.Stats.StatType.BaseAttackBonus, 11)
                                                     .Configure();
            FeatureSelectionConfigurator.For(FeatureSelectionRefs.BasicFeatSelection).AddToAllFeatures(DextrousCarnage).Configure();
        }
    }

}