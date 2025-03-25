using System;
using UnityEngine;
using UnityEngine.UI;
using System.Threading.Tasks;
using UnityUtils;
using Cysharp.Threading.Tasks;

public class FadeCanvas : MonoBehaviour
{
    [SerializeField] private Image fadeImage;
    [SerializeField] private Color color;
    [SerializeField] private CanvasGroup group;
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
        Fade(duration, 0).Forget();
    }

    public void FadeIn(float duration = 0.5f)
    {
        Fade(duration, 1).Forget();
    }

    private async UniTaskVoid Fade(float duration, float target)
    {
        this.target = target;
        group.alpha = Mathf.Abs(target - 1);

        while (!IsDone)
        {
            group.alpha = Mathf.MoveTowards(group.alpha, target, 0.01f / duration);
            await UniTask.WaitForSeconds(0.01f);
        }
    }
}

