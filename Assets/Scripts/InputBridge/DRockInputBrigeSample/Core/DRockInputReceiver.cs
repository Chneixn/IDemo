using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DRockInputBridge.Sample
{
    public abstract class DRockInputReceiver : IDRockInputReceiver
    {

        private int id;
        public int controllerId => id;
        public IDRockUserInput userInput;
        public DRockInputReceiver(IDRockUserInput input, int id)
        {
            this.id = id;
            this.userInput = input;
        }

        // 实现该函数以接受用户输入
        public abstract void OnUpdate(float deltaTime);

        // 重写进入和退出时的回调函数，则默认在执行对应的状态时执行
        public virtual void OnEnter() { }
        public virtual void OnExit() { }
        public virtual void OnPause() { }
        public virtual void OnResume() { }
    }
}
