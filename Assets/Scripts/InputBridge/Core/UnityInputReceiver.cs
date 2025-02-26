using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DRockInputBridge
{
    public abstract class UnityInputReceiver : MonoBehaviour, IInputReceiver
    {
        public IUserInput userInput;

        public virtual void Start()
        {
            userInput = InputManager.Instance;
        }
        public abstract void SetActionMap(SourceInput sourceInput);
        public abstract void OnEnter();
        public abstract void OnExit();
        public abstract void OnPause();
        public abstract void OnResume();
        public abstract void OnUpdate();
        public virtual void OnLateUpdate() { }
    }
}
