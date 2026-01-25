using System.Collections.Generic;
using Nautilus.Assets;
using Nautilus.Assets.Gadgets;
using Nautilus.Crafting;
using Nautilus.Handlers;
using UnityEngine;
using alterratech.Items.TechItems;
// Добавляем этот неймспейс для работы со звуком
using FMODUnity;

namespace alterratech.Items.Equipment
{
    public static class ShadowImpulseRifle
    {
        public static PrefabInfo Info { get; } = PrefabInfo
            .WithTechType("ShadowImpulseRifle", "Импульсная винтовка SHADOW", "Адаптированное инопланетное оружие. Использует магнитный импульс для поражения целей.")
            .WithIcon(Sprite.Create(Texture2D.whiteTexture, new Rect(0, 0, 1, 1), Vector2.zero));

        public static void Register()
        {
            var customPrefab = new CustomPrefab(Info);

            customPrefab.SetGameObject(() =>
            {
                GameObject prefab = Object.Instantiate(Resources.Load<GameObject>("WorldEntities/Doodads/Precursor/Precursor_Rifle"));

                prefab.AddComponent<PrefabIdentifier>().ClassId = Info.ClassID;
                prefab.AddComponent<TechTag>().type = Info.TechType;
                prefab.AddComponent<Pickupable>();

                var tool = prefab.AddComponent<ImpulseRifleTool>();

                var energyMixin = prefab.AddComponent<EnergyMixin>();
                var storage = prefab.EnsureComponent<ChildObjectIdentifier>();
                storage.classId = "BatterySlot";
                energyMixin.storageRoot = storage;
                energyMixin.compatibleBatteries = new List<TechType> { TechType.Battery, TechType.PrecursorIonBattery };

                return prefab;
            });

            customPrefab.SetRecipe(new RecipeData(
                new Ingredient(EngineItem.Info.TechType, 1),
                new Ingredient(TechType.PrecursorIonCrystal, 1),
                new Ingredient(TechType.AdvancedWiringKit, 1),
                new Ingredient(TechType.Magnetite, 2)
            )).WithFabricatorType(CraftTree.Type.Workbench);

            customPrefab.SetEquipment(EquipmentType.Hand);

            // Регистрация сканера (привязано к Синему ключу Предтеч)
            PDAHandler.AddCustomScannerEntry(TechType.PrecursorKey_Blue, Info.TechType, false, 1, 3f, false, "ShadowRifleEntry");

            customPrefab.Register();
        }
    }

    public class ImpulseRifleTool : PlayerTool
    {
        public float damage = 40f;
        public float fireRate = 0.25f;
        public float range = 60f;
        private float nextFireTime;

        public override bool OnRightHandDown()
        {
            if (Time.time > nextFireTime && CanFire())
            {
                Fire();
                return true;
            }
            return false;
        }

        private bool CanFire()
        {
            return this.energyMixin != null && this.energyMixin.charge >= 1.5f;
        }

        private void Fire()
        {
            nextFireTime = Time.time + fireRate;
            this.energyMixin.ConsumeEnergy(1.5f);

            // ИСПРАВЛЕНИЕ: Используем RuntimeManager для проигрывания звука по строковому пути
            RuntimeManager.PlayOneShot("event:/tools/stasis_rifle/fire", transform.position);

            Vector3 aimStart = MainCamera.camera.transform.position;
            Vector3 aimDir = MainCamera.camera.transform.forward;

            if (Physics.Raycast(aimStart, aimDir, out RaycastHit hit, range))
            {
                GameObject target = hit.collider.gameObject;
                LiveMixin liveMixin = target.GetComponentInParent<LiveMixin>();
                if (liveMixin != null)
                {
                    liveMixin.TakeDamage(damage, hit.point, DamageType.Normal, null);
                }

                SpawnHitEffect(hit.point);
            }

            var arms = Player.main.GetComponent<ArmsController>();
            if (arms != null && arms.animator != null)
            {
                SafeAnimator.SetTrigger(arms.animator, "stasis_rifle_fire");
            }
        }

        private void SpawnHitEffect(Vector3 position)
        {
            GameObject hitVFX = Resources.Load<GameObject>("vfx/StasisRifleHit");
            if (hitVFX != null)
            {
                // Здесь Utils.PlayOneShotPS работает верно, так как hitVFX — это GameObject
                Utils.PlayOneShotPS(hitVFX, position, Quaternion.identity);
            }
        }
    }
}