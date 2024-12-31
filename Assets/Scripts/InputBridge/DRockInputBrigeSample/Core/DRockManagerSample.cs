using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DRockInputBridge.Sample
{
    public interface IDRockUserInput
    {
        bool GetKeyDown(string name);
        bool GetKey(string name);
        float GetValue(string name);
        Vector2 GetVector2(string name);
    }

    public abstract class DRockBridge : IDRockUserInput
    {
        private DRockPushdownFSM fsm;
        public DRockBridge()
        {
            fsm = new DRockPushdownFSM(SetCurrentInput);
        }
        protected abstract void SetCurrentInput(int id);
        public abstract bool GetKeyDown(string name);
        public abstract bool GetKey(string name);
        public abstract float GetValue(string name);
        public abstract Vector2 GetVector2(string name);
        public void Push(IDRockInputReceiver receiver)
        {
            fsm.Push(receiver);
        }
        public void Pop()
        {
            fsm.Pop();
        }
        public void OnUpdate(float deltaTime)
        {
            fsm.CurrentReceiver?.OnUpdate(deltaTime);
        }
    }

    public class DRockManagerSample : DRockBridge
    {
        private TestPlayerInput.CharacterControlActions characterAction;
        private TestPlayerInput.InventoryInteractionActions inventoryAction;
        private TestPlayerInput.NewactionmapActions newactionmapActions;
        private TestPlayerInput _playerInput;

        public DRockManagerSample()
        {
            _playerInput = new();
            _playerInput.Enable();
            characterAction = _playerInput.CharacterControl;
            inventoryAction = _playerInput.InventoryInteraction;
            newactionmapActions = _playerInput.Newactionmap;
        }

        /// <summary>
        /// 切换ActionMap
        /// </summary>
        /// <param name="id"></param>
        protected override void SetCurrentInput(int id)
        {
            switch (id)
            {
                case 0:
                    characterAction.Enable();
                    break;

                case 1:
                    inventoryAction.Enable();
                    break;
                case 2:
                    newactionmapActions.Enable();
                    break;
            }
        }

        public override bool GetKeyDown(string name)
        {
            return _playerInput.asset[name].triggered;
        }

        public override bool GetKey(string name)
        {
            return _playerInput.asset[name].inProgress;
        }

        public override float GetValue(string name)
        {
            return _playerInput.asset[name].ReadValue<float>();
        }

        public override Vector2 GetVector2(string name)
        {
            return _playerInput.asset[name].ReadValue<Vector2>();
        }
    }
}
