using System.Collections.Generic;
using alterratech.Items.Minerals;
using Nautilus.Assets;
using Nautilus.Assets.Gadgets;
using Nautilus.Assets.PrefabTemplates;
using Nautilus.Crafting;
using Nautilus.Handlers;
using Nautilus.Utility;
using UnityEngine;

namespace alterratech.Items.Equipment
{
    public static class ShadowTank
    {
        public static PrefabInfo Info { get; } = PrefabInfo
            .WithTechType("ShadowTank", "Теневой Баллон", "Максимальная эффективность.")
            .WithIcon(SpriteManager.Get(TechType.HighCapacityTank));

        public static void Register()
        {
            var customPrefab = new CustomPrefab(Info);
            var clone = new CloneTemplate(Info, TechType.HighCapacityTank);

            clone.ModifyPrefab += obj =>
            {
                obj.AddComponent<ShadowTankLogic>();

                var oxygen = obj.GetComponent<Oxygen>();
                if (oxygen != null)
                {
                    oxygen.oxygenCapacity = 160f;
                }

                var renderers = obj.GetComponentsInChildren<Renderer>(true);
                foreach (var r in renderers)
                {
                    foreach (var m in r.materials)
                    {
                        m.color = new Color(0.02f, 0.02f, 0.02f);
                    }
                }
            };

            customPrefab.SetGameObject(clone);

            customPrefab.SetRecipe(new RecipeData(
                new Ingredient(TechType.HighCapacityTank, 1),
                new Ingredient(UnknownMinerales.Info.TechType, 2)
            )).WithFabricatorType(CraftTree.Type.Workbench);

            customPrefab.SetEquipment(EquipmentType.Tank);

            RegisterEncyclopedia();

            customPrefab.Register();
        }

        private static void RegisterEncyclopedia()
        {
            LanguageHandler.SetLanguageLine("EncyPath_Tech/Shadow Protocol", "PROJECT SHADOW");

            string encyclopediaKey = "ShadowBallon_Info_Page";

            string shadowDescription =
                    "<color=#555555ff>Объект: Теневой Баллон (Shadow Air Tank)</color>\n\n" +
                    "Сверхтехнологичное решение для длительных автономных погружений. " +
                    "Разработка основана на интеграции <b>Неизвестного Минерала</b> в структуру внутренней оболочки высокого давления.\n\n" +
                    "<b>Технические характеристики:</b>\n" +
                    "• <b>Объем:</b> Увеличенная емкость за счет молекулярного сжатия газовой смеси.\n" +
                    "• <b>Материалы:</b> <color=#90000f> ERROR: DATA CORRUPTED </color>\n" +
                    "• <b>Автономность:</b> Обеспечивает +160 единиц кислорода к базовому запасу.\n\n" +
                    "<b>Анализ:</b>\n" +
                    "Устройство превосходит все гражданские аналоги Альтерры по показателям плотности хранения кислорода. " +
                    "Использование в официальных экспедициях запрещено из-за непредсказуемости влияния излучения минерала на состав смеси.\n\n" +
                    "<i>«Максимальная эффективность без лишних слов».</i>";

            PDAHandler.AddEncyclopediaEntry(
                encyclopediaKey,
                "Tech/Shadow Protocol",
                "Теневой Баллон",
                shadowDescription
            );

            StoryGoalHandler.RegisterItemGoal(encyclopediaKey, Story.GoalType.Encyclopedia, Info.TechType);
        }
    }

    public class ShadowTankLogic : MonoBehaviour
    {
        void Update()
        {
        }
    }
}