using System.IO;
using System.Reflection;
using UnityEngine;
using EpicStructureLoader; // Обязательно добавьте EpicStructureLoader.dll в References проекта!

namespace alterratech
{
    public static class StructureLoaders
    {
        public static void LoadStructures()
        {
            string modPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            string structuresFolder = Path.Combine(modPath, "Structures");

            if (Directory.Exists(structuresFolder))
            {
                string[] files = Directory.GetFiles(structuresFolder, "*.structure");

                foreach (string file in files)
                {
                    // 1. Создаем обязательную переменную
                    int entityCount = 0;

                    // 2. Передаем её через ref
                    StructureLoading.LoadAndRegisterStructureAtPath(file, ref entityCount);

                    Debug.Log($"[AlterraTech] Загружен файл: {Path.GetFileName(file)} (Заспавнено объектов: {entityCount})");
                }
            }
        }
    }
}