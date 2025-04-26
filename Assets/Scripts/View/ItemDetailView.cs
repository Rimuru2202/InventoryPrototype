using UnityEngine;
using TMPro;
using Model;
using DG.Tweening;
using System.Collections;

namespace View
{
    public class ItemDetailView : MonoBehaviour
    {
        [Header("UI Elements")]
        public TMP_Text nameText;
        public TMP_Text descText;
        public CanvasGroup canvasGroup;
        public RectTransform panelRect; // RectTransform панели (привязать в инспекторе)

        private Coroutine _showRoutine;

        /// <summary>
        /// Показать панель описания предмета с анимацией.
        /// </summary>
        public void Show(InventoryItem item)
        {
            if (item == null)
            {
                Hide();
                return;
            }

            // Останавливаем предыдущую анимацию
            if (_showRoutine != null)
                StopCoroutine(_showRoutine);

            _showRoutine = StartCoroutine(CoShow(item));
        }

        private IEnumerator CoShow(InventoryItem item)
        {
            // Устанавливаем текст
            nameText.text = item.Definition.itemName;
            descText.text = item.Definition.description;

            // Начальное состояние: панель за левым краем экрана, прозрачность 0
            Vector2 offScreen = new Vector2(-panelRect.rect.width, panelRect.anchoredPosition.y);
            panelRect.anchoredPosition = offScreen;
            canvasGroup.alpha = 0f;

            // Параллельный слайд + fade-in
            panelRect.DOAnchorPosX(0f, 0.5f).SetEase(Ease.OutCubic);
            canvasGroup.DOFade(1f, 0.5f);

            // Ждём 2 секунды
            yield return new WaitForSeconds(2f);

            // Параллельный слайд обратно + fade-out
            panelRect.DOAnchorPosX(offScreen.x, 0.5f).SetEase(Ease.InCubic);
            yield return canvasGroup.DOFade(0f, 0.5f).WaitForCompletion();
        }

        /// <summary>
        /// Скрыть панель без анимации.
        /// </summary>
        public void Hide()
        {
            if (_showRoutine != null)
                StopCoroutine(_showRoutine);

            canvasGroup.alpha = 0f;
            // Возвращаем панель за экран
            panelRect.anchoredPosition = new Vector2(-panelRect.rect.width, panelRect.anchoredPosition.y);
        }
    }
}
