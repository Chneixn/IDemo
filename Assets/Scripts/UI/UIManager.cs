using UnityEngine;
using TMPro;
using System;
using UnityEngine.UI;

public class UIManager : SameSceneSingleMono<UIManager>
{
    public void LockCursor(bool isLock)
    {
        if (isLock)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        else
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }

}
