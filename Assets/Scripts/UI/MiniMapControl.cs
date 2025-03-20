using UnityEngine;
using UnityUtils;


namespace UISystem
{
    public class MiniMapControl : IUIView
    {
        [SerializeField] private Transform mapCam;
        [SerializeField] private Transform characterMark;
        [SerializeField] private Transform playerPos;
        public bool enableMiniMap;

        private void LateUpdate()
        {
            // 角色标记与小地图摄像机在世界坐标中与角色位置同步
            if (!enableMiniMap) return;

            mapCam.position = new Vector3(playerPos.position.x, mapCam.position.y, playerPos.position.z);
            characterMark.position = new Vector3(playerPos.position.x, characterMark.position.y, playerPos.position.z);
            
            Vector3 direction = Vector3.ProjectOnPlane(playerPos.forward, Vector3.up);
            characterMark.forward = direction;
        }

        public override void OnInit()
        {
            if (!enableMiniMap)
            {
                gameObject.SetActiveSafe(false);
            }
        }

        public override void OnOpen()
        {
            gameObject.SetActiveSafe(true);
        }

        public override void OnClose()
        {
            gameObject.SetActiveSafe(false);
        }
    }
}