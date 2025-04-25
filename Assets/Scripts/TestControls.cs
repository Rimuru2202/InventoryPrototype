using UnityEngine;
using Model;
using View;

public class TestControls : MonoBehaviour
{
    [SerializeField] private InventoryManager manager;
    [SerializeField] private InventoryView view;
    [SerializeField] private int testSlotIndex;

    public void OnAddRandom()
    {
        var defs = manager.allDefinitions;
        var def  = defs[Random.Range(0, defs.Length)];
        bool injured = def.itemType == ItemType.Animal && Random.value > 0.5f;
        manager.AddItem(def, injured);
    }

    public void OnRemove()
    {
        manager.RemoveItem(testSlotIndex);
    }

    public void OnToggleState()
    {
        manager.ToggleState(testSlotIndex);
    }

    public void OnSplit()
    {
        manager.SplitStack(testSlotIndex);
    }
}