using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UISystem
{
    public class PlayerHUD : UIViewController
    {
        public WeaponInfo weaponInfo;
        public PlayerStateBar playerStateBar;
        public InteractionInfo interactionInfo;
        public MiniMapControl miniMap;
        public CrossHair crossHair;

        public override void OnLoad()
        {
            weaponInfo.OnInit();
            playerStateBar.OnInit();
            interactionInfo.OnInit();
            miniMap.OnInit();
            crossHair.OnInit();
        }

        public override void OnOpen()
        {
            weaponInfo.OnOpen();
            playerStateBar.OnOpen();
            interactionInfo.OnOpen();
            miniMap.OnOpen();
            crossHair.OnOpen();
        }
    }
}

