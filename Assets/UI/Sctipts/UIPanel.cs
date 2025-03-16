using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[DisallowMultipleComponent]
public class UIPanel : MonoBehaviour
{
    public bool ExitOnNewPanelPush = false;
    public UnityEvent prePushAction;
    public UnityEvent postPushAction;
    public UnityEvent prePopAction;
    public UnityEvent postPopAction;

    [SerializeField] private bool enableVFX = true;
    [SerializeField] private UIPanelVFX vfx;

    private void Awake()
    {
        if (null == vfx && enableVFX)
            vfx = gameObject.AddComponent<UIPanelVFX>();
    }

    public void OnEnter()
    {
        prePushAction?.Invoke();
        
        if (enableVFX)
            vfx.Enter(postPushAction);
        else postPushAction?.Invoke();
    }

    public void OnExit()
    {
        prePopAction?.Invoke();

        if (enableVFX)
            vfx.Exit(postPopAction);
        else postPopAction?.Invoke();
    }

}
