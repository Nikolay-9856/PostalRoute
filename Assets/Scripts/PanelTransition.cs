using UnityEngine;
[RequireComponent(typeof(CanvasGroup))]
public class PanelTransition : MonoBehaviour
{
    public float duration = 0.18f;
    private CanvasGroup group;
    private float elapsed;
    private void OnEnable()
    {
        group = GetComponent<CanvasGroup>();
        elapsed = 0;
        group.alpha = 0;
        group.interactable = false;
        // Блокирует нажатия сквозь появляющуюся панель с первого кадра.
        group.blocksRaycasts = true;
    }
    private void Update()
    {
        // unscaledDeltaTime продолжает отсчёт при Time.timeScale = 0 (пауза).
        elapsed += Time.unscaledDeltaTime;
        group.alpha = duration <= 0 ? 1 : Mathf.Clamp01(elapsed / duration);
        group.interactable = group.alpha >= 1;
    }
}
