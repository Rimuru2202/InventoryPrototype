using System.Collections.Generic;
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

    // Список индексов выделенных слотов
    public List<int> SelectedSlots { get; } = new List<int>();

    void Awake()
    {
        _model = new InventoryModel(config);
        InventoryPersistence.Load(_model, allDefinitions);
        view.Render();
    }

    void Update()
    {
        // Esc сбрасывает выделение
        if (Input.GetKeyDown(KeyCode.Escape))
            ClearSelection();
    }

    void OnApplicationQuit()
        => InventoryPersistence.Save(_model);

    /// <summary>
    /// Добавляет в инвентарь один случайный предмет (без привязки к выделенным слотам).
    /// </summary>
    public void AddRandomItem()
    {
        if (allDefinitions == null || allDefinitions.Length == 0)
            return;

        var def = allDefinitions[Random.Range(0, allDefinitions.Length)];
        _model.AddItem(def, 1, false);  // Передаем значения явно
        view.Render();
    }

    /// <summary>
    /// Пытается добавить +1 в единственный выделенный слот.
    /// Если слот пуст — положит туда случайный предмет.
    /// При переполнении стека — возвращает false и текст фидбека.
    /// </summary>
    public bool TryAddToSelected(out string feedback)
    {
        feedback = "";

        if (SelectedSlots.Count != 1)
            return false;

        int idx = SelectedSlots[0];
        var slot = _model.Slots[idx];

        if (slot == null)
        {
            // Пустой слот — кладём случайный
            if (allDefinitions == null || allDefinitions.Length == 0)
                return false;

            var def = allDefinitions[Random.Range(0, allDefinitions.Length)];
            _model.SetSlot(idx, new InventoryItem(def, 1, false));  // Передаем параметры явно
            view.Render();
            return true;
        }
        else
        {
            // Уже есть предмет
            if (slot.Count >= slot.Definition.maxStack)
            {
                feedback = "Превышено значение";
                return false;
            }

            slot.Count += 1;
            view.Render();
            return true;
        }
    }

    public void SetSingleSelection(int index)
    {
        SelectedSlots.Clear();
        SelectedSlots.Add(index);
        view.Render();
        detailView.Show(_model.Slots[index]);
    }

    public void ToggleSelectionAt(int index)
    {
        bool ctrl = Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl);

        if (!ctrl)
        {
            SetSingleSelection(index);
            return;
        }

        if (SelectedSlots.Contains(index))
            SelectedSlots.Remove(index);
        else
            SelectedSlots.Add(index);

        view.Render();
        if (SelectedSlots.Count == 1)
            detailView.Show(_model.Slots[SelectedSlots[0]]);
        else
            detailView.Hide();
    }

    public void ClearSelection()
    {
        SelectedSlots.Clear();
        view.Render();
        detailView.Hide();
    }

    public void RemoveSelected()
    {
        foreach (int idx in SelectedSlots)
        {
            _model.RemoveItem(idx);
        }

        // Убираем из выделения все пустые слоты
        SelectedSlots.RemoveAll(idx => _model.Slots[idx] == null);

        // Если ничего не осталось выделено — полностью скрыть детали
        if (SelectedSlots.Count == 0)
            detailView.Hide();
        else if (SelectedSlots.Count == 1)
            detailView.Show(_model.Slots[SelectedSlots[0]]);
        else
            detailView.Hide();

        view.Render();
    }

    public void SplitSelected()
    {
        if (SelectedSlots.Count != 1)
            return;

        int idx = SelectedSlots[0];
        var slot = _model.Slots[idx];
        if (slot == null || slot.Count < 2)
            return;

        int half = slot.Count / 2;
        slot.Count -= half;

        // Найти первый пустой слот
        for (int i = 0; i < _model.Slots.Count; i++)
        {
            if (_model.Slots[i] == null)
            {
                _model.SetSlot(i, new InventoryItem(slot.Definition, half, slot.IsInjured));  // Передаем все параметры
                break;
            }
        }

        view.Render();
        detailView.Show(_model.Slots[idx]);
    }

    public void ToggleSelected()
    {
        if (SelectedSlots.Count != 1)
            return;

        int idx = SelectedSlots[0];
        var slot = _model.Slots[idx];
        
        if (slot == null || slot.Definition.itemType != ItemType.Animal)
            return;
        
        slot.IsInjured = !slot.IsInjured;
        
        view.Render();
        detailView.Show(slot);
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
}
