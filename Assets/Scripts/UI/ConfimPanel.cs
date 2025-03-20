using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UISystem
{
    public class ConfimPanel : UIViewController
    {
        public string title;
        public string description;
        public string confimText;
        public string cancelText;
        [SerializeField] private TextMeshProUGUI titleGUI;
        [SerializeField] private TextMeshProUGUI descriptionGUI;
        [SerializeField] private TextMeshProUGUI confimGUI;
        [SerializeField] private TextMeshProUGUI cancelGUI;

        public Button confimButton;
        public Button cancelButton;

        public Action OnConfim;
        public Action OnCancel;

        public override void OnOpen()
        {
            titleGUI.text = title;
            descriptionGUI.text = description;
            confimGUI.text = confimText;
            cancelGUI.text = cancelText;
            confimButton.onClick.AddListener(() => { UIManager.Instance.PopUI(this); OnConfim?.Invoke(); });
            cancelButton.onClick.AddListener(() => { UIManager.Instance.PopUI(this); OnCancel?.Invoke(); });
        }

        public override void OnClose()
        {
            OnConfim = null;
            OnCancel = null;
        }
    }
}