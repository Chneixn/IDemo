using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class SpringTest : MonoBehaviour
{
    public bool pause = true;
    public bool needDraw = false;
    public int m = 1;
    public float moveSpeed = 15f;
    public Vector3 gravity = new(0f, -30f, 0f);
    public Vector3 velocity = Vector3.zero;
    public float distance = 0f;
    public SpringCalculater joint;
    public Transform target;
    public LineRenderer lr;
    private Timer timer;

    private void Start()
    {
        ClearDraw();
        joint = new();
        timer = TimerManager.CreatCountingTimer();
    }

    [ContextMenu("重置位置")]
    public void ResetPosition()
    {
        transform.position = new(target.position.x + 5f, target.position.y - 5f, target.position.z);
    }

    // Update is called once per frame
    void Update()
    {
        distance = Vector3.Distance(target.position, transform.position);

        if (!pause)
        {
            joint.active = true;
            joint.SelfVelocity = velocity;
            joint.SelfPosition = transform.position;
            joint.TargetPosition = target.position;
            joint.gravity = gravity;

            joint.UpdateValue(Time.deltaTime);

            Vector3 targetVelocity = joint.Value;

            int v = (int)timer.Elapsed / 5;
            m = v % 2 == 0 ? 1 : -1;
            targetVelocity += m * moveSpeed * Time.deltaTime * Vector3.right;

            // velocity = Vector3.Lerp(velocity, targetVelocity, Time.deltaTime * 12f);
            velocity = targetVelocity;

            transform.position += velocity * Time.deltaTime;

            if (transform.position.y < 0f)
            {
                transform.position = new(transform.position.x, 0f, transform.position.z);
                velocity.y = 0f;
            }
        }
    }

    private void LateUpdate()
    {
        if (needDraw && joint.TargetPosition != Vector3.zero)
        {
            // 需要绘制钩爪时运动曲线
            lr.positionCount = 2;
            lr.SetPosition(0, transform.position);
            lr.SetPosition(1, joint.TargetPosition);
        }
        else ClearDraw();
    }

    private void ClearDraw()
    {
        if (lr.positionCount > 0)
            lr.positionCount = 0;
    }
}
