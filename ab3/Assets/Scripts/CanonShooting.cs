using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;


public class CanonShooting : MonoBehaviour
{
    [SerializeField] private GameObject canonBallPrefab;
    [SerializeField] private Transform canonBallSpawnPoint;
    [SerializeField] private Transform barrelTransform;
    [SerializeField] private float rotationSpeed = 10f;
    
    [SerializeField] private int linePoints = 25;
    
    [SerializeField] private float minBallSpeed = 5f;
    [SerializeField] private float maxBallSpeed = 20f;
    [SerializeField] private float maxChargeTime = 3f; // Increased charge time
    [SerializeField] private float trajectoryUpdateRate = 0.02f; // Smooth updates
    [SerializeField] private CinemachineVirtualCamera canonCamera;
    [SerializeField] private CinemachineVirtualCamera canonCamera2; // Second perspective
    [SerializeField] private CinemachineVirtualCamera followCamera;
    [SerializeField] private float cameraFollowTime = 1.5f;
    [SerializeField] private UI userinterface_manager;
    
    private float chargeStartTime;
    private bool isCharging = false;
    private float currentCharge = 0f;
    private bool isUsingFirstCamera = true;

    private LineRenderer lineRenderer;

    void Start()
    {
        lineRenderer = gameObject.AddComponent<LineRenderer>();
        lineRenderer.positionCount = linePoints;
        lineRenderer.startWidth = 0.15f;
        lineRenderer.endWidth = 0.15f;
        lineRenderer.enabled = false;

        // Simple solid line style
        lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        lineRenderer.material.color = new Color(1f, 1f, 1f, 0.7f);
        lineRenderer.useWorldSpace = true;

        // Setup initial camera priorities
        canonCamera.Priority = 10;
        followCamera.Priority = 9;

        // Setup follow camera configuration
        var transposer = followCamera.GetCinemachineComponent<CinemachineTransposer>();
        if (transposer != null)
        {
            transposer.m_FollowOffset = new Vector3(0, 3, -6); // Adjust these values
            transposer.m_XDamping = 1f;
            transposer.m_YDamping = 1f;
            transposer.m_ZDamping = 1f;
        }

        // Disable rotation following
        followCamera.GetCinemachineComponent<CinemachineComposer>().enabled = false;
        followCamera.transform.rotation = Quaternion.Euler(20, 0, 0); // Fixed angle
    }

    void Update()
    {
        // Add camera toggle

        if (userinterface_manager.HasCatAmmo() == false)
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene("Lost");
        }


        if (Input.GetKeyDown(KeyCode.V))
        {
            isUsingFirstCamera = !isUsingFirstCamera;
            canonCamera.Priority = isUsingFirstCamera ? 10 : 8;
            canonCamera2.Priority = isUsingFirstCamera ? 8 : 10;
        }

        // Handle input rotation
        if (Input.GetKey(KeyCode.W)) barrelTransform.Rotate(-Time.deltaTime * rotationSpeed, 0, 0);
        if (Input.GetKey(KeyCode.S)) barrelTransform.Rotate(Time.deltaTime * rotationSpeed, 0, 0);
        if (Input.GetKey(KeyCode.A)) barrelTransform.Rotate(0, -Time.deltaTime * rotationSpeed, 0);
        if (Input.GetKey(KeyCode.D)) barrelTransform.Rotate(0, Time.deltaTime * rotationSpeed, 0);

        // Get current rotation and normalize to [-180, 180] range
        Vector3 currentRotation = barrelTransform.localEulerAngles;
        float xRot = currentRotation.x;
        float yRot = currentRotation.y;

        // Normalize X rotation
        if (xRot > 180f) xRot -= 360f;
        
        // Normalize Y rotation
        //if (yRot > 180f) yRot -= 360f;

        // Apply clamping
        xRot = Mathf.Clamp(xRot, -10f, 35f);
        yRot = Mathf.Clamp(yRot, 140f, 220f);

