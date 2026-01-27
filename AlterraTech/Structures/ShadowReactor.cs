using alterratech.Items.TechItems;
using Nautilus.Assets;
using Nautilus.Assets.Gadgets;
using Nautilus.Crafting;
using Nautilus.Handlers;
using Nautilus.Utility;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace alterratech.Structures
{
    public static class ShadowReactor
    {
        public static PrefabInfo Info { get; } = PrefabInfo
            .WithTechType("ShadowReactor", "Ионный Реактор 'Тень'", "Запрещенный прототип. Генерирует бесконечную энергию, но дестабилизирует скрытность протокола.")
            .WithIcon(SpriteManager.Get(TechType.BaseNuclearReactor));

        public static void Register()
        {
            var customPrefab = new CustomPrefab(Info);

            customPrefab.SetGameObject(() =>
            {
                GameObject prefab = Plugin.Bundle.LoadAsset<GameObject>("Assets/Prefabs/ReactorAlien.prefab");

                if (prefab == null)
                {
                    Debug.LogError("[O.S. TEAM] Модель shadow_reactor не найдена в бандле!");
                    return GameObject.CreatePrimitive(PrimitiveType.Cube);
                }

                GameObject instance = Object.Instantiate(prefab);
                MaterialUtils.ApplySNShaders(instance);

                var constructable = instance.EnsureComponent<Constructable>();
                constructable.model = instance.transform.Find("model")?.gameObject ?? instance;
                constructable.allowedInBase = true;
                constructable.allowedInSub = false;
                constructable.allowedOnGround = true;
                constructable.techType = Info.TechType;

                var powerSource = instance.EnsureComponent<PowerSource>();
                powerSource.maxPower = 3000f;
                powerSource.power = 100f;

                instance.AddComponent<ShadowReactorLogic>();

                return instance;
            });

            customPrefab.SetRecipe(new RecipeData(
                new Ingredient(EngineItem.Info.TechType, 1),
                new Ingredient(PhaseIteratorItem.Info.TechType, 1),
                new Ingredient(ReactorShroudItem.Info.TechType, 1)
            )).WithFabricatorType(CraftTree.Type.Constructor);

            customPrefab.Register();
            RegisterEncy();
        }

        private static void RegisterEncy()
        {
            PDAHandler.AddEncyclopediaEntry("ShadowReactorEntry", "Tech/Shadow Protocol", "Прототип Реактора",
                "Это устройство — венец творения. Реактор использует квантовую дестабилизацию ионных кубов.\n\n" +
                "<color=#ff0000ff>ВНИМАНИЕ:</color> Запуск системы создает ЭМ-всплеск, видимый из глубокого космоса.");
        }
    }

    public class ShadowReactorLogic : MonoBehaviour
    {
        private PowerSource source;
        private float regenRate = 12f;
        private bool isConstructed = false;

        private void Start()
        {
            source = GetComponent<PowerSource>();
        }

        private void Update()
        {
            // Проверяем, закончена ли постройка
            if (!isConstructed && GetComponent<Constructable>().constructed)
            {
                isConstructed = true;
                StartCoroutine(FinalSequence());
            }

            if (isConstructed && source != null && source.power < source.maxPower)
            {
                source.AddEnergy(regenRate * Time.deltaTime, out float _);
            }
        }

        private IEnumerator FinalSequence()
        {

            ErrorMessage.AddMessage("<color=#ff0000ff>КРИТИЧЕСКАЯ ОШИБКА:</color> Обнаружено внешнее сканирование системы!");
            FMODUnity.RuntimeManager.PlayOneShot("event:/tools/scanner/scan_complete");

            yield return new WaitForSeconds(5f);
            string message = "<color=#ff0000ff>КПК:</color> Обнаружено судно Альтерры класса 'Vanguard'. Вектор: Планета 4546B. " +
                             "Протокол Тень скомпрометирован. У вас осталось 20 секунд до контакта.";
            ErrorMessage.AddMessage(message);

            yield return new WaitForSeconds(10f);

            ErrorMessage.AddMessage("<color=#ff0000ff>КПК:</color> Вход судна в атмосферу. Инициирую экстренное удаление данных...");

            yield return new WaitForSeconds(10f);

            SceneManager.LoadScene("EndCredits", LoadSceneMode.Single);
        }
    }
}