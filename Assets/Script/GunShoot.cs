using UnityEngine;
using UnityEngine.InputSystem;

public class GunShoot : MonoBehaviour
{
    [Header("Gun Settings")]
    [SerializeField] private Transform playerTransform;
    [SerializeField] private float gunLength = 0.5f;
    [SerializeField] private float pivotRadius = 0.1f;

    [Header("Shooting Settings")]
    [SerializeField] private GameObject bombPrefab;
    [SerializeField] private float bombSpeed = 10f;
    [SerializeField] private float fireRate = 0.5f;

    [Header("Raycast Settings")]
    [SerializeField] private bool enableRaycast = true;
    [SerializeField] private float raycastDistance = 5f;
    [SerializeField] private LayerMask raycastTargets = -1;
    [SerializeField] private LineRenderer lineRenderer;
    [SerializeField] private float rayVisibleTime = 0.1f;

    [Header("Rocket Jump Settings")]
    [SerializeField] private float maxPushForce = 800f; // Maximum push force when very close to wall
    [SerializeField] private float minPushForce = 100f; // Minimum push force when at max range
    [SerializeField] private GameObject explosionEffect; // Visual effect to spawn at wall
    [SerializeField] private LayerMask pushTargets = -1; // What can be pushed (should include Player layer)

    private Camera mainCamera;
    private Vector3 originalGunScale;
    private PlayerInput playerInput;
    private InputAction shootAction;
    private float lastFireTime;
    private Rigidbody2D playerRb;

    void Awake()
    {
        mainCamera = Camera.main;
        originalGunScale = transform.localScale;

        if (playerTransform == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
                playerTransform = player.transform;
        }

        // Get player Rigidbody2D for applying push force
        if (playerTransform != null)
        {
            playerRb = playerTransform.GetComponent<Rigidbody2D>();
        }

        // Get shooting input
        playerInput = GetComponent<PlayerInput>();
        if (playerInput == null && playerTransform != null)
            playerInput = playerTransform.GetComponent<PlayerInput>();

        if (playerInput != null)
            shootAction = playerInput.actions["Shoot"];

        // Setup LineRenderer if not assigned
        if (lineRenderer == null)
        {
            lineRenderer = GetComponent<LineRenderer>();
            if (lineRenderer == null)
            {
                // Create a LineRenderer component
                lineRenderer = gameObject.AddComponent<LineRenderer>();
                SetupLineRenderer();
            }
        }
    }

    void OnEnable()
    {
        if (shootAction != null)
            shootAction.Enable();
    }

    void OnDisable()
    {
        if (shootAction != null)
            shootAction.Disable();
    }

    void Update()
    {
        if (playerTransform != null)
        {
            AimGunAtMouse();
            HandleShooting();
            
            if (enableRaycast)
            {
                UpdateRaycast();
            }
        }
    }

    private void AimGunAtMouse()
    {
        if (mainCamera == null) return;

        Vector3 mouseScreenPos = Input.mousePosition;
        mouseScreenPos.z = mainCamera.nearClipPlane;
        Vector3 mouseWorldPos = mainCamera.ScreenToWorldPoint(mouseScreenPos);

        Vector2 mousePos2D = mouseWorldPos;
        Vector2 playerPos2D = playerTransform.position;
        Vector2 mouseDirection = (mousePos2D - playerPos2D).normalized;

        float mouseAngle = Mathf.Atan2(mouseDirection.y, mouseDirection.x);

        Vector2 dynamicPivotWorld = playerPos2D + new Vector2(
            Mathf.Cos(mouseAngle) * pivotRadius,
            Mathf.Sin(mouseAngle) * pivotRadius
        );

        Vector2 aimDirection = (mousePos2D - dynamicPivotWorld).normalized;
        float aimAngle = Mathf.Atan2(aimDirection.y, aimDirection.x) * Mathf.Rad2Deg;

        Vector3 gunWorldPosition = dynamicPivotWorld + aimDirection * (gunLength * 0.5f);

        transform.position = gunWorldPosition;
        transform.rotation = Quaternion.Euler(0, 0, aimAngle);
        transform.localScale = originalGunScale;
    }

    private void HandleShooting()
    {
        if (shootAction != null && shootAction.WasPressedThisFrame() && Time.time >= lastFireTime + fireRate)
        {
            if (enableRaycast)
            {
                ShootRaycast();
            }
            else
            {
                Shoot();
            }
            lastFireTime = Time.time;
        }
    }

    private void Shoot()
    {
        if (bombPrefab == null) return;

        GameObject bomb = Instantiate(bombPrefab, transform.position, transform.rotation);
        
        Rigidbody2D bombRb = bomb.GetComponent<Rigidbody2D>();
        if (bombRb != null)
        {
            Vector2 shootDirection = transform.right;
            bombRb.velocity = shootDirection * bombSpeed;
        }
    }

