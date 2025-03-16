using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    public GameObject mainCam;

    [Header("主菜单")]
    [Header("继续按钮")]
    [SerializeField] private Button continueButton;

    [Header("设置")]
    public TMP_Dropdown frameRateSelect;

    private bool _levelSelected;

    private void Awake()
    {
        Application.targetFrameRate = 30;
        SetContinueBotton();
    }

    /// <summary>
    /// 将镜头旋转到关卡选择界面
    /// </summary>
    public void ShowLevelSelectMenu()
    {
        if (!_levelSelected)
        {
            mainCam.transform.rotation = Quaternion.Euler(0, 180, 0);
            _levelSelected = true;
        }
        else
        {
            mainCam.transform.rotation = Quaternion.Euler(Vector3.zero);
            _levelSelected = false;
        }
    }

    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        Debug.Log("Quit!");
#else
        Application.Quit();
#endif
    }

    public void ChangeFrameRate()
    {
        if (frameRateSelect.value == 0)
            Application.targetFrameRate = 30;
        else if (frameRateSelect.value == 1)
            Application.targetFrameRate = 60;
        else if (frameRateSelect.value == 2)
            Application.targetFrameRate = 120;
        else if (frameRateSelect.value == 3)
            QualitySettings.vSyncCount = 1;
    }

    public void SetContinueBotton()
    {
        // if (SaveLoad.CheckSaveDateExist(out SaveDate data))
        // {
        //     // continueButton.onClick.AddListener(() => LoadNewScene("FPSMap_0"));
        //     continueButton.interactable = true;
        // }
        // else continueButton.interactable = false;
    }
}
