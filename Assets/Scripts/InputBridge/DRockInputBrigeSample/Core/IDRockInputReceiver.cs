using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DRockInputBridge.Sample
{
    public interface IDRockInputReceiver
    {
        int controllerId { get; }
        /// <summary>
        /// 接管所有Update中函数，在GameManager中统一调用
        /// </summary>
        /// <param name="deltaTime"></param>
        void OnUpdate(float deltaTime);

        /// <summary>
        /// 当该控制器被启用时调用
        /// </summary>
        void OnEnter();

        /// <summary>
        /// 当该控制器被停用时调用
        /// </summary>
        void OnExit();

        /// <summary>
        /// 停用该控制器后，推入栈前调用
        /// </summary>
        void OnPause();

        /// <summary>
        /// 从栈离开，启用为当前控制器时调用
        /// </summary>
        void OnResume();
    }
}
