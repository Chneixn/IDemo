using UnityEngine;

namespace Sample
{
    public class Target : MonoBehaviour, IDamageable
    {
        [SerializeField] private GameObject targetObject;
        private Collider m_collider;
        private float downYVelocity = 0f;
        [SerializeField] private float downTime = 2.5f;
        private bool isDown = false;
        private bool isDead = false;

        private void Awake()
        {
            m_collider = targetObject.GetComponent<Collider>();
            m_collider.isTrigger = true;
        }

        private void FixedUpdate()
        {
            if (isDown)
            {
                targetObject.transform.localEulerAngles = new Vector3(Mathf.SmoothDamp(targetObject.transform.localRotation.x, -90f, ref downYVelocity, downTime), 0f, 0f);
            }
            else if (!isDown && isDead)
            {
                targetObject.transform.localEulerAngles = new Vector3(Mathf.SmoothDamp(targetObject.transform.localRotation.x, 90f, ref downYVelocity, downTime), 0f, 0f);
            }
        }

        public void Heal(float healAmount)
        {
            isDown = false;
            isDead = false;
        }

        public void TakeDamage(float damage, DamageType type, Vector3 direction)
        {
            isDown = true;
            isDead = true;
        }
    }
}

