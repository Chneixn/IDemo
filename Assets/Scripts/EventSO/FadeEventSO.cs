using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(menuName = "MyEvent/UI/FadeEventSO")]
public class FadeEventSO : ScriptableObject
{
    public UnityAction<Color, float, bool> OnFadeCanvasRequested;

    /// <summary>
    /// 渐变黑
    /// </summary>
    /// <param name="duration"></param>
    public void FadeIn(float duration)
    {
        RasieEvent(Color.black, duration, true);
    }

    /// <summary>
    /// 渐变透明
    /// </summary>
    /// <param name="duration"></param>
    public void FadeOut(float duration)
    {
        RasieEvent(Color.clear, duration, false);
    }

    public void RasieEvent(Color targetColor, float duration, bool fadeIn)
    {
        OnFadeCanvasRequested?.Invoke(targetColor, duration, fadeIn);
    }
}
