using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class ITarget : MonoBehaviour, IDamageable
{
    public float downDuraction = 0.5f;
    public float upDuraction = 1f;
    public Transform model;

    [SerializeField] private bool isDowned = false;

    public AudioClip s_down;
    public AudioClip s_up;

    private new AudioSource audio;
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
            if (model.localEulerAngles.x < -90) break;
            // Debug.Log(model.transform.localEulerAngles.x);
            await UniTask.WaitForSeconds(DELTA);
        }
        Recovery().Forget();
    }

    private async UniTask Recovery()
    {
        audio.PlayOneShot(s_up);
        float delta = 90f * DELTA / upDuraction;
        float timer = 0f;
        while (timer < upDuraction)
        {
            model.Rotate(new Vector3(delta, 0, 0));
            timer += DELTA;
            // Debug.Log(model.transform.localEulerAngles.x);
            await UniTask.WaitForSeconds(DELTA);
        }
        model.localEulerAngles = Vector3.zero;
        isDowned = false;
    }
}
