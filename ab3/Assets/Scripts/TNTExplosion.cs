using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TNTExplosion : MonoBehaviour
{
    public float explosionForceThreshold = 5f; // Minimum force required to trigger explosion
    public GameObject explosionEffect; // Assign an explosion particle effect prefab
    public float explosionRadius = 5f; // Radius of the explosion
    public float explosionForce = 500f; // Force applied to nearby objects
    public float effectLifetime = 12f; // Time before the explosion effect is removed

    void OnCollisionEnter(Collision collision)
    {
        // Calculate the collision impact using impulse
        float collisionForce = collision.impulse.magnitude;

        if (collisionForce >= explosionForceThreshold)
        {
            Explode();
        }
    }

    void Explode()
    {
        // Play loud TNT explosion
        AudioManager.Instance.PlayTNTExplosion();

        // Instantiate the explosion effect at the TNT's position
        if (explosionEffect != null)
        {
            GameObject effect = Instantiate(explosionEffect, transform.position, Quaternion.identity);
            Destroy(effect, effectLifetime); // Destroy the effect after its lifetime
        }

        // Apply explosion force to nearby objects
        Collider[] colliders = Physics.OverlapSphere(transform.position, explosionRadius);
        foreach (Collider nearbyObject in colliders)
        {
            Rigidbody rb = nearbyObject.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.AddExplosionForce(explosionForce, transform.position, explosionRadius);
            }
        }

        // Destroy the TNT object
        Destroy(gameObject);
    }
}