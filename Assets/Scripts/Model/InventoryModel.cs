using System.Collections.Generic;
using UnityEngine;
using Data;

namespace Model
{
    public class InventoryModel
    {
        private readonly List<InventoryItem> _slots;
        public IReadOnlyList<InventoryItem> Slots => _slots;

        public InventoryModel(InventoryConfig config)
        {
            // Инициализируем список слотов заданным количеством (может расширяться при добавлении)
            _slots = new List<InventoryItem>(config.defaultSlotCount);
            for (int i = 0; i < config.defaultSlotCount; i++)
                _slots.Add(null);
        }

        /// <summary>
        /// Добавляет предмет(ы) в инвентарь, заполняя сначала существующие стопки, затем открывая новые слоты.
        /// </summary>
        public bool AddItem(ItemDefinition def, int count = 1, bool isInjured = false)
        {
            // 1) Попытаться дозалить в уже существующие подходящие стопки
            for (int i = 0; i < _slots.Count && count > 0; i++)
            {
                var slot = _slots[i];
                if (slot != null
                    && slot.Definition == def
                    && slot.Count < def.maxStack
                    && slot.IsInjured == isInjured)
                {
                    int space = def.maxStack - slot.Count;
                    int toAdd = Mathf.Min(space, count);
                    slot.Count += toAdd;
                    count -= toAdd;
                }
            }

            // 2) Поместить в свободные слоты
            for (int i = 0; i < _slots.Count && count > 0; i++)
            {
                if (_slots[i] == null)
                {
                    int toAdd = Mathf.Min(def.maxStack, count);
                    _slots[i] = new InventoryItem(def, toAdd, isInjured);
                    count -= toAdd;
                }
            }
            return true;
        }
        public bool RemoveItem(int slotIndex, int count = 1)
        {
            if (slotIndex < 0 || slotIndex >= _slots.Count || _slots[slotIndex] == null)
                return false;

            var slot = _slots[slotIndex];
            slot.Count -= count;
            if (slot.Count <= 0)
                _slots[slotIndex] = null;

            return true;
        }

        /// <summary>
        /// Переключает состояние “ранен/здоров” для животных в слоте.
        /// </summary>
        public void ToggleInjuryState(int slotIndex)
        {
            if (slotIndex < 0 || slotIndex >= _slots.Count) return;
            var slot = _slots[slotIndex];
            if (slot != null && slot.Definition.itemType == ItemType.Animal)
                slot.IsInjured = !slot.IsInjured;
        }

        /// <summary>
        /// Меняет местами два слота без перемещения количества.
        /// </summary>
        public void SwapItems(int from, int to)
        {
            if (from < 0 || to < 0 || from >= _slots.Count || to >= _slots.Count) return;
            var tmp = _slots[from];
            _slots[from] = _slots[to];
            _slots[to] = tmp;
        }

        /// <summary>
        /// Делит стопку на две примерно равные части: половина остаётся, вторая создаётся в первом свободном слоте.
        /// </summary>
        public void SplitStack(int slotIndex)
        {
            if (slotIndex < 0 || slotIndex >= _slots.Count) return;
            var slot = _slots[slotIndex];
            if (slot == null || slot.Count < 2) return;

            int half = slot.Count / 2;
            slot.Count -= half;
            AddItem(slot.Definition, half, slot.IsInjured);
        }

        /// <summary>
        /// Объединяет стопку из слота from в слот to до заполнения maxStack.
        /// Если из слота from ничего не останется — он очищается.
        /// </summary>
        public void MergeStacks(int from, int to)
        {
            if (from < 0 || to < 0
                || from >= _slots.Count || to >= _slots.Count
                || from == to) return;

            var src = _slots[from];
            var dst = _slots[to];
            if (src == null || dst == null) return;
            if (src.Definition != dst.Definition || src.IsInjured != dst.IsInjured) return;

            int space = dst.Definition.maxStack - dst.Count;
            int toMove = Mathf.Min(space, src.Count);
            dst.Count += toMove;
            src.Count -= toMove;
            if (src.Count <= 0)
                _slots[from] = null;
        }

        // Методы для InventoryPersistence — устанавливают или очищают слот по индексу
        public void SetSlot(int index, InventoryItem item)
        {
            if (index < 0 || index >= _slots.Count) return;
            _slots[index] = item;
        }

        public void ClearSlot(int index)
        {
            if (index < 0 || index >= _slots.Count) return;
            _slots[index] = null;
        }
    }
}
