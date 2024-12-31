using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DRockInputBridge
{
    public class InputManagerInspectorDebug : MonoBehaviour
    {
        public InputManager inputManager;
        public List<UnityInputReceiver> inputStack = new();
        private PushdownFSM debugFSM;

        private void OnEnable()
        {
            if (debugFSM != null)
            {
                debugFSM.OnInputIn += OnPush;
                debugFSM.OnInputOut += OnPop;
            }
        }

        private void Start()
        {
            inputManager = InputManager.Instance;
            debugFSM = inputManager.FSM;
            if (debugFSM == null) Debug.LogError("FSM Missing!");
            debugFSM.OnInputIn += OnPush;
            debugFSM.OnInputOut += OnPop;
        }

        private void OnDisable()
        {
            debugFSM.OnInputIn -= OnPush;
            debugFSM.OnInputOut -= OnPop;
        }

        private void OnPush(IInputReceiver receiver)
        {
            var rec = receiver as UnityInputReceiver;
            if (rec != null)
                inputStack.Add(rec);
        }

        private void OnPop(IInputReceiver receiver)
        {
            inputStack.RemoveAt(0);
        }
    }
}

