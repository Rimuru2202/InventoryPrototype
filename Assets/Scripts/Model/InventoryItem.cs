namespace Model
{
    public class InventoryItem
    {
        public ItemDefinition Definition { get; private set; }
        public int Count { get; set; }
        public bool IsInjured { get; set; }

        public InventoryItem(ItemDefinition def, int count, bool isInjured = false)
        {
            Definition = def;
            Count = count;
            IsInjured = isInjured;
        }
    }
}