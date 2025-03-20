using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UISystem
{
    public class UIViewController : MonoBehaviour
    {
        public int Index;
        public UILayer UILayer;
        protected Stack<IUIView> UIViews = new();

        public virtual void OnLoad() { }

        public virtual void OnOpen() { }

        public virtual void OnClose()
        {

        }

        public virtual void AddView(IUIView view)
        {
            UIViews.Push(view);
            view.OnInit();
        }

        public virtual void RemoveView()
        {
            if (UIViews.Count > 0)
                UIViews.Pop();
            else UIManager.Instance.PopUI(this);
        }

        public void SetVisible(bool visible)
        {
            foreach (var view in UIViews)
            {
                if (visible) view.OnReShow();
                else view.OnHide();
            }
            gameObject.SetActive(visible);
        }

    }
}