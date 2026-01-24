using HarmonyLib;
using Nautilus.Assets;
using Nautilus.Assets.Gadgets;
using Nautilus.Crafting;
using System.Collections.Generic;
using UnityEngine;

namespace alterratech.Items.modules
{
    public class PrawnOdradekDisplayManager : MonoBehaviour
    {
        public TechType moduleTechType;
        private List<GameObject> spawnedModels = new List<GameObject>();
        private Exosuit exosuit;
        private bool wasActive = false;

        private void Start()
        {
            exosuit = GetComponent<Exosuit>();
            InvokeRepeating(nameof(Refresh), 1f, 1f);
        }

        private void Refresh()
        {
            if (exosuit == null) return;
            bool hasModule = exosuit.modules.GetCount(moduleTechType) > 0;

            if (hasModule && !wasActive)
            {
                ErrorMessage.AddMessage("Higgs: Я — часть той силы, что вечно хочет зла и вечно совершает благо. Я — Хиггс, та частичка Бога, что есть во всём");
                wasActive = true;
            }
            else if (!hasModule && wasActive)
            {
                wasActive = false;
            }

            if (hasModule && spawnedModels.Count == 0)
            {
                GameObject prefab = Plugin.Bundle.LoadAsset<GameObject>("Assets/Prefabs/odradek.prefab");
                if (prefab == null) return;

                foreach (Transform t in GetComponentsInChildren<Transform>(true))
                {
                    if (t.name.ToLower().Contains("cabin"))
                    {
                        GameObject model = Instantiate(prefab, t);
                        model.transform.localPosition = new Vector3(1f, 1.3f, 0f);
                        model.transform.localScale = new Vector3(0.4f, 0.4f, 0.4f);
                        Nautilus.Utility.MaterialUtils.ApplySNShaders(model);
                        spawnedModels.Add(model);
                    }
                }
            }
            else if (!hasModule && spawnedModels.Count > 0)
            {
                foreach (var m in spawnedModels) Destroy(m);
                spawnedModels.Clear();
            }
        }
    }

    public static class PrawnOdradekModule
    {
        public static PrefabInfo Info { get; } = PrefabInfo.WithTechType("PrawnOdradekModule", "Одрадек (Краб)", "Death Stranding");

        public static void Register()
        {
            var customPrefab = new CustomPrefab(Info);
            customPrefab.SetEquipment(EquipmentType.ExosuitModule);
            customPrefab.SetRecipe(new RecipeData(new Ingredient(TechType.Titanium, 1))).WithFabricatorType(CraftTree.Type.Workbench);
            customPrefab.SetUnlock(TechType.Knife);
            customPrefab.Register();
        }
    }

    [HarmonyPatch(typeof(Exosuit))]
    public static class Exosuit_Patch
    {
        [HarmonyPatch(nameof(Exosuit.Start))]
        [HarmonyPostfix]
        public static void Postfix(Exosuit __instance)
        {
            var manager = __instance.gameObject.AddComponent<PrawnOdradekDisplayManager>();
            manager.moduleTechType = PrawnOdradekModule.Info.TechType;
        }
    }
}