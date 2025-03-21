using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace UISystem
{
    public class InteractionInfo : IUIView
    {
        public string InfoDefaultText = "按 \"E\" 交互";
        public string InfoText;
        public TextMeshProUGUI textGUI;

        public override void OnInit()
        {
            PlayerManager.Instance.Interactor.OnHoverEnter += OnHoverEnter;
            PlayerManager.Instance.Interactor.OnHoverExit += OnHoverExit;
            gameObject.SetActive(false);
        }

        private void OnHoverExit(IInteractable interactable)
        {
            InfoText = InfoDefaultText;
            gameObject.SetActive(false);
        }

        private void OnHoverEnter(IInteractable interactable)
        {
            if (!string.IsNullOrEmpty(interactable.InteractionText))
                InfoText = interactable.InteractionText;
            gameObject.SetActive(true);
        }

        public override void OnClose()
        {

        }

        public override void OnOpen()
        {
            textGUI.text = InfoText;
        }
    }
}