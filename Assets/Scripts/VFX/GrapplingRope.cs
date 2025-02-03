using System;
using UnityEngine;

[Serializable]
public class GrapplingRope
{
    [Header("Setting")]
    private Grabbing grabber;
    public int quality = 500;
    public float damper = 14f;
    public float strength = 800f;
    public float velocity = 15f;
    public float waveCount = 3f;
    public float waveHeight = 1f;
    public bool enabledGrabblingPath = false;
    private readonly AnimationCurve affectCurve;
    private readonly Spring spring = new();
    private readonly LineRenderer rope_lr;
    private readonly LineRenderer swing_lr;
    private Vector3 currentGrapplePosition;

    public GrapplingRope(Grabbing grabber, LineRenderer rope_lr, LineRenderer swing_lr, AnimationCurve affectCurve)
    {
        spring.SetTarget(0);
        this.rope_lr = rope_lr;
        this.swing_lr = swing_lr;
        this.grabber = grabber;
        this.affectCurve = affectCurve;
        ClearDraw();
    }

    public void ClearDraw()
    {
        enabledGrabblingPath = false;
        currentGrapplePosition = grabber.gunTip.position;
        spring.Reset();
        if (rope_lr.positionCount > 0)
            rope_lr.positionCount = 0;

    }

    public void DrawRope()
    {
        if (rope_lr.positionCount == 0)
        {
            spring.SetVelocity(velocity);
            rope_lr.positionCount = quality + 1;
        }

        spring.SetDamper(damper);
        spring.SetStrength(strength);
        spring.Update(Time.deltaTime);

        Vector3 grapplePoint = grabber.GrapplePoint;
        Vector3 gunTipPosition = grabber.gunTip.position;
        Vector3 up = Quaternion.LookRotation((grapplePoint - gunTipPosition).normalized) * Vector3.up;

        currentGrapplePosition = Vector3.Lerp(currentGrapplePosition, grapplePoint, grabber.grappleDelayTime / Time.deltaTime);

        if (enabledGrabblingPath)
        {
            // 需要绘制钩爪时运动曲线
            swing_lr.SetPosition(0, gunTipPosition);
            swing_lr.SetPosition(1, currentGrapplePosition);
        }

        for (int i = 0; i < quality + 1; i++)
        {
            // 绘制钩爪射出时曲线
            float delta = i / (float)quality;
            Vector3 offest = Mathf.Sin(delta * waveCount * Mathf.PI) * affectCurve.Evaluate(delta) * spring.Value * waveHeight * up;
            rope_lr.SetPosition(i, Vector3.Lerp(gunTipPosition, currentGrapplePosition, delta) + offest);
        }
    }
}
