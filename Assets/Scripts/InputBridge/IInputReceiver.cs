using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DRockInputBridge
{
    public interface IInputReceiver
    {
        /// <summary>
        /// 当该控制器被启用时需要激活哪些ActionMap
        /// </summary>
        /// <param name="sourceInput">输入源</param>
        void SetActionMap(SourceInput sourceInput);

        /// <summary>
        /// 当该控制器被启用时调用
        /// </summary>
        void OnEnter();

        /// <summary>
        /// 当该控制器为当前控制器时每帧调用
        /// </summary>
        void OnUpdate();

        /// <summary>
        /// 当前帧刷新后调用(用于控制摄像机)
        /// </summary>
        void OnLateUpdate();

        /// <summary>
        /// 当该控制器被停用时调用
        /// </summary>
        void OnExit();

        /// <summary>
        /// 当有子控制器推入栈时，该控制器暂停接收时
        /// </summary>
        void OnPause();

        /// <summary>
        /// 当子控制器离开时，该控制器重新启用接收时
        /// </summary>
        void OnResume();
    }
}
