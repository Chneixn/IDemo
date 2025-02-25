using System;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class FadeCanvas : MonoBehaviour
{
    [SerializeField] private Image fadeImage;
    [SerializeField] private Color color;
    private CanvasGroup group;
    private float target;

    public bool IsDone => target == group.alpha;

    private void Awake()
    {
        if (group == null) group = GetComponent<CanvasGroup>();
        if (fadeImage == null) fadeImage = GetComponentInChildren<Image>();
        fadeImage.color = color;
    }

    public void FadeOut(float duration = 0.5f)
    {
        Fade(duration, 0);
    }

    public void FadeIn(float duration = 0.5f)
    {
        Fade(duration, 1);
    }

    private void Fade(float duration, float target)
    {
        this.target = target;
        group.DOFade(target, duration);
    }
}

