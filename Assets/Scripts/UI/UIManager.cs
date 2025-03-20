using UnityEngine;
using TMPro;
using System;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityUtils;
using UnityEngine.AddressableAssets;

namespace UISystem
{
    public enum UILayer
    {
        SceneLayer = 1000, // 3D UI, 使用 World Space
        BackgroundLayer = 2000, // UI 背景, 黑边
        NormalLayer = 3000,   // 没有特殊要求的 UI
        InfoLayer = 4000,  // 需要在普通层上显示的 UI
        TopLayer = 5000,   // 顶层, Loading 显示
        TipLayer = 6000    // 提示层, 显示一些提示窗口
    }


    public class UIManager : PersistentSingleton<UIManager>
    {
        public List<UIViewController> openedViews = new();

        public ConfimPanel confimPanel;

        void Start()
        {
            Addressables.LoadAssetAsync<GameObject>("ConfimPanel").Completed += (handle) =>
            {
                confimPanel = Instantiate(handle.Result).GetComponent<ConfimPanel>();
                confimPanel.gameObject.SetActive(false);
                handle.Release();
            };
        }

        public void PushUI(UIViewController controller)
        {
            openedViews.Add(controller);
            controller.Index = openedViews.Count - 1;
            controller.OnLoad();
            controller.OnOpen();
            controller.SetVisible(true);
        }

        public int PopUI(UIViewController controller)
        {
            openedViews.Remove(controller);
            controller.OnClose();
            controller.SetVisible(false);
            return controller.Index;
        }

        public void LockCursor(bool isLock)
        {
            if (isLock)
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
            else
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
        }

        public void CallConfimPanel(string title, string description, Action OnConfim,
            string confimText = "Confim", string cancelText = "Cancel", Action OnCancel = null)
        {
            confimPanel.title = title;
            confimPanel.description = description;
            confimPanel.cancelText = cancelText;
            confimPanel.confimText = confimText;
            confimPanel.OnConfim = OnConfim;
            confimPanel.OnCancel = OnCancel;
            PushUI(confimPanel);
        }
    }
}
