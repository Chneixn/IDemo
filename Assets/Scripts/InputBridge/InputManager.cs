using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.EnhancedTouch;

namespace DRockInputBridge
{
    // TODO：也许可以多个控制器并行推入推出
    public enum PushType
    {
        Single,
        Mult
    }

    public interface IUserInput
    {
        public SourceInput SourceInput { get; }
        bool GetKeyDown(string name);
        bool GetKeyUp(string name);
        bool GetKey(string name);
        float GetValue(string name);
        Vector2 GetVector2(string name);
    }

    public class InputManager : IUserInput
    {
        public static InputManager Instance { get; } = new InputManager();

        private SourceInput _sourceInput;
        public SourceInput SourceInput => _sourceInput;
        protected PushdownFSM fsm = new();
        public PushdownFSM FSM => fsm;

        /// <summary>
        /// SourceInput 只能在Mono周期内被实例化
        /// </summary>
        public InputManager()
        {
            _sourceInput = new();
            _sourceInput.Enable();
        }

        public void Push(IInputReceiver receiver)
        {
            receiver.SetActionMap(_sourceInput);
            fsm.Push(receiver);
        }

        public void Pop()
        {
            fsm.Pop();
        }

        public void OnUpdate()
        {
            fsm.CurrentReceiver.OnUpdate();
        }

        public void OnLateUpdate()
        {
            fsm.CurrentReceiver.OnLateUpdate();
        }

        #region 转接为以string监听输入(旧输入模式样式)
        public bool GetKeyDown(string name)
        {
            return _sourceInput.asset[name].triggered;
        }

        public bool GetKeyUp(string name)
        {
            return _sourceInput.asset[name].WasReleasedThisFrame();
        }

        public bool GetKey(string name)
        {
            return _sourceInput.asset[name].IsPressed();
        }

        public float GetValue(string name)
        {
            return _sourceInput.asset[name].ReadValue<float>();
        }

        public Vector2 GetVector2(string name)
        {
            return _sourceInput.asset[name].ReadValue<Vector2>();
        }
        #endregion
    }
}
