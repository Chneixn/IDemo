using UnityEngine;
using UnityUtils;

public class MiniMapControl : MonoBehaviour
{
    [SerializeField] private Transform mapCam;
    [SerializeField] private Transform characterMark;
    [SerializeField] private Transform playerPos;
    [SerializeField] private GameObject miniMapUI;
    public bool enableMiniMap;

    private void Awake()
    {
        miniMapUI = GameObject.Find("MiniMapUI");
        if (enableMiniMap && miniMapUI != null)
        {
            miniMapUI.SetActiveSafe(true);
        }
        else
        {
            Debug.LogError("MiniMapUI missing!");
        }
    }

    private void LateUpdate()
    {
        // 角色标记与小地图摄像机在世界坐标中与角色位置同步
        if (enableMiniMap)
        {
            if (playerPos != null)
            {
                Vector3 _direction = Vector3.ProjectOnPlane(playerPos.forward, Vector3.up);
                if (mapCam != null)
                {
                    mapCam.position = new Vector3(playerPos.position.x, mapCam.position.y, playerPos.position.z);
                }
                else
                    Debug.LogError("mapCam missing!");
                if (characterMark != null)
                {
                    characterMark.position = new Vector3(playerPos.position.x, characterMark.position.y, playerPos.position.z);
                    characterMark.forward = _direction;
                }
                else
                    Debug.LogError("characterMark missing!");
            }
            else
                Debug.LogError("playerPos missing!");
        }
    }
}
