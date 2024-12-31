using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DRockInputBridge.Sample
{
    public abstract class DRockUnityInputReceiver : MonoBehaviour, IDRockInputReceiver
    {
        private int id;
        public int controllerId => id;
        public IDRockUserInput userInput;
        public virtual void Init(IDRockUserInput input, int id)
        {
            this.id = id;
            this.userInput = input;
        }
        public abstract void OnUpdate(float deltaTime);
        public virtual void OnEnter() { }
        public virtual void OnExit() { }
        public virtual void OnPause() { }
        public virtual void OnResume() { }
    }
}
