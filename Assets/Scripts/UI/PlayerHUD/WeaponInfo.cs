using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace UISystem
{
    public class WeaponInfo : IUIView
    {
        public Sprite weaponIconSprite;
        public int currentBulletsCount;
        public int totalBulletsLeft;

        public Image weaponIconUI;
        public TextMeshProUGUI ammoTextUI;

        public override void OnClose()
        {

        }

        public override void OnInit()
        {

        }

        public override void OnOpen()
        {

        }
    }
}