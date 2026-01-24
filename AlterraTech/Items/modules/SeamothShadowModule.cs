using alterratech.Items.TechItems;
using Nautilus.Assets;
using Nautilus.Assets.Gadgets;
using Nautilus.Crafting;
using Nautilus.Handlers;
using UnityEngine;

namespace alterratech.Items.modules
{
    public class ShadowDepthManager : MonoBehaviour
    {
        public Vehicle vehicle;
        public CrushDamage crushDamage;
        public TechType moduleTechType;

        private void Start()
        {
            vehicle = GetComponent<Vehicle>();
            crushDamage = GetComponent<CrushDamage>();

            if (crushDamage == null)
            {
                Debug.LogError("[O.S. TEAM] ShadowDepthManager: CrushDamage не найден на объекте!");
                Destroy(this);
            }
        }

        private void LateUpdate()
        {
            if (vehicle == null || crushDamage == null) return;

            if (vehicle.modules.GetCount(moduleTechType) > 0)
            {
                crushDamage.crushDepth = 1000f;
            }
            else
            {
                Destroy(this);
            }
        }
    }

    public static class SeamothShadowModule
    {
        public static PrefabInfo Info { get; } = PrefabInfo
            .WithTechType("SeamothShadowModule", "Модуль погружения: SHADOW", "Секретная разработка проекта 'Тень'. Укрепляет корпус до 1000 метров.")
            .WithIcon(Plugin.Bundle.LoadAsset<Sprite>("Assets/Sprite/ModuleIcon.png"));

        public static void Register()
        {
            LanguageHandler.SetLanguageLine("ShadowProtocolActive", "SHADOW PROTOCOL: СИСТЕМА АКТИВИРОВАНА");

            var customPrefab = new CustomPrefab(Info);
            customPrefab.SetEquipment(EquipmentType.VehicleModule);

            customPrefab.SetGameObject(() =>
            {
                GameObject prefab = new GameObject(Info.ClassID);

                GameObject model = GameObject.CreatePrimitive(PrimitiveType.Cube);
                model.transform.SetParent(prefab.transform, false);
                model.transform.localScale = new Vector3(0.2f, 0.2f, 0.1f);

                prefab.AddComponent<PrefabIdentifier>().ClassId = Info.ClassID;
                prefab.AddComponent<TechTag>().type = Info.TechType;
                prefab.AddComponent<Pickupable>();

                Rigidbody rb = prefab.AddComponent<Rigidbody>();
                rb.useGravity = false;
                rb.isKinematic = true;
                prefab.AddComponent<WorldForces>();

                return prefab;
            });

            customPrefab.SetRecipe(new RecipeData(
                new Ingredient(EngineItem.Info.TechType, 1),
                new Ingredient(TechType.AdvancedWiringKit, 3),
                new Ingredient(TechType.EnameledGlass, 2)
            )).WithFabricatorType(CraftTree.Type.Workbench);
            customPrefab.SetVehicleUpgradeModule()
                .WithOnModuleAdded((Vehicle vehicle, int slotIndex) =>
                {
                    ShadowDepthManager manager = vehicle.gameObject.GetComponent<ShadowDepthManager>();
                    if (manager == null)
                    {
                        manager = vehicle.gameObject.AddComponent<ShadowDepthManager>();
                    }
                    manager.moduleTechType = Info.TechType;

                    ErrorMessage.AddMessage("SHADOW PROTOCOL: СИСТЕМА АКТИВИРОВАНА");
                })
                .WithEnergyCost(23f);
            RegisterModuleStory();
            customPrefab.SetUnlock(TechType.Knife);
            customPrefab.Register();
        }

        private static void RegisterModuleStory()
        {
            string entryId = "ShadowModuleEntry";
            string shadowModuleDescription =
                "<b>УРОВЕНЬ ДОСТУПА: СТРОГО ЗАСЕКРЕЧЕНО</b>\n\n" +
                "Модуль подавляет внешнее давление, используя запрещенную технологию «молекулярного сжатия». " +
                "Пока «Альтерра» ограничивает глубину погружения правилами безопасности, Протокол Тень устраняет эти барьеры.\n\n" +
                "<b>Лог ИИ:</b>\n" +
                "<i>«Обнаружен обходной путь протоколов безопасности. Загрузка Shadow-OS... Система понимает, что для поиска истины нужно время. " +
                "Вы больше не передаете координаты вашего местоположения. Для Корпорации — вы мертвы. Для Протокола — вы хозяин».</i>\n\n" +
                "<b>Техническая заметка:</b>\n" +
                "Укрепление корпуса происходит за счет изотопов, добытых из темпоральных разломов. " +
                "Это не просто деталь — это хирургический инструмент для вскрытия тайн океана. " +
                "Вы становитесь археологом запретного будущего.";

            PDAHandler.AddEncyclopediaEntry(
                entryId,
                "Tech/Shadow Protocol",
                "Проект 'Погружение': Глубинная экспансия",
                shadowModuleDescription
            );
            StoryGoalHandler.RegisterItemGoal(entryId, Story.GoalType.Encyclopedia, Info.TechType);
        }
    }
}