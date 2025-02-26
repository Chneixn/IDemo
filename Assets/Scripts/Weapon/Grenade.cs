using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grenade : MonoBehaviour
{
    [SerializeField] private float throwForce;
    [SerializeField] private float pullPinDely;
    [SerializeField] private float throwDely;
    public float timeToHolster;
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private IPoolableObject grenadePrefab;
    [SerializeField] private AudioClip pullPinSound;
    [SerializeField] private AudioClip throwGrenadeSound;
    [SerializeField] private SkinnedMeshRenderer grenadeMesh;

    public void Enable()
    {
        if (grenadeMesh.enabled != true)
            grenadeMesh.enabled = true;
    }

    //void Explode()
    //{
    //    //Apply camera shake
    //    GameObject mainCamera = GameObject.FindGameObjectWithTag("MainCamera");

    //    //Get all other colliders inside contact radius
    //    Collider[] collidersToDestroy = Physics.OverlapSphere(transform.position, radius);

    //    foreach(Collider nearbyObject in collidersToDestroy)
    //    {
    //        DestructibleObject destructible = nearbyObject.GetComponent<DestructibleObject>();

    //        if(destructible != null)
    //        {
    //            //Instantiate shattered object version that was been hit
    //            destructible.FractureObject();
    //        }
    //    }

    //    Collider[] collidersToMove = Physics.OverlapSphere(transform.position, radius);

    //    foreach(Collider nearbyObject in collidersToMove)
    //    {
    //        if(nearbyObject.gameObject.tag == "Destructible" || nearbyObject.gameObject.tag == "Barrel")
    //        {
    //            Rigidbody rb = nearbyObject.GetComponent<Rigidbody>();

    //            if (rb != null)
    //            {
    //                //Apply physics in all other object parts that was been hit
    //                rb.isKinematic = false;
    //                rb.AddExplosionForce(force, transform.position, radius);
    //            }
    //        }
    //    }

    //    //Disable granade mesh on player's hand after throw it
    //    grenadeMesh.enabled = false;
    //    Destroy(gameObject, 2.5f);
    //}

    public IEnumerator SpawnGrenade()
    {
        yield return new WaitForSeconds(throwDely);
        grenadeMesh.enabled = false;
        if (grenadePrefab != null && spawnPoint != null)
        {
            ExplosionBulletSet _grenade = GameObjectPoolManager.GetItem<ExplosionBulletSet>(grenadePrefab.GetComponent<ExplosionBulletSet>());
            _grenade.GetComponent<Rigidbody>().AddForce(spawnPoint.transform.forward * throwForce, ForceMode.Impulse);
        }
        
        yield break;
    }

    public IEnumerator PlayGrenadeSound(AudioSource audioSource)
    {
        yield return new WaitForSeconds(pullPinDely);
        audioSource.clip = pullPinSound;
        audioSource.Play();
        yield return new WaitForSeconds(throwDely - pullPinDely);
        audioSource.clip = throwGrenadeSound;
        audioSource.Play();
        yield break;
    }
}
