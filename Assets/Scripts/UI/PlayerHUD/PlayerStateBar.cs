using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UISystem
{
    public class PlayerStateBar : IUIView
    {
        [Header("HealthBar")]
        [SerializeField] private TextMeshProUGUI healthText;
        [SerializeField] private Slider healthSlider;
        [SerializeField] private float hp;
        [SerializeField] private float healthDelay;

        [Space]
        [Range(0f, 1f)]
        [SerializeField] private float changSpeed;

        public void UpdateView()
        {
            if (healthDelay > hp)
            {
                healthDelay -= changSpeed;
                healthText.SetText(string.Format("HP {0}", Mathf.CeilToInt(healthDelay)));
                healthSlider.value = healthDelay / 100;
            }

            if (healthDelay < hp)
            {
                healthDelay += changSpeed;
                healthText.SetText(string.Format("HP {0}", Mathf.CeilToInt(healthDelay)));
                healthSlider.value = healthDelay / 100;
            }

            if (hp == 0)
            {
                healthText.SetText(string.Format("HP {0}", hp));
                healthSlider.value = 0;
            }
        }

        public override void OnInit()
        {
            healthDelay = 100;
            healthSlider.value = 1.0f;
            healthText.SetText(string.Format("HP {0}", Mathf.CeilToInt(healthDelay)));
        }

        public override void OnOpen()
        {

        }

        public override void OnClose()
        {

        }
    }
}