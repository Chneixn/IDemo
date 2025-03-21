using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UISystem
{
    public class PlayerStateBar : IUIView
    {
        [Header("Health Bar")]
        [SerializeField] private Image hpFill;
        [SerializeField] private Image hpFillDelay;
        [SerializeField] private float hp;
        [SerializeField, Range(0f, 1f)] private float changSpeed;

        public void UpdateHPBar()
        {
            hpFill.fillAmount = hp;
            StopAllCoroutines();
            StartCoroutine(HealthDelay());
        }

        private IEnumerator HealthDelay()
        {
            while (hpFillDelay.fillAmount != hpFill.fillAmount)
            {
                // 血少了
                if (hpFillDelay.fillAmount - hpFill.fillAmount > 0) hpFillDelay.fillAmount -= changSpeed * 0.01f;
                else hpFillDelay.fillAmount = hpFill.fillAmount;
                yield return new WaitForSeconds(Time.deltaTime);
            }
        }

        public override void OnInit()
        {
            hp = 100f;
            hpFill.fillAmount = 1f;
            hpFillDelay.fillAmount = 1f;
        }

        public override void OnOpen()
        {

        }

        public override void OnClose()
        {

        }
    }
}