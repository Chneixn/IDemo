using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class FadeCanvas : MonoBehaviour
{
    [Header("事件监听")]
    [SerializeField] private FadeEventSO fadeEvent;
    [SerializeField] private Image fadeImage;

    private void OnEnable()
    {
        fadeEvent.OnFadeCanvasRequested += Fade;
    }

    private void OnDisable()
    {
        fadeEvent.OnFadeCanvasRequested -= Fade;
    }

    private void Fade(Color targetColor, float duration, bool fadeIn)
    {
        fadeImage.DOBlendableColor(targetColor, duration);
    }
}

