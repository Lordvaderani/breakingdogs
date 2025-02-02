using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateTheCat : MonoBehaviour
{
    [SerializeField] private float initialSpeed = 1000f;
    [SerializeField] private float maxSpeed = 3000f;
    [SerializeField] private float acceleration = 200f;
    [SerializeField] private float destroyDelay = 4f;
    [SerializeField] private UI ui_manager;

    private float currentSpeed;
    private float elapsedTime;
    private bool isDestroyed = false;

    void Start()
    {
        if (!CompareTag("Projectile")) return; // Only initialize if it's a launched cat

        currentSpeed = initialSpeed;
        elapsedTime = 0f;
        StartCoroutine(DestroyAfterDelay());
    }

    private IEnumerator DestroyAfterDelay()
    {
        if (!CompareTag("Projectile")) yield break;

        yield return new WaitForSeconds(destroyDelay);
        if (!isDestroyed)
        {
            DestroyWithEffects();
        }
    }


    void OnTriggerEnter(Collider other)
    {
        // If enters explosion radius
        if (other.CompareTag("Explosion"))
        {
            DestroyWithEffects();
        }
    }

    private void DestroyWithEffects()
    {
        if (!CompareTag("Projectile") || isDestroyed) return;
        ui_manager.DecreaseCatAmmo();
        isDestroyed = true;

        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.StopCatMeow();
        }


        Destroy(gameObject);
    }

    void Update()
    {
        if (!CompareTag("Projectile") || isDestroyed) return;

        elapsedTime += Time.deltaTime;
        currentSpeed = Mathf.Min(initialSpeed + (acceleration * elapsedTime), maxSpeed);
        transform.Rotate(0, Time.deltaTime * currentSpeed, 0);
    }

    void OnDestroy()
    {
        AudioManager.Instance.StopCatMeow();
    }
}