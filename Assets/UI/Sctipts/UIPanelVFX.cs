using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// UI进入和退出的方式
/// </summary>
public enum UIPageEntryMode
{
    None,
    Slide,
    Zoom,
    Fade
}

/// <summary>
/// UI界面在使用滑动进入退出时的方向
/// </summary>
public enum UIPageDirection
{
    Up,
    Right,
    Down,
    Left,
    None
}

[RequireComponent(typeof(UIPanel))]
[DisallowMultipleComponent]
public class UIPanelVFX : MonoBehaviour
{
    private AudioSource audioSource;
    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;

    [SerializeField] private float animationSpeed = 4f;
    public bool playAudio = false;
    [SerializeField] private AudioClip entryClip;
    [SerializeField] private AudioClip exitClip;
    [SerializeField] private UIPageEntryMode entryMode = UIPageEntryMode.None;
    [SerializeField] private UIPageDirection entryDirection = UIPageDirection.None;
    [SerializeField] private UIPageEntryMode exitMode = UIPageEntryMode.None;
    [SerializeField] private UIPageDirection exitDirection = UIPageDirection.None;

    private Coroutine animationCoroutine;
    private Coroutine audioCoroutine;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
        audioSource = GetComponent<AudioSource>();

        audioSource.enabled = false;
        audioSource.playOnAwake = false;
        audioSource.loop = false;
        audioSource.spatialBlend = 0;
    }

    public void Enter(UnityEvent onEnd)
    {
        StopAnimation();

        switch (entryMode)
        {
            case UIPageEntryMode.Slide:
                SlideIn(onEnd);
                break;
            case UIPageEntryMode.Zoom:
                ZoomIn(onEnd);
                break;
            case UIPageEntryMode.Fade:
                FadeIn(onEnd);
                break;
        }

        if (playAudio) PlayEntryClip();
    }

    public void Exit(UnityEvent onEnd)
    {
        StopAnimation();

        switch (exitMode)
        {
            case UIPageEntryMode.None:
                break;
            case UIPageEntryMode.Slide:
                SlideOut(onEnd);
                break;
            case UIPageEntryMode.Zoom:
                ZoomOut(onEnd);
                break;
            case UIPageEntryMode.Fade:
                FadeOut(onEnd);
                break;
        }

        if (playAudio) PlayExitClip();
    }

    #region 音效管理
    private void PlayEntryClip()
    {
        if (entryClip != null && audioSource != null)
        {
            if (audioCoroutine != null) StopCoroutine(audioCoroutine);

            audioCoroutine = StartCoroutine(PlayClip(entryClip));
        }
    }

    private void PlayExitClip()
    {
        if (exitClip != null && audioSource != null)
        {
            if (audioCoroutine != null) StopCoroutine(audioCoroutine);

            audioCoroutine = StartCoroutine(PlayClip(exitClip));
        }
    }

    private IEnumerator PlayClip(AudioClip clip)
    {
        audioSource.enabled = true;

        WaitForSeconds wait = new(clip.length);

        audioSource.PlayOneShot(clip);

        yield return wait;

        audioSource.enabled = false;
    }
    #endregion

    private void StopAnimation()
    {
        if (animationCoroutine != null)
        {
            StopCoroutine(animationCoroutine);
        }
    }

    private void SlideIn(UnityEvent onEnd)
    {
        animationCoroutine = StartCoroutine(AnimationHelper.SlideIn(rectTransform, entryDirection, animationSpeed, onEnd));
    }

    private void SlideOut(UnityEvent onEnd)
    {
        animationCoroutine = StartCoroutine(AnimationHelper.SlideOut(rectTransform, exitDirection, animationSpeed, onEnd));
    }

    private void ZoomIn(UnityEvent onEnd)
    {
        animationCoroutine = StartCoroutine(AnimationHelper.ZoomIn(rectTransform, animationSpeed, onEnd));
    }

    private void ZoomOut(UnityEvent onEnd)
    {
        animationCoroutine = StartCoroutine(AnimationHelper.ZoomOut(rectTransform, animationSpeed, onEnd));
    }

    private void FadeIn(UnityEvent onEnd)
    {
        animationCoroutine = StartCoroutine(AnimationHelper.FadeIn(canvasGroup, animationSpeed, onEnd));
    }

    private void FadeOut(UnityEvent onEnd)
    {
        animationCoroutine = StartCoroutine(AnimationHelper.FadeOut(canvasGroup, animationSpeed, onEnd));
    }
}
