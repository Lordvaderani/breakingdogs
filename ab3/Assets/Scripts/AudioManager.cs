using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Audio Sources")]
    [SerializeField] private AudioSource chargingSource;
    [SerializeField] private float chargingMinPitch = 0.8f;
    [SerializeField] private float chargingMaxPitch = 1.2f;
    [SerializeField] private AudioSource shootSource;
    [SerializeField] private AudioSource meowSource;
    [SerializeField] private AudioSource impactSource;
    [SerializeField] private AudioSource explosionSource;
    [SerializeField] private AudioSource tntExplosionSource;

    [Header("Audio Settings")]
    [SerializeField] private float minPitch = 0.8f;
    [SerializeField] private float maxPitch = 1.2f;
    [SerializeField] private float minImpactVolume = 0.2f;
    [SerializeField] private float maxImpactVolume = 1.0f;
    [SerializeField] private float impactForceMultiplier = 0.1f;
    [SerializeField] private float explosionVolume = 1f;
    [SerializeField] private float impactVolume = 1f;

    [Header("Distance Sound Settings")]
    [SerializeField] private float maxHearingDistance = 50f;
    [SerializeField] private float minHearingDistance = 5f;

    private Transform mainCamera;
    private Transform currentCatBall;

    private void Awake()
    {
        // Singleton pattern implementation
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        mainCamera = Camera.main.transform;
        meowSource.spatialBlend = 1f; // Make the sound 3D
        meowSource.rolloffMode = AudioRolloffMode.Linear;
        meowSource.minDistance = minHearingDistance;
        meowSource.maxDistance = maxHearingDistance;
    }

    void Update()
    {
        if (currentCatBall != null && meowSource.isPlaying)
        {
            meowSource.transform.position = currentCatBall.position;
        }
    }

    public void PlayCharging()
    {
        if (chargingSource != null)
        {
            chargingSource.pitch = Random.Range(chargingMinPitch, chargingMaxPitch);
            chargingSource.Play();
        }
    }

    public void StopCharging()
    {
        if (chargingSource != null)
            chargingSource.Stop();
    }

    public void PlayShoot()
    {
        if (shootSource != null)
            shootSource.Play();
    }

    public void StartCatMeow(Transform catTransform)
    {
        if (meowSource != null)
        {
            currentCatBall = catTransform;
            // Set random pitch once at start
            float initialPitch = Random.Range(minPitch, maxPitch);
            meowSource.pitch = initialPitch;
            meowSource.loop = true;
            meowSource.Play();
        }
    }

    public void StopCatMeow()
    {
        currentCatBall = null;
        if (meowSource != null)
        {
            meowSource.Stop();
        }
    }

    public void PlayImpact(Vector3 position, float collisionForce)
    {
        if (impactSource != null)
        {
            AudioSource.PlayClipAtPoint(impactSource.clip, position, impactVolume);
        }
    }

    public void PlayExplosion(Vector3 position)
    {
        if (explosionSource != null)
        {
            AudioSource.PlayClipAtPoint(explosionSource.clip, position, explosionVolume);
        }
    }

    public void PlayTNTExplosion()
    {
        if (tntExplosionSource != null)
        {
            tntExplosionSource.spatialBlend = 0f; // 2D sound
            tntExplosionSource.volume = 1f;
            tntExplosionSource.Play();
        }
    }
}