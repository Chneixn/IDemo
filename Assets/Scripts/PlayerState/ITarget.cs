using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class ITarget : MonoBehaviour, IDamageable
{
    public float downDuraction = 1f;
    public float upDuraction = 1f;
    public float cd = 2f;
    public Transform model;

    [SerializeField] private bool isDowned = false;

    public AudioClip s_down;
    public AudioClip s_up;

    private new AudioSource audio;

    private CancellationTokenSource cancelRecovery;
    private const float DELTA = 0.02f;

    public virtual void OnTakeDamage(float damage) => TryDown();

    public void TakeDamage(float damage, DamageType type, Vector3 direction)
    {
        TryDown();
    }

#if UNITY_EDITOR
    [ContextMenu("Try Down")]
#endif
    private void TryDown()
    {
        if (audio == null) audio = GetComponent<AudioSource>();
        if (isDowned) return;
        if (cancelRecovery != null)
        {
            cancelRecovery.Cancel();
            cancelRecovery.Dispose();
        }
        cancelRecovery = new CancellationTokenSource();
        Down().Forget();
    }

    private async UniTaskVoid Down()
    {
        isDowned = true;
        audio.PlayOneShot(s_down);
        float delta = -90f * DELTA / downDuraction;
        float timer = 0f;
        while (timer < downDuraction)
        {
            model.Rotate(new Vector3(delta, 0, 0));
            timer += DELTA;
            Debug.Log(model.transform.localEulerAngles.x);
            await UniTask.WaitForSeconds(DELTA);
        }

        await UniTask.WaitForSeconds(cd);

        isDowned = false;
        Recovery(cancelRecovery.Token).Forget();
    }

    private async UniTask Recovery(CancellationToken cancelToken)
    {
        audio.PlayOneShot(s_up);
        float delta = 90f * DELTA / upDuraction;
        float timer = 0f;
        while (timer < upDuraction)
        {
            model.Rotate(new Vector3(delta, 0, 0));
            timer += DELTA;
            Debug.Log(model.transform.localEulerAngles.x);
            await UniTask.WaitForSeconds(DELTA, cancellationToken: cancelToken);
        }
    }
}
