using UnityEngine;

public class WoodDestruction : MonoBehaviour
{
    public float destructionThreshold = 10f; // Adjust this value based on testing
    public GameObject destructionEffect; // Assign a particle effect prefab for destruction
    public float effectLifetime = 12f; // Time before destruction effect is removed
    public bool birdDestructable = true; // Determines if wood can be destroyed by birds, true by default

    void OnCollisionEnter(Collision collision)
    {
        if (!birdDestructable && collision.gameObject.layer == LayerMask.NameToLayer("Bird"))
        {
            return;
        }

        float collisionForce = collision.impulse.magnitude;
        AudioManager.Instance.PlayImpact(transform.position, collisionForce);

        if (collisionForce > destructionThreshold)
        {
            if (destructionEffect != null)
            {
                GameObject effect = Instantiate(destructionEffect, transform.position, Quaternion.identity);
                Destroy(effect, effectLifetime);
                AudioManager.Instance.PlayExplosion(transform.position);
            }
            Destroy(gameObject);
        }
    }
}