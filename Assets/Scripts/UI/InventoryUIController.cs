using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Model;
using DG.Tweening;

namespace UI
{
    public class InventoryUIController : MonoBehaviour
    {
        [Header("Inventory Manager")]
        [SerializeField] private InventoryManager manager;

        [Header("Buttons")]
        [SerializeField] private Button removeBtn;
        [SerializeField] private Button splitBtn;
        [SerializeField] private Button toggleBtn;
        [SerializeField] private Button addBtn;

        [Header("Feedback Text")]
        [SerializeField] private TMP_Text feedbackText;

        private void Start()
        {
            removeBtn.onClick.AddListener(manager.RemoveSelected);
            splitBtn.onClick.AddListener(manager.SplitSelected);
            toggleBtn.onClick.AddListener(manager.ToggleSelected);
            addBtn.onClick.AddListener(OnAddClicked);

            feedbackText.text = "";
        }

        private void Update()
        {
            var sel = manager.SelectedSlots;
            int count = sel.Count;

            // "Удалить" — активна, если выделено ≥1 и все не пусты
            removeBtn.interactable = count > 0 && sel.All(i => manager.Model.Slots[i] != null);

            // "Разделить" — ровно 1 и Count ≥2
            splitBtn.interactable = count == 1
                && manager.Model.Slots[sel[0]] != null
                && manager.Model.Slots[sel[0]].Count >= 2;

            // "Изменить состояние" — ровно 1 и это животное
            toggleBtn.interactable = count == 1
                && manager.Model.Slots[sel[0]] != null
                && manager.Model.Slots[sel[0]].Definition.itemType == ItemType.Animal;

            // "Добавить" — всегда, кроме случая одного выбранного полного слота
            bool addAllowed = true;
            if (count == 1)
            {
                var slot = manager.Model.Slots[sel[0]];
                if (slot != null && slot.Count >= slot.Definition.maxStack)
                    addAllowed = false;
            }
            addBtn.interactable = addAllowed;

            // Сброс фидбека, если снова можно добавлять
            if (addAllowed && feedbackText.text != "")
                feedbackText.text = "";
        }

        private void OnAddClicked()
        {
            var sel = manager.SelectedSlots;
            if (sel.Count == 1)
            {
                if (!manager.TryAddToSelected(out string fb))
                {
                    // Выводим фидбек и трясём текст
                    feedbackText.text = fb;
                    feedbackText.rectTransform
                        .DOPunchPosition(Vector3.right * 10f, 0.5f)  // Убраны параметры vibrato и elasticity
                        .SetEase(Ease.OutQuad);
                }
                else
                {
                    feedbackText.text = "";
                }
            }
            else
            {
                manager.AddRandomItem();
                feedbackText.text = "";
            }
        }
    }
}
