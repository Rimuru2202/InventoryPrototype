using UnityEngine;
using TMPro;
using Model;
using System.Collections;

namespace View
{
    public class ItemDetailView : MonoBehaviour
    {
        public TMP_Text nameText;
        public TMP_Text descText;
        public CanvasGroup canvasGroup;

        private Coroutine _fadeRoutine;

        public void Show(InventoryItem item)
        {
            // отменяем предыдущие анимации
            if (_fadeRoutine != null) StopCoroutine(_fadeRoutine);
            _fadeRoutine = StartCoroutine(FadeInAndOut(item));
        }

        private IEnumerator FadeInAndOut(InventoryItem item)
        {
            nameText.text = item.Definition.itemName;
            descText.text = item.Definition.description;

            // Fade In (0 → 1 за 0.5c)
            yield return Fade(0f, 1f, 0.5f);

            // Ждём 2 секунды
            yield return new WaitForSeconds(2f);

            // Fade Out (1 → 0 за 0.5c)
            yield return Fade(1f, 0f, 0.5f);
        }

        private IEnumerator Fade(float from, float to, float duration)
        {
            float elapsed = 0f;
            canvasGroup.interactable = to > 0.5f;
            canvasGroup.blocksRaycasts = to > 0.5f;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                canvasGroup.alpha = Mathf.Lerp(from, to, elapsed / duration);
                yield return null;
            }
            canvasGroup.alpha = to;
        }

        public void Hide()
        {
            if (_fadeRoutine != null) StopCoroutine(_fadeRoutine);
            canvasGroup.alpha = 0f;
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;
        }
    }
}