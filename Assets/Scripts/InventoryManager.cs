using UnityEngine;
using Data;
using Model;
using View;

public class InventoryManager : MonoBehaviour
{
    [Header("Конфиг и все определения")]
    public InventoryConfig config;
    public ItemDefinition[] allDefinitions;

    [Header("Связанные компоненты")]
    [SerializeField] private InventoryView view;
    [SerializeField] private ItemDetailView detailView;

    public InventoryView View => view;
    public ItemDetailView DetailView => detailView;

    private InventoryModel _model;
    public InventoryModel Model => _model;

    void Awake()
    {
        _model = new InventoryModel(config);
        InventoryPersistence.Load(_model, allDefinitions);
        view.Render(); // сразу отрисовать подгруженное
    }

    void OnApplicationQuit()
        => InventoryPersistence.Save(_model);

    public void AddItem(ItemDefinition def, bool injured = false)
    {
        _model.AddItem(def, isInjured: injured);
        view.Render();
    }

    public void RemoveItem(int slotIndex)
    {
        _model.RemoveItem(slotIndex);
        view.Render();
        detailView.Hide();
    }

    public void ToggleState(int slotIndex)
    {
        _model.ToggleInjuryState(slotIndex);
        view.Render();
    }

    public void SwapItems(int from, int to)
    {
        _model.SwapItems(from, to);
        view.Render();
    }

    public void MergeStacks(int from, int to)
    {
        _model.MergeStacks(from, to);
        view.Render();
    }

    public void SplitStack(int slotIndex)
    {
        _model.SplitStack(slotIndex);
        view.Render();
    }
}