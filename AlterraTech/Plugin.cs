using BepInEx;
using BepInEx.Logging;
using alterratech.Items.Equipment;
using alterratech.Items.Minerals;
using alterratech.Items.TechItems;
using alterratech.Items.modules;
using alterratech.Structures;
using HarmonyLib;
using System.IO;
using System.Reflection;
using UnityEngine;
using alterratech.Creatures;

namespace alterratech
{
    [BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
    [BepInDependency("com.snmodding.nautilus")]
    [BepInDependency("com.lee23.ecclibrary")]
    [BepInDependency("com.lee23.epicstructureloader")]
    public class Plugin : BaseUnityPlugin
    {
        public new static ManualLogSource Logger { get; private set; }
        private static Assembly Assembly { get; } = Assembly.GetExecutingAssembly();
        public static AssetBundle Bundle;
        public static string ModPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

        private void Awake()
        {
            Logger = base.Logger;
            Harmony.CreateAndPatchAll(Assembly, $"{PluginInfo.PLUGIN_GUID}");

            string bundlePath = Path.Combine(ModPath, "Assets", "assetbundledlc");

            if (File.Exists(bundlePath))
            {
                Bundle = AssetBundle.LoadFromFile(bundlePath);
                Logger.LogInfo("AssetBundle успешно загружен!");
            }
            else
            {
                Logger.LogError($"Файл бандла не найден по пути: {bundlePath}");
            }
            InitializePrefabs();

            Logger.LogInfo($"Plugin {PluginInfo.PLUGIN_GUID} is loaded!");
        }

        private void InitializePrefabs()
        {
            UnknownMinerales.Register();
            TechKnifePrefab.Register();
            //ShadowRebreather.Register();
            SeamothShadowModule.Register();
            PrawnOdradekModule.Register();
            ShadowTank.Register();
            StructureLoaders.LoadStructures();
            StaticStructures.Register();
            Debug.LogError("AlterraTech: Структуры Загружены");
            DeepLeviathan.RegisterEntity();
            Debug.LogError("AlterraTech: DeepLev Загружен");
            EngineItem.Register();
        }
    }
}