        // Apply rotation
        barrelTransform.localEulerAngles = new Vector3(xRot, yRot, 0);

        if (Input.GetKeyDown(KeyCode.Space))
        {
            chargeStartTime = Time.time;
            isCharging = true;
            lineRenderer.enabled = true;
            AudioManager.Instance.PlayCharging();
        }

        if (Input.GetKey(KeyCode.Space) && isCharging)
        {
            float holdTime = Time.time - chargeStartTime;
            currentCharge = Mathf.Clamp01(holdTime / maxChargeTime);
            DrawTrajectory();
        }

        if (Input.GetKeyUp(KeyCode.Space))
        {
            AudioManager.Instance.StopCharging();
            AudioManager.Instance.PlayShoot();
            float finalSpeed = Mathf.Lerp(minBallSpeed, maxBallSpeed, currentCharge);
            StartCoroutine(ShootCanonBall(finalSpeed));
            isCharging = false;
            currentCharge = 0f;
            lineRenderer.enabled = false;
        }
    }

    void DrawTrajectory()
    {
        float currentSpeed = Mathf.Lerp(minBallSpeed, maxBallSpeed, currentCharge);
        Vector3[] points = new Vector3[linePoints];
        Vector3 startPosition = canonBallSpawnPoint.position;
        Vector3 startVelocity = canonBallSpawnPoint.forward * currentSpeed;
        
        // Smooth multi-color transition
        Color chargeColor;
        if (currentCharge < 0.33f)
        {
            chargeColor = Color.Lerp(Color.white, Color.green, currentCharge * 3f);
        }
        else if (currentCharge < 0.66f)
        {
            chargeColor = Color.Lerp(Color.green, new Color(1f, 0.5f, 0f), (currentCharge - 0.33f) * 3f); // Orange
        }
        else
        {
            chargeColor = Color.Lerp(new Color(1f, 0.5f, 0f), Color.red, (currentCharge - 0.66f) * 3f);
        }
        
        lineRenderer.material.color = new Color(chargeColor.r, chargeColor.g, chargeColor.b, 0.8f);
        
        // Smooth trajectory calculation
        for (int i = 0; i < linePoints; i++)
        {
            float time = i * trajectoryUpdateRate;
            Vector3 point = startPosition + startVelocity * time;
            point.y += Physics.gravity.y * time * time / 2f;

            points[i] = point;
        }
        
        lineRenderer.SetPositions(points);
    }

    private IEnumerator ShootCanonBall(float speed)
    {
        lineRenderer.enabled = false;
        GameObject canonBall = Instantiate(canonBallPrefab, canonBallSpawnPoint.position, canonBallSpawnPoint.rotation);
        canonBall.transform.Rotate(0, 0, -90f);
        canonBall.tag = "Projectile"; // Tag the launched cat
        
        AudioManager.Instance.StartCatMeow(canonBall.transform);
        
        // Create an empty GameObject to follow
        GameObject followTarget = new GameObject("CameraTarget");
        followTarget.transform.position = canonBall.transform.position;
        
        // Setup smooth follow
        followCamera.Follow = followTarget.transform;
        followTarget.AddComponent<SmoothFollow>().target = canonBall.transform;
        followCamera.Priority = 11; // Ensure follow cam overrides both canon cams
        
        Rigidbody rb = canonBall.GetComponent<Rigidbody>();
        rb.AddForce(canonBallSpawnPoint.forward * speed, ForceMode.Impulse);

        yield return new WaitForSeconds(cameraFollowTime);
        followCamera.Priority = 9;
        Destroy(followTarget, 0.5f);
    }
}

// Add this class to the same file
public class SmoothFollow : MonoBehaviour
{
    public Transform target;
    private Vector3 velocity = Vector3.zero;
    public float smoothTime = 0.3f;

    void LateUpdate()
    {
        if (target != null)
        {
            Vector3 targetPosition = target.position;
            transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, smoothTime);
        }
    }
}