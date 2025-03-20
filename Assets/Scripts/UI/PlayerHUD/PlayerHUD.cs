using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UISystem
{
    public class PlayerHUD : UIViewController
    {
        public PlayerStateBar playerStateBar;
        public InteractionInfo interactionInfo;
        public MiniMapControl miniMap;
        public CrossHair crossHair;

        public override void OnLoad()
        {
            playerStateBar.OnInit();
            interactionInfo.OnInit();
            miniMap.OnInit();
            crossHair.OnInit();
        }

        public override void OnOpen()
        {
            playerStateBar.OnOpen();
            interactionInfo.OnOpen();
            miniMap.OnOpen();
            crossHair.OnOpen();
        }
    }
}

