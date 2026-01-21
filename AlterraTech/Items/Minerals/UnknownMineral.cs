using Nautilus.Assets;
using Nautilus.Assets.Gadgets;
using Nautilus.Assets.PrefabTemplates;
using Nautilus.Crafting;
using Nautilus.Handlers;
using Nautilus.Utility;
using System.IO;
using System.Reflection;
using UnityEngine;

namespace alterratech.Items.Minerals
{
    public static class UnknownMinerales
    {
        public static PrefabInfo Info { get; private set; }
        public static string ModPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

        public static void Register()
        {

            string iconPath = Path.Combine(ModPath, "Assets", "UnknownMineralIcon.png");

            Info = PrefabInfo.WithTechType("UnknownMineral", "Неизвестный Минерал", "Материал неизвестного происхождения.")
                .WithIcon(ImageUtils.LoadSpriteFromFile(iconPath));

            var customPrefab = new CustomPrefab(Info);
            var mineralClone = new CloneTemplate(Info, TechType.Nickel);


            mineralClone.ModifyPrefab += obj =>
            {
                string texturePath = Path.Combine(ModPath, "Assets", "UnknownMineral_diffuse.png");

                if (File.Exists(texturePath))
                {
                    var renderer = obj.GetComponentInChildren<Renderer>();
                    if (renderer != null)
                    {
                        Texture2D texture = ImageUtils.LoadTextureFromFile(texturePath);
                        renderer.material.mainTexture = texture;
                    }
                }
            };
            var recipe = new RecipeData(
                new Ingredient(TechType.Gold, 2),
                new Ingredient(TechType.Titanium, 3),
                new Ingredient(TechType.PrecursorIonCrystal, 1)
            );
            customPrefab.SetRecipe(recipe).WithFabricatorType(CraftTree.Type.Fabricator);
            customPrefab.SetGameObject(mineralClone);
            customPrefab.SetSpawns(
                new LootDistributionData.BiomeData { biome = BiomeType.SafeShallows_Grass, count = 1, probability = 0.5f },
                new LootDistributionData.BiomeData { biome = BiomeType.GrassyPlateaus_CaveFloor, count = 1, probability = 0.5f }
            );
            customPrefab.SetEquipment(EquipmentType.None);
            customPrefab.SetUnlock(TechType.Knife);
            EncyPda();
            customPrefab.Register();
        }
        private static void EncyPda()
        {
            LanguageHandler.SetLanguageLine("EncyPath_Tech/Shadow Protocol", "PROJECT SHADOW");

            string UnkMineral = "UnknownMineral";
            string UnkMineralDisc =
                "<color=#888888ff>Объект: Неизвестный Минерал (Синтетическая аномалия)</color>\n\n" +
                "Этот ресурс не является природным образованием. Анализ показывает, что это <b>высокостабильный метаматериал</b>, созданный путем молекулярного слияния благородных металлов и инопланетных ионных структур.\n\n" +
                "<b>Свойства:</b>\n" +
                "• <b>Пространственное сжатие:</b> Минерал создает локальное поле, которое позволяет 'уплотнять' молекулы газов вокруг себя (используется в Теневых баллонах).\n" +
                "• <b>Искусственная решетка:</b> Структура материала была получена путем воздействия сверхвысоких температур на титаново-золотую основу в присутствии ионного излучателя.\n" +
                "• <b>Стабильность:</b> В отличие от природных кристаллов предтеч, этот минерал не истощается со временем, сохраняя постоянный энергетический фон.\n\n" +
                "<color=#ffff00ff>ЗАМЕТКА ИИ:</color>\n" +
                "<i>«Архивы указывают, что синтез такого вещества теоретически возможен, но требует катализатора, который был утерян тысячи лет назад. Тот факт, что вы держите его в руках, подтверждает: Теневой Протокол — это не просто данные, это рабочая технология».</i>";

            PDAHandler.AddEncyclopediaEntry(UnkMineral, "Tech/Shadow Protocol", "Старый Корабль", UnkMineralDisc);
            StoryGoalHandler.RegisterItemGoal(UnkMineral, Story.GoalType.Encyclopedia, Info.TechType);
        }
    }
}