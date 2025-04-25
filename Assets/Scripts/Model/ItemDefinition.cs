using UnityEngine;

namespace Model
{
    [CreateAssetMenu(menuName = "Inventory/ItemDefinition")]
    public class ItemDefinition : ScriptableObject
    {
        public string id;
        public ItemType itemType;
        public Sprite icon;
        public int maxStack;
        public string itemName;

        [TextArea(2,5)]
        public string description;
    }
}