using System;
using UnityEngine;
using UnityEngine.UI;

public class FadeCanvas : MonoBehaviour
{
    [SerializeField] private Image fadeImage;

    private void Awake()
    {
        if (fadeImage == null) GetComponentInChildren<Image>();
    }

    public void FadeOut(float duration = 0.5f)
    {
        Color cur = fadeImage.color;
        cur.a = 1;
        fadeImage.color = cur;
        fadeImage.CrossFadeAlpha(0, duration, true);
    }

    public void FadeIn(float duration = 0.5f)
    {
        Color cur = fadeImage.color;
        cur.a = 0;
        fadeImage.color = cur;
        fadeImage.CrossFadeAlpha(1, duration, true);
    }
}

