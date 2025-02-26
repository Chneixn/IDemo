using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DRockInputBridge
{
    public abstract class DRockInputReceiver : IInputReceiver
    {
        private IUserInput userInput;
        private SourceInput sourceInput;

        public event Action OnInputEnter;
        public event Action OnInputExit;

        public DRockInputReceiver(IUserInput input)
        {
            this.userInput = input;
        }

        public void SetActionMap(SourceInput sourceInput)
        {
            this.sourceInput = sourceInput;
        }

        public virtual void OnEnter() { OnInputEnter?.Invoke(); }
        public virtual void OnExit() { OnInputExit?.Invoke(); }
        public virtual void OnPause() { }
        public virtual void OnResume() { }
        public void OnUpdate() { }
        public void OnLateUpdate() { }
    }
}
