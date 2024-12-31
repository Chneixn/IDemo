using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IGunAnimation
{
    void AimAnimationStart();
    void AimAnimationEnd();
    void ShootingAnimationStart(bool aimState);
    void ShootingAnimationEnd();
    void ReloadAnimationStart(bool isEmpty);
    void ReloadAnimationEnd();
}
