using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class AnimationHelper
{
    
    public static IEnumerator SlideIn(RectTransform transform, UIPageDirection direction, float speed, UnityEvent onEnd = null)
    {
        Vector2 startPosition = direction switch
        {
            UIPageDirection.Up => new(0, -Screen.height),
            UIPageDirection.Right => new(-Screen.width, 0),
            UIPageDirection.Down => new(0, Screen.height),
            UIPageDirection.Left => new(Screen.width, 0),
            _ => new(0, -Screen.height),
        };
        float timer = 0;
        while (timer < 1)
        {
            transform.anchoredPosition = Vector2.Lerp(startPosition, Vector2.zero, timer);
            yield return null;
            timer += Time.deltaTime * speed;
        }

        transform.anchoredPosition = Vector2.zero;
        onEnd?.Invoke();
    }

    public static IEnumerator SlideOut(RectTransform transform, UIPageDirection direction, float speed, UnityEvent onEnd = null)
    {
        Vector2 endPosition = direction switch
        {
            UIPageDirection.Up => new(0, Screen.height),
            UIPageDirection.Right => new(Screen.width, 0),
            UIPageDirection.Down => new(0, -Screen.height),
            UIPageDirection.Left => new(-Screen.width, 0),
            _ => new(0, Screen.height),
        };
        float timer = 0;
        while (timer < 1)
        {
            transform.anchoredPosition = Vector2.Lerp(Vector2.zero, endPosition, timer);
            yield return null;
            timer += Time.deltaTime * speed;
        }

        transform.anchoredPosition = endPosition;
        onEnd?.Invoke();
    }

    public static IEnumerator ZoomIn(RectTransform rectTransform, float speed, UnityEvent onEnd = null)
    {
        float timer = 0;
        while (timer < 1)
        {
            rectTransform.localScale = Vector3.Lerp(Vector3.zero, Vector3.one, timer);
            yield return null;
            timer += Time.deltaTime * speed;
        }

        rectTransform.localScale = Vector3.one;
        onEnd?.Invoke();
    }

    public static IEnumerator ZoomOut(RectTransform rectTransform, float speed, UnityEvent onEnd = null)
    {
        float timer = 0;
        while (timer < 1)
        {
            rectTransform.localScale = Vector3.Lerp(Vector3.one, Vector3.zero, timer);
            yield return null;
            timer += Time.deltaTime * speed;
        }

        rectTransform.localScale = Vector3.zero;
        onEnd?.Invoke();
    }

    public static IEnumerator FadeIn(CanvasGroup canvasGroup, float speed, UnityEvent onEnd = null)
    {
        canvasGroup.blocksRaycasts = false;
        canvasGroup.interactable = false;

        float timer = 0;
        while (timer < 1)
        {
            canvasGroup.alpha = Mathf.Lerp(1, 0, timer);
            yield return null;
            timer += Time.deltaTime * speed;
        }

        canvasGroup.alpha = 0;
        onEnd?.Invoke();
    }

    public static IEnumerator FadeOut(CanvasGroup canvasGroup, float speed, UnityEvent onEnd = null)
    {
        canvasGroup.blocksRaycasts = true;
        canvasGroup.interactable = true;

        float timer = 0;
        while (timer < 1)
        {
            canvasGroup.alpha = Mathf.Lerp(0, 1, timer);
            yield return null;
            timer += Time.deltaTime * speed;
        }

        canvasGroup.alpha = 1;
        onEnd?.Invoke();
    }
}
