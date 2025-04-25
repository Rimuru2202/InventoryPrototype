using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using Model;

namespace View
{
    public class InventorySlotView : MonoBehaviour,
        IBeginDragHandler, IDragHandler, IEndDragHandler, IDropHandler, IPointerClickHandler
    {
        public Image iconImage;
        public TMP_Text countText;
        public GameObject injuredOverlay;
        [HideInInspector] public int slotIndex;

        private InventoryItem _item;
        private InventoryManager _manager;
        private Transform _canvas;
        private Image _dragIcon;

        public void Initialize(InventoryManager manager, int index, Transform dragCanvas)
        {
            _manager = manager;
            slotIndex = index;
            _canvas = dragCanvas;
        }

        public void UpdateView(InventoryItem item)
        {
            _item = item;
            if (item == null)
            {
                iconImage.enabled = false;
                countText.text = "";
                injuredOverlay.SetActive(false);
                return;
            }
            iconImage.enabled = true;
            iconImage.sprite = item.Definition.icon;
            countText.text = item.Count > 1 ? item.Count.ToString() : "";
            injuredOverlay.SetActive(
                item.Definition.itemType == ItemType.Animal && item.IsInjured
            );
        }

        public void OnPointerClick(PointerEventData e)
            => _manager.DetailView.Show(_item);

        public void OnBeginDrag(PointerEventData e)
        {
            if (_item == null) return;
            _dragIcon = Instantiate(iconImage, _canvas);
            _dragIcon.raycastTarget = false;
        }

        public void OnDrag(PointerEventData e)
        {
            if (_dragIcon != null)
                _dragIcon.transform.position = e.position;
        }

        public void OnEndDrag(PointerEventData e)
        {
            if (_dragIcon != null) Destroy(_dragIcon.gameObject);
        }

        public void OnDrop(PointerEventData e)
        {
            var src = e.pointerDrag?.GetComponent<InventorySlotView>();
            if (src == null || src.slotIndex == slotIndex) return;

            // если на тот же тип + состояние — слипать, иначе — меняться
            if (_item != null
                && _item.Definition == src._item.Definition
                && _item.IsInjured == src._item.IsInjured)
            {
                _manager.MergeStacks(src.slotIndex, slotIndex);
            }
            else
            {
                _manager.SwapItems(src.slotIndex, slotIndex);
            }
        }
    }
}
