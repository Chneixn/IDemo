using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LoadingPanel : MonoBehaviour
{
    public float percentCompleted = 0f;
    [SerializeField] private TextMeshProUGUI loadingText;
    [SerializeField] private Slider _slider;

    private void Awake()
    {
        Reset();
    }

    private void OnDisable()
    {
        Reset();
    }

    private void Reset()
    {
        _slider.value = 0f;
        loadingText.text = string.Empty;
    }

    private void Update()
    {
        _slider.value = percentCompleted;
        loadingText.text = $"{percentCompleted}%";
    }
}
