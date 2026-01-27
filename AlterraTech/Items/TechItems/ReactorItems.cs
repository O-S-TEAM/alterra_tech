using Nautilus.Assets;
using Nautilus.Assets.Gadgets;
using Nautilus.Crafting;
using UnityEngine;
using Nautilus.Utility;

namespace alterratech.Items.TechItems
{
    public static class PhaseIteratorItem
    {
        public static PrefabInfo Info { get; } = PrefabInfo
            .WithTechType("PhaseIterator", "Итератор Фазы #00", "Стабилизатор ионных колебаний. Позволяет извлекать энергию из куба без его разрушения.")
            .WithIcon(SpriteManager.Get(TechType.PrecursorIonCrystal))
            .WithSizeInInventory(new Vector2int(2, 2));

        public static void Register()
        {
            var customPrefab = new CustomPrefab(Info);
            customPrefab.SetGameObject(() => {
                GameObject prefab = Object.Instantiate(Resources.Load<GameObject>("WorldEntities/Doodads/Precursor/Precursor_IonCrystal"));
                var renderers = prefab.GetComponentsInChildren<Renderer>();
                foreach (var r in renderers) r.material.color = Color.black;
                return prefab;
            });
            customPrefab.SetRecipe(new RecipeData(
                new Ingredient(TechType.PrecursorIonCrystal, 2),
                new Ingredient(TechType.AdvancedWiringKit, 1),
                new Ingredient(TechType.Magnetite, 2)
            )).WithFabricatorType(CraftTree.Type.Fabricator);
            customPrefab.SetUnlock(EngineItem.Info.TechType);
            customPrefab.Register();
        }
    }
    public static class ReactorShroudItem
    {
        public static PrefabInfo Info { get; } = PrefabInfo
            .WithTechType("ReactorShroud", "Иридиевый Кожух", "Тяжелая защита для ионных реакторов.")
            .WithIcon(SpriteManager.Get(TechType.PlasteelIngot));

        public static void Register()
        {
            var customPrefab = new CustomPrefab(Info);
            customPrefab.SetGameObject(() => {
                GameObject prefab = Object.Instantiate(Resources.Load<GameObject>("WorldEntities/Doodads/Precursor/Prison/Relics/Alien_relic_03"));
                return prefab;
            });
            customPrefab.SetRecipe(new RecipeData(
                new Ingredient(TechType.PlasteelIngot, 1),
                new Ingredient(TechType.Lead, 4),
                new Ingredient(TechType.Diamond, 2)
            )).WithFabricatorType(CraftTree.Type.Fabricator);
            customPrefab.SetUnlock(EngineItem.Info.TechType);
            customPrefab.Register();
        }
    }
}