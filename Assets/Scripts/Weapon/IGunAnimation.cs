using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IGunAnimation
{
    void AimStart();
    void AimEnd();
    void ShotStart(bool aimState);
    void ShotEnd(bool isEmpty);
    void ReloadStart(bool isEmpty);
    void ReloadEnd();
}
