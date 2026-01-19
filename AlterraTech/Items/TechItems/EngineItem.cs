using alterratech.Items.Equipment;
using Nautilus.Assets;
using Nautilus.Assets.Gadgets;
using Nautilus.Crafting;
using Nautilus.Handlers;
using Nautilus.Utility;
using UnityEngine;

namespace alterratech.Items.TechItems
{
    public static class EngineItem
    {
        public static PrefabInfo Info { get; } = PrefabInfo
            .WithTechType("Engine", "Теневой Двигатель", "Экспериментальный прототип двигателя. Излучает едва заметное тепло.")
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
                new Ingredient(TechType.PrecursorIonCrystal, 2),
                new Ingredient(TechType.WiringKit, 2),
                new Ingredient(TechType.CopperWire, 3)
            )).WithFabricatorType(CraftTree.Type.Fabricator);
            customPrefab.SetPdaGroupCategory(TechGroup.Resources, TechCategory.Electronics);
            customPrefab.SetUnlock(ShadowTank.Info.TechType);
            customPrefab.Register();
            RegisterEncyclopedia();
        }

        private static void RegisterEncyclopedia()
        {
            // Используем TechType как ID для энциклопедии, чтобы StoryGoalHandler его подцепил
            string entryId = "Engine";

            PDAHandler.AddEncyclopediaEntry(
                entryId,
                "Tech/Artifacts",
                "Двигатель (Прототип #7600)",
                "Данный агрегат не числится в официальном реестре Alterra. \n\n" +
                "**Технический анализ:**\n" +
                "Устройство представляет собой гибрид термального реактора и ионного ускорителя. " +
                "Внутренняя архитектура сильно изменена. Поверхностные слои имеют следы ручной пайки и " +
                "нестандартной калибровки, характерной для команды 'Альтерра'.\n\n" +
                "**Обнаруженные аномалии:**\n" +
                "1. Эмиссия: Текстурные датчики фиксируют свечение, не связанное с потреблением энергии.\n" +
                "2. Температура: Объект остается теплым даже в арктических водах.\n" +
                "3. Программный код: В прошивку вшит алгоритм 'Shadow Protocol', блокирующий удаленный доступ Alterra.\n\n" +
                "**ВНИМАНИЕ:** Использование данного двигателя в гражданских постройках может привести к нарушению гарантии и дестабилизации реальности."
            );

            // Твоя верная строка
            StoryGoalHandler.RegisterItemGoal(entryId, Story.GoalType.Encyclopedia, Info.TechType);
        }
    }
}