    private void ShootRaycast()
    {
        Vector2 rayOrigin = transform.position;
        Vector2 rayDirection = transform.right;

        // Perform the raycast with limited distance
        RaycastHit2D hit = Physics2D.Raycast(rayOrigin, rayDirection, raycastDistance, raycastTargets);

        if (hit.collider != null)
        {
            Debug.Log($"Ray hit: {hit.collider.name} on layer: {LayerMask.LayerToName(hit.collider.gameObject.layer)} at distance: {hit.distance:F2}");
            
            // Handle what happens when ray hits something
            HandleRaycastHit(hit);
            
            // Show visual ray to hit point
            ShowRayVisual(rayOrigin, hit.point);
        }
        else
        {
            // Ray didn't hit anything within range
            Vector2 rayEnd = rayOrigin + rayDirection * raycastDistance;
            ShowRayVisual(rayOrigin, rayEnd);
            Debug.Log($"Ray didn't hit anything within {raycastDistance} units");
        }
    }

    private void HandleRaycastHit(RaycastHit2D hit)
    {
        int hitLayer = hit.collider.gameObject.layer;
        
        // Check if we hit a wall - trigger rocket jump
        if (hitLayer == LayerMask.NameToLayer("Wall"))
        {
            Debug.Log("Wall layer hit by ray - triggering rocket jump!");
            
            // Spawn explosion effect at wall hit point
            SpawnExplosionEffect(hit.point);
            
            // Apply rocket jump force to player
            ApplyRocketJumpForce(hit.point, hit.distance);
        }
        else if (hitLayer == LayerMask.NameToLayer("Enemy"))
        {
            Debug.Log("Enemy layer hit by ray!");
            // Deal damage to enemy
        }
        else
        {
            Debug.Log($"Hit object on layer: {LayerMask.LayerToName(hitLayer)}");
        }
    }

    private void SpawnExplosionEffect(Vector2 explosionPosition)
    {
        if (explosionEffect != null)
        {
            GameObject explosion = Instantiate(explosionEffect, explosionPosition, Quaternion.identity);
            
            // Auto-destroy explosion effect after some time
            Destroy(explosion, 2f);
        }
    }

    private void ApplyRocketJumpForce(Vector2 wallHitPoint, float distanceToWall)
    {
        if (playerRb == null || playerTransform == null) return;

        // Calculate direction from wall to player (opposite of raycast direction)
        Vector2 playerPosition = playerTransform.position;
        Vector2 pushDirection = (playerPosition - wallHitPoint).normalized;

        // Calculate force based on distance to wall
        // Closer to wall = stronger push, further = weaker push
        float normalizedDistance = Mathf.Clamp01(distanceToWall / raycastDistance);
        float pushForce = Mathf.Lerp(maxPushForce, minPushForce, normalizedDistance);

        // Apply the force to player
        playerRb.AddForce(pushDirection * pushForce, ForceMode2D.Impulse);

        Debug.Log($"Rocket jump! Distance: {distanceToWall:F2}, Force: {pushForce:F0}, Direction: {pushDirection}");
    }

    private void UpdateRaycast()
    {
        // Continuous raycast for aiming preview with limited range
        Vector2 rayOrigin = transform.position;
        Vector2 rayDirection = transform.right;

        RaycastHit2D hit = Physics2D.Raycast(rayOrigin, rayDirection, raycastDistance, raycastTargets);

        if (hit.collider != null)
        {
            // Show ray to hit point (red = obstacle)
            ShowAimingRay(rayOrigin, hit.point, Color.red);
        }
        else
        {
            // Show ray to max range (green = clear)
            Vector2 rayEnd = rayOrigin + rayDirection * raycastDistance;
            ShowAimingRay(rayOrigin, rayEnd, Color.green);
        }
    }

    private void ShowRayVisual(Vector3 start, Vector3 end)
    {
        if (lineRenderer != null)
        {
            lineRenderer.enabled = true;
            lineRenderer.SetPosition(0, start);
            lineRenderer.SetPosition(1, end);
            
            // Hide the line after a short time
            StartCoroutine(HideRayAfterTime());
        }
    }

    private void ShowAimingRay(Vector3 start, Vector3 end, Color color)
    {
        if (lineRenderer != null)
        {
            lineRenderer.enabled = true;
            lineRenderer.startColor = color;
            lineRenderer.endColor = color;
            lineRenderer.SetPosition(0, start);
            lineRenderer.SetPosition(1, end);
        }

        // Also show in Scene view for debugging
        Debug.DrawLine(start, end, color);
    }

    private System.Collections.IEnumerator HideRayAfterTime()
    {
        yield return new WaitForSeconds(rayVisibleTime);
        if (lineRenderer != null)
        {
            lineRenderer.enabled = false;
        }
    }

    private void SetupLineRenderer()
    {
        if (lineRenderer != null)
        {
            lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
            lineRenderer.startColor = Color.red;
            lineRenderer.endColor = Color.red;
            lineRenderer.startWidth = 0.05f;
            lineRenderer.endWidth = 0.05f;
            lineRenderer.positionCount = 2;
            lineRenderer.useWorldSpace = true;
            lineRenderer.enabled = false;
        }
    }

    // Visualize raycast range in editor
    void OnDrawGizmosSelected()
    {
        if (enableRaycast)
        {
            Gizmos.color = Color.cyan;
            Vector3 rayEnd = transform.position + transform.right * raycastDistance;
            Gizmos.DrawLine(transform.position, rayEnd);
            
            // Draw a small sphere at max range
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(rayEnd, 0.2f);
        }
    }
}