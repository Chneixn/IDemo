using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Canvas))]
[DisallowMultipleComponent]
public class MenuController : MonoBehaviour
{
    [SerializeField] private UIPanel initialPage;  // 初始界面
    [SerializeField] private GameObject firstFocusItem; // 界面默认选中控件

    private Canvas rootCanvas;
    private readonly Stack<UIPanel> pageStack = new();

    [SerializeField] private bool debug;
    [SerializeField] private string currentPanelName;

    private void Awake()
    {
        rootCanvas = GetComponent<Canvas>();
    }

    private void Start()
    {
        if (firstFocusItem != null)
            EventSystem.current.SetSelectedGameObject(firstFocusItem);

        if (initialPage != null) PushPage(initialPage);
    }

    private void OnCancel()
    {
        if (rootCanvas.enabled && rootCanvas.gameObject.activeInHierarchy)
        {
            if (pageStack.Count != 0) PopPage();
        }
    }

    /// <summary>
    /// 界面是否已在栈中
    /// </summary>
    /// <param name="page"></param>
    /// <returns></returns> <summary>
    public bool IsPageInStack(UIPanel page)
    {
        return pageStack.Contains(page);
    }

    /// <summary>
    /// 界面是否已在栈顶
    /// </summary>
    /// <param name="page"></param>
    /// <returns></returns>
    public bool IsPageOnTopOfStack(UIPanel page)
    {
        return pageStack.Count > 0 && page == pageStack.Peek();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="page"></param>
    private void PushPage(UIPanel page)
    {
        page.OnEnter();

        if (pageStack.Count > 0)
        {
            UIPanel current = pageStack.Peek();

            if (current.ExitOnNewPanelPush) current.OnExit();
        }

        pageStack.Push(page);

        // Debug信息
        if (debug) currentPanelName = page.gameObject.name;
    }

    private void PopPage()
    {
        if (pageStack.Count > 1)
        {
            UIPanel page = pageStack.Pop();
            page.OnExit();

            // Debug信息
            if (debug) currentPanelName = page.gameObject.name;

            UIPanel newCurrentPage = pageStack.Peek();

            if (newCurrentPage.ExitOnNewPanelPush) newCurrentPage.OnEnter();
        }
        else
        {
            Debug.LogWarning("Trying to pop a page but only 1 page remains in the stack!");
        }
    }

    public void PopAllPages()
    {
        for (int i = 1; i < pageStack.Count; i++)
        {
            PopPage();
        }
    }
}
