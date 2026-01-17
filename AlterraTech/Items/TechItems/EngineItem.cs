using Nautilus.Assets;
using Nautilus.Assets.Gadgets;
using Nautilus.Crafting;
using Nautilus.Utility;
using UnityEngine;

namespace alterratech.Items.TechItems
{
    public static class EngineItem
    {
        public static PrefabInfo Info { get; } = PrefabInfo
            .WithTechType("ShadowEngine", "Теневой Двигатель", "Экспериментальный прототип двигателя. Излучает едва заметное тепло.")
            .WithIcon(Plugin.Bundle.LoadAsset<Sprite>("Assets/Sprite/EngineSp.png"))
            .WithSizeInInventory(new Vector2int(2, 2));

        public static void Register()
        {
            var customPrefab = new CustomPrefab(Info);

            customPrefab.SetGameObject(() =>
            {
                GameObject prefab = Plugin.Bundle.LoadAsset<GameObject>("Assets/Prefabs/engineobj.prefab");
                if (prefab == null) return new GameObject("Empty (Bundle Error)");

                GameObject instance = Object.Instantiate(prefab);
                MaterialUtils.ApplySNShaders(instance);

                instance.EnsureComponent<PrefabIdentifier>().ClassId = Info.ClassID;
                instance.EnsureComponent<LargeWorldEntity>().cellLevel = LargeWorldEntity.CellLevel.Near;

                var rb = instance.EnsureComponent<Rigidbody>();
                rb.useGravity = true;
                rb.mass = 10f;

                instance.EnsureComponent<Pickupable>();

                return instance;
            });

            customPrefab.SetRecipe(new RecipeData(
                new Ingredient(TechType.TitaniumIngot, 1),
                new Ingredient(TechType.WiringKit, 1),
                new Ingredient(TechType.CopperWire, 2)
            )).WithFabricatorType(CraftTree.Type.Fabricator);

            customPrefab.SetPdaGroupCategory(TechGroup.Resources, TechCategory.Electronics);

            customPrefab.Register();
        }
    }
}