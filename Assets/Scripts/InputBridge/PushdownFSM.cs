using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// 1.current为当前操作状态，此时只启动某一InputAction
/// 2.当有子菜单打开时，调用Push方法，将前一操作状态传入stack中，此时只启用子菜单的InputAction
/// 3.当从子菜单或子模式退出时，调用Pop方法，将当前操作状态退出，从stack中取出上一个传入的操作状态

namespace DRockInputBridge
{
    /// <summary>
    /// 下推状态机，实现多个控制状态的逐个推入与逐个推出
    /// </summary>
    public class PushdownFSM
    {
        private IInputReceiver current; // 当前输入接收器
        public IInputReceiver CurrentReceiver => current;
        public Action<IInputReceiver> OnInputIn;
        public Action<IInputReceiver> OnInputOut;
        private Stack<IInputReceiver> stack = new(); // 缓存栈

        /// <summary>
        /// 切换当前控制为新控制，当前控制器推入栈中
        /// </summary>
        /// <param name="newReceiver"></param>
        public void Push(IInputReceiver newReceiver)
        {
            newReceiver.OnEnter();
            if (current != null)
            {
                current.OnPause();
                stack.Push(current);
            }

            current = newReceiver;
            OnInputIn?.Invoke(current);
        }

        /// <summary>
        /// 推出栈
        /// </summary>
        public void Pop()
        {
            current.OnExit();
            if (stack.Count > 0)
            {
                current = stack.Pop();
                current.OnResume();
            }
            OnInputOut?.Invoke(current);
        }
    }
}
