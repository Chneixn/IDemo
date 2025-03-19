using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 界面显示的基类，所有界面都继承 UIView 实现接口
/// </summary>
namespace UISystem
{
    public abstract class IUIView : MonoBehaviour
    {
        public abstract void OnInit();
        public abstract void OnOpen();
        public abstract void OnClose();
        public virtual void OnReShow() { }
        public virtual void OnHide() { }
    }
}