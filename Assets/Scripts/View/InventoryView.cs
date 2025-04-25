using UnityEngine;
using System.Collections.Generic;

namespace View
{
    public class InventoryView : MonoBehaviour
    {
        [SerializeField] private InventoryManager manager;
        [SerializeField] private GameObject slotPrefab;

        private List<InventorySlotView> _slotViews = new List<InventorySlotView>();
        private Transform _dragCanvas;

        void Start()
        {
            _dragCanvas = GetComponentInParent<Canvas>().transform;
            InitializeSlots();
            Render();
        }

        void InitializeSlots()
        {
            var slots = manager.Model.Slots;
            for (int i = 0; i < slots.Count; i++)
            {
                var go = Instantiate(slotPrefab, transform);
                var view = go.GetComponent<InventorySlotView>();
                view.Initialize(manager, i, _dragCanvas);
                _slotViews.Add(view);
            }
        }

        public void Render()
        {
            var slots = manager.Model.Slots;
            for (int i = 0; i < _slotViews.Count; i++)
                _slotViews[i].UpdateView(slots[i]);
        }
    }
}