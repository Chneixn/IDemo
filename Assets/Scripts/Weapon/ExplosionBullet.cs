using System.Collections;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class ExplosionBullet : Bullet
{
    [Header("爆炸设置")]
    public bool StartCoutingOnAwake;
    public bool explodeOnTuch;      //是否碰撞即爆炸
    public float explosionRange;    //爆炸范围
    [Tooltip("爆炸时影响的最大物体数量(默认为30)")]
    public int damagableLimit = 30;
    [SerializeField] protected bool enableExplosionForce;
    [SerializeField] protected float explosionForce;    //爆炸推力 = 1200
    [SerializeField] protected float VFX_Time;

    [SerializeField] protected IPoolableParticleSystem VFXPrefab;
    [SerializeField] protected AudioClip[] explosionSounds;
    protected AudioSource audioSource;

    private Collider[] damagables;

    protected override void Awake()
    {
        base.Awake();
        audioSource = GetComponent<AudioSource>();
        damagables = new Collider[damagableLimit];
    }

    public override void OnGet()
    {
        base.OnGet();
        if (StartCoutingOnAwake)
            StartCoroutine(DelyToExplode(lifeTime));
    }

    public override void OnRecycle()
    {
        audioSource.Stop();
    }

    protected virtual IEnumerator DelyToExplode(float lifeTime)
    {
        yield return new WaitForSeconds(lifeTime);
        Explode();
    }

    protected virtual void Explode()
    {
        PlayVFX();
        PlayExplodeSound();

        if (explosionRange != 0)
        {
            // 爆炸范围检测
            var count = Physics.OverlapSphereNonAlloc(transform.position, explosionRange, damagables, damagableLayer);
            if (count > 0)
            {
                for (int i = 0; i < count; i++)
                {
                    // 爆炸伤害
                    if (damagables[i].TryGetComponent(out IDamageable target))
                        target.TakeDamage(Damage, DamageType.Explosion, damagables[i].transform.position - transform.position);

                    // 爆炸推动
                    if (enableExplosionForce)
                    {
                        if (damagables[i].TryGetComponent(out Rigidbody rb))
                            rb.AddExplosionForce(explosionForce, transform.position, explosionRange);
                        if (damagables[i].TryGetComponent(out CharacterControl cc))
                        {
                            // 让你飞起来！
                            var dir = cc.Motor.CharacterTransformToCapsuleCenter - transform.position;
                            cc.InternalVelocity = dir.normalized * explosionForce;
                        }
                    }
                }
            }
        }

        GameObjectPoolManager.RecycleItem(this);
    }

    private void PlayVFX()
    {
        if (VFXPrefab == null) return;

        var vfx = GameObjectPoolManager.GetItem<IPoolableParticleSystem>(VFXPrefab, transform.position, transform.rotation);
        TimerManager.CreateTimeOut(VFX_Time, () => GameObjectPoolManager.RecycleItem(vfx));
    }

    protected virtual void PlayExplodeSound()
    {
        if (audioSource == null) return;
        audioSource.clip = explosionSounds[Random.Range(0, explosionSounds.Length)];
        audioSource.Play();
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explosionRange);
    }
#endif
}
