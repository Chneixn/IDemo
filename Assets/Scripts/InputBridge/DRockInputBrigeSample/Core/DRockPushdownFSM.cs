using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DRockInputBridge.Sample
{
    /// <summary>
    /// 下推状态机，实现多个控制状态的逐个推入与逐个推出
    /// 1.current为当前操作状态，此时只启动某一InputAction
    /// 2.当有子菜单打开时，调用Push方法，将前一操作状态传入stack中，此时只启用子菜单的InputAction
    /// 3.当从子菜单或子模式退出时，调用Pop方法，将当前操作状态退出，从stack中取出上一个传入的操作状态
    /// </summary>
    public class DRockPushdownFSM
    {
        private IDRockInputReceiver current; //当前输入接收器
        public IDRockInputReceiver CurrentReceiver => current;

        private Stack<IDRockInputReceiver> Stack; //缓存栈

        private Action<int> SetCurrentInput;

        public DRockPushdownFSM(Action<int> setCurrentInput)
        {
            //在实例化状态机时需要设定第一个操作状态
            this.SetCurrentInput = setCurrentInput;
            this.Stack = new();
        }

        /// <summary>
        /// 切换当前控制为新控制，当前控制器推入栈中
        /// </summary>
        /// <param name="newReceiver"></param>
        public void Push(IDRockInputReceiver newReceiver)
        {
            newReceiver.OnEnter();
            if (current != null)
            {
                current.OnPause();
                Stack.Push(current);
            }
            SetCurrentInput(newReceiver.controllerId);

            current = newReceiver;
#if UNITY_EDITOR
            Debug.Log($"当前控制方案：{newReceiver.controllerId}");
#endif
        }

        /// <summary>
        /// 推出栈
        /// </summary>
        public void Pop()
        {
            current.OnExit();
            if (Stack.Count > 0)
            {
                current = Stack.Pop();
                current.OnResume();
                SetCurrentInput(current.controllerId);
#if UNITY_EDITOR
                Debug.Log($"当前控制方案：{current.controllerId}");
#endif
            }
        }
    }
}
