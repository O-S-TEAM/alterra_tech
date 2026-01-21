using alterratech.Items.Equipment;
using alterratech.Items.modules;
using Nautilus.Assets;
using Nautilus.Handlers;
using Nautilus.Utility;
using Story;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Runtime.Remoting.Metadata.W3cXsd2001;
using UnityEngine;

namespace alterratech.Structures
{
    public static class StaticStructures
    {
        private static int scannedServersCount = 0;
        private const int TotalServersRequired = 3;

        public static PrefabInfo TitanicInfo { get; } = PrefabInfo
            .WithTechType("TitanicStructure", "Titanic", "Огромный обломок древнего судна.");

        public static PrefabInfo ServerInfo { get; } = PrefabInfo
            .WithTechType("ServerStructure", "Server", "Старый Сервер (Shadow Protocol)");

        public static void Register()
        {
            var titanicPrefab = CreateBasePrefab(TitanicInfo, "Assets/Prefabs/titanic.prefab");
            var serverPrefab = CreateBasePrefab(ServerInfo, "Assets/Prefabs/serverV3.prefab");

            serverPrefab.Register();
            titanicPrefab.Register();

            EncyPda();
            Debug.LogError("AlterraTech: PDA записи загружены");

            CoordinatedSpawnsHandler.RegisterCoordinatedSpawn(new SpawnInfo(TitanicInfo.TechType, new Vector3(-1745f, -420f, 0f)));
            Debug.LogError("AlterraTech: Структуры добавлены в мир");
        }

        private static void EncyPda()
        {
            LanguageHandler.SetLanguageLine("EncyPath_Tech/Shadow Protocol", "PROJECT SHADOW");

            string titanicEncy = "Titanic";
            string titanicDesc = "Запись: Объект \"Титаник\" — Находка на глубине\n" +
                "\"Данное судно не числится в реестрах 'Альтерры'. Корпус состоит из примитивных сплавов железа, которые должны были сгнить, но структура нетронута.\n" +
                "Сонар фиксирует странные пустоты. Похоже, корабль буквально 'выпал' из временного разлома...\"\n\n" +
                "Восстановленный лог: 14 апреля...\n" +
                "\"Мы видели лед, но теперь вокруг только бесконечная толща воды. Что-то бьется в обшивку с той стороны. " +
                "Если кто-то найдет эту запись... знайте, 'Титаник' не утонул. Он просто сменил океан.\"";

            PDAHandler.AddEncyclopediaEntry(titanicEncy, "Tech/Shadow Protocol", "Старый Корабль", titanicDesc);

            string serverEncy = "ServerKey";
            string serverDescription =
                "<b>Объект: Автономный серверный узел (Архитектура начала XXI века)</b>\n\n" +
                "В эпоху квантовых процессоров этот аппарат выглядит как механический динозавр. " +
                "Вместо био-чипов здесь используются примитивные <b>кремниевые платы</b> и медные проводники.\n\n" +
                "<b>Технические артефакты:</b>\n" +
                "• <b>Механические накопители:</b> Данные на вращающихся магнитных дисках.\n" +
                "• <b>Охлаждение:</b> Лопастные вентиляторы. Устройство прогоняло через себя воздух.\n" +
                "• <b>Архитектура:</b> Маркировка напоминает древние серии 'Ryzen'.\n\n" +
                "< b > Данные:</ b > Внутри обнаружены данные чертежей 'Альтерры'.\n\n" +
                "<color=#ffff00ff>ЗАМЕТКА ИИ:</color>\n" +
                "<i>«Внутри Сервера до сканирования была странная энергия...».</i>";

            PDAHandler.AddEncyclopediaEntry(serverEncy, "Tech/Shadow Protocol", "Старый Сервер", serverDescription);

            PDAHandler.AddCustomScannerEntry(TitanicInfo.TechType, 2f, false, titanicEncy);
            PDAHandler.AddCustomScannerEntry(ServerInfo.TechType, ShadowTank.Info.TechType, true, 3, 2f, true, serverEncy);
            PDAHandler.AddCustomScannerEntry(ServerInfo.TechType, SeamothShadowModule.Info.TechType, true, 4, 2f, true);
        }

        private static CustomPrefab CreateBasePrefab(PrefabInfo info, string assetPath)
        {
            var customPrefab = new CustomPrefab(info);
            customPrefab.SetGameObject(() =>
            {
                GameObject prefab = Plugin.Bundle.LoadAsset<GameObject>(assetPath);
                if (prefab == null) return new GameObject("Empty (Bundle Error)");

                GameObject instance = Object.Instantiate(prefab);
                MaterialUtils.ApplySNShaders(instance);
                instance.AddComponent<PrefabIdentifier>().ClassId = info.ClassID;

                var lwe = instance.EnsureComponent<LargeWorldEntity>();
                lwe.cellLevel = LargeWorldEntity.CellLevel.Global;

                var rb = instance.GetComponent<Rigidbody>();
                if (rb != null) rb.isKinematic = true;

                return instance;
            });
            return customPrefab;
        }
    }
}