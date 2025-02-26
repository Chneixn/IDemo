using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Debug_CharacterInfo : MonoBehaviour
{
    private CharacterControl cc;
    [SerializeField] private float speed;
    [SerializeField] private TextMeshProUGUI ui_speed;

    [SerializeField] private Vector3 moveDirection;
    [SerializeField] private TextMeshProUGUI ui_moveDirection;

    [SerializeField] private Vector3 position;
    [SerializeField] private TextMeshProUGUI ui_position;

    [SerializeField] private bool isGround;
    [SerializeField] private TextMeshProUGUI ui_isGround;

    [SerializeField] private string moveMentState;
    [SerializeField] private TextMeshProUGUI ui_moveMentState;

    private void Start()
    {
        cc = PlayerManager.Instance.CharacterControl;
        cc.OnMovementStateChanged += UpdateMoveState;
    }

    private void OnDisable()
    {
        cc.OnMovementStateChanged -= UpdateMoveState;
    }

    void Update()
    {
        speed = cc.CurrentSpeed.magnitude;
        ui_speed.text = $"Speed: {speed:f2}";

        moveDirection = cc.MoveDirection;
        ui_moveDirection.text = $"Direction: {moveDirection.x:f2} {moveDirection.y:f2} {moveDirection.z:f2}";

        position = cc.transform.position;
        ui_position.text = $"Position: {position.x:f2} {position.y:f2} {position.z:f2}";

        isGround = cc.IsStableGround;
        ui_isGround.text = $"IsGround: {isGround}";
    }

    void UpdateMoveState(IMovementState state)
    {
        moveMentState = nameof(state);
        ui_moveMentState.text = "MState: " + moveMentState;
    }
}
