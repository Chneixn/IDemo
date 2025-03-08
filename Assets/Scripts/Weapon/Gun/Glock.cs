using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Glock : BaseGun
{
    public override void HandleInput(ref WeaponInput inputs)
    {
        if (inputs.reload) Reload();
        else if (inputs.fire) Shoot();
        else if (inputs.aim)
        {
            AimStart();
        }
        else if (isAiming)
        {
            AimEnd();
        }
        else if (inputs.switchFireMod) ChangeFireMode();
    }
}
