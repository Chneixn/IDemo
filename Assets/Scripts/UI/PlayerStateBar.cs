using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerStateBar : MonoBehaviour
{
    [Header("HealthBar")]
    [SerializeField] private TextMeshProUGUI healthText;
    [SerializeField] private Slider healthSlider;
    [SerializeField] private float _health;
    [SerializeField] private float healthDelay;

    [Header("PowerBar")]
    [SerializeField] private Image powerImage;
    [SerializeField] private TextMeshProUGUI power;
    [SerializeField] private float powerDelay;
    [SerializeField] private float _power;

    [Space]
    [Range(0f, 1f)]
    [SerializeField] private float _changSpeed;

    private void Start()
    {
        healthDelay = 100;
        healthSlider.value = 1.0f;
        healthText.SetText(string.Format("HP {0}", Mathf.CeilToInt(healthDelay)));

        // powerDelay = 100;
    }

    private void Update()
    {
        if (healthDelay > _health)
        {
            healthDelay -= _changSpeed;
            healthText.SetText(string.Format("HP {0}", Mathf.CeilToInt(healthDelay)));
            healthSlider.value = healthDelay / 100;
        }

        if (healthDelay < _health)
        {
            healthDelay += _changSpeed;
            healthText.SetText(string.Format("HP {0}", Mathf.CeilToInt(healthDelay)));
            healthSlider.value = healthDelay / 100;
        }

        if (_health == 0)
        {   
            healthText.SetText(string.Format("HP {0}", _health));
            healthSlider.value = 0;
        }

        // if (powerDelay > _power)
        // {
        //     powerDelay -= Time.deltaTime;
        //     powerImage.fillAmount = powerDelay;
        // }
    }

    /// <summary>
    /// 接收Health数值
    /// </summary>
    /// <param name="currentHealth">当前生命值</param>
    public void OnHealthChange(float currentHealth)
    {
        _health = currentHealth;
    }

    public void OnPowerChange(float currentPower)
    {
        _power = currentPower;
    }
}
