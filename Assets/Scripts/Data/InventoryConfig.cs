using UnityEngine;

namespace Data
{
    [CreateAssetMenu(menuName = "Inventory/Config")]
    public class InventoryConfig : ScriptableObject
    {
        [Tooltip("Сколько слотов по-умолчанию показывать в панели инвентаря")]
        public int defaultSlotCount = 20;
    }
}