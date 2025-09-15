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

    private Camera mainCamera;
    private Vector3 originalGunScale;
    private PlayerInput playerInput;
    private InputAction shootAction;
    private float lastFireTime;

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

        // Get shooting input
        playerInput = GetComponent<PlayerInput>();
        if (playerInput == null && playerTransform != null)
            playerInput = playerTransform.GetComponent<PlayerInput>();

        if (playerInput != null)
            shootAction = playerInput.actions["Shoot"];
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
            Shoot();
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
}