using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using Model;
using DG.Tweening;

namespace View
{
    public class InventorySlotView : MonoBehaviour,
        IBeginDragHandler, IDragHandler, IEndDragHandler, IDropHandler, IPointerClickHandler
    {
        [Header("UI-элементы слота")]
        public Image backgroundImage;
        public Image iconImage;
        public TMP_Text countText;
        public GameObject injuredOverlay;

        [HideInInspector]
        public int slotIndex;

        private InventoryItem _item;
        private InventoryManager _manager;
        private Transform _canvas;
        private GameObject _dragGo;

        private static readonly Color NormalColor = new Color32(0x80, 0x80, 0x80, 0xFF);
        private static readonly Color SelectedColor = Color.green;

        public void Initialize(InventoryManager manager, int index, Transform dragCanvas)
        {
            _manager = manager;
            slotIndex = index;
            _canvas = dragCanvas;

            if (iconImage) iconImage.raycastTarget = false;
            if (countText) countText.raycastTarget = false;
            if (injuredOverlay)
            {
                var img = injuredOverlay.GetComponent<Image>();
                if (img) img.raycastTarget = false;
            }
        }

        public void UpdateView(InventoryItem item)
        {
            _item = item;
            bool isSelected = _manager.SelectedSlots.Contains(slotIndex);
            backgroundImage.color = isSelected ? SelectedColor : NormalColor;

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

        public void OnPointerClick(PointerEventData eventData)
        {
            _manager.ToggleSelectionAt(slotIndex);

            transform.DOKill();
            transform.localScale = Vector3.one;
            transform.DOPunchScale(Vector3.one * 0.2f, 0.4f, 1, 0.5f)
                .SetEase(Ease.OutBack);
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            // Нельзя начать перетаскивание, если слот пустой
            if (_item == null || _item.Count <= 0)
                return;

            // Создаём копию иконки и сразу привязываем к канвасу
            _dragGo = Instantiate(iconImage.gameObject, _canvas, false);
            var dragImage = _dragGo.GetComponent<Image>();
            dragImage.raycastTarget = false;

            // Настраиваем RectTransform копии
            var srcRT = iconImage.rectTransform;
            var rt = _dragGo.GetComponent<RectTransform>();
            rt.anchorMin = rt.anchorMax = new Vector2(0.5f, 0.5f);
            rt.pivot = new Vector2(0.5f, 0.5f);
            rt.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, srcRT.rect.width);
            rt.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, srcRT.rect.height);
            rt.localScale = Vector3.one;

            UpdateDragPosition(eventData);
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (_dragGo == null) return;
            UpdateDragPosition(eventData);
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            if (_dragGo != null)
                Destroy(_dragGo);

            _manager.View.Render();
        }

        public void OnDrop(PointerEventData eventData)
        {
            var srcView = eventData.pointerDrag?.GetComponent<InventorySlotView>();
            if (srcView == null || srcView == this) return;

            var srcItem = srcView._item;
            var dstItem = _item;

            if (srcItem == null || srcItem.Count <= 0)
                return;

            bool sameDef = dstItem != null && srcItem.Definition == dstItem.Definition;
            bool sameInjured = sameDef && srcItem.IsInjured == dstItem.IsInjured;
            bool canMerge = sameDef && sameInjured && dstItem.Count < dstItem.Definition.maxStack;

            if (canMerge)
            {
                _manager.MergeStacks(srcView.slotIndex, slotIndex);
            }
            else if (dstItem == null)
            {
                // Перенос предмета в пустой слот: переезжает полностью
                _manager.Model.SetSlot(slotIndex, srcItem);
                _manager.Model.SetSlot(srcView.slotIndex, null);
            }
            else
            {
                _manager.SwapItems(srcView.slotIndex, slotIndex);
            }

            _manager.View.Render();
        }


        private void UpdateDragPosition(PointerEventData eventData)
        {
            if (_dragGo == null) return;

            var rt = _dragGo.GetComponent<RectTransform>();
            var canvasRect = _canvas.GetComponent<RectTransform>();
            var canvasComp = _canvas.GetComponentInParent<Canvas>();
            Camera cam = canvasComp.renderMode == RenderMode.ScreenSpaceOverlay ? null : canvasComp.worldCamera;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                canvasRect, eventData.position, cam, out Vector2 localPoint
            );
            rt.anchoredPosition = localPoint;
        }
    }
}
