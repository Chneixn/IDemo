using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityUtils;


namespace UISystem
{
    public class CrossHair : IUIView
    {
        public Sprite crossHairSprite;
        public Sprite onHitSprite;
        [SerializeField] private float unenableHitUITime;

        [SerializeField] private Image crossHairUI;
        [SerializeField] private Image onHitUI;

        public void ChangeCrossHair(Sprite sprite)
        {
            crossHairSprite = sprite;
            crossHairUI.sprite = sprite;
        }

        public void ChangeHitSprite(Sprite sprite)
        {
            onHitSprite = sprite;
            onHitUI.sprite = sprite;
        }

        public void OnHit()
        {
            onHitUI.gameObject.SetActive(true);
            StopAllCoroutines();
            StartCoroutine(UnenableHitUI());
        }

        private IEnumerator UnenableHitUI()
        {
            yield return new WaitForSeconds(unenableHitUITime);
            onHitUI.gameObject.SetActive(false);
        }

        public override void OnClose()
        {
            gameObject.SetActiveSafe(false);
        }

        public override void OnInit()
        {

        }

        public override void OnOpen()
        {
            gameObject.SetActiveSafe(true);
        }
    }
}