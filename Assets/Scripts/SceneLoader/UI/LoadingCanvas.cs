using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LoadingCanvas : MonoBehaviour
{
    private float percentCompleted = 0f;
    private float targetPercent = 0f;
    [SerializeField] private TextMeshProUGUI loadingText;
    [SerializeField] private Slider _slider;
    public bool IsDone => percentCompleted >= 1f;

    private void OnEnable() => Reset();
    private void OnDisable() => Reset();

    private void Update()
    {
        while (percentCompleted < targetPercent)
        {
            percentCompleted += Time.deltaTime;
            _slider.value = Mathf.Clamp01(percentCompleted / targetPercent);
        }
    }

    public void SetTargetPercent(float targetPercent)
    {
        this.targetPercent = targetPercent;
    }

    private void Reset()
    {
        percentCompleted = 0f;
        targetPercent = 0f;
        _slider.value = 0f;
        loadingText.text = string.Empty;
    }
}
