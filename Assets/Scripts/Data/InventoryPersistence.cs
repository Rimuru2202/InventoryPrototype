using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Model;

namespace Data
{
    public static class InventoryPersistence
    {
        private const string Key = "PlayerInventory";

        [System.Serializable]
        public class SlotData
        {
            public string definitionId;
            public int count;
            public bool isInjured;
        }

        [System.Serializable]
        public class InventoryData
        {
            public List<SlotData> slots;
        }

        public static void Save(InventoryModel model)
        {
            var data = new InventoryData {
                slots = model.Slots.Select(s =>
                    s != null
                        ? new SlotData {
                            definitionId = s.Definition.id,
                            count = s.Count,
                            isInjured = s.IsInjured
                        }
                        : null
                ).ToList()
            };
            PlayerPrefs.SetString(Key, JsonUtility.ToJson(data));
            PlayerPrefs.Save();
        }

        public static void Load(InventoryModel model, ItemDefinition[] defs)
        {
            if (!PlayerPrefs.HasKey(Key)) return;
            var data = JsonUtility.FromJson<InventoryData>(
                PlayerPrefs.GetString(Key)
            );

            for (int i = 0; i < data.slots.Count; i++)
            {
                var sd = data.slots[i];
                if (sd == null)
                {
                    model.ClearSlot(i);
                    continue;
                }
                var def = defs.FirstOrDefault(d => d.id == sd.definitionId);
                if (def != null)
                    model.SetSlot(i, new InventoryItem(def, sd.count, sd.isInjured));
            }
        }
    }
}