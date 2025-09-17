using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class RecoilGunshoot : MonoBehaviour
{
    public float offset =90;

    public float rotateSpeed;

    [Header("Shoot")]
    public Transform firePoint;
    public GameObject bulletPrefab;
    public GameObject bombPrefab;
    public float bulletSpeed;
    public float bombSpeed;


    [Header("Recoil")]
    private Player player;
    public float recoilSpped;

    //Hanlde switch bomb and gun
    bool isRayReachGround;
    [SerializeField] private float rayDistance = 2f;

    void Start()
    {
        player = GetComponentInParent<Player>();
    }
    void Update()
    {
        if (player.IsDead()) return;

        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

     
        Vector2 parentPos = transform.parent.position;

        
        Vector2 dir = (mousePos - parentPos).normalized;

       
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg - offset;

        // Rotate gun around parent center
        transform.rotation = Quaternion.Euler(0, 0, angle);

        if(Input.GetMouseButtonDown(0))
        {
            if (isRayReachGround)
            {
            ShootBullet();

            }
            else
            {
                ShootBomb();
            }
        }




        int mask = LayerMask.GetMask("Ground", "Destroyable");
        RaycastHit2D ray = Physics2D.Raycast(firePoint.position, firePoint.up, rayDistance, mask);

        if (ray.collider != null)
        {
            isRayReachGround = true;
        }
        else
        {
            isRayReachGround = false;
        }
    }   

    void ShootBullet()
    {
        
        //GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation); 
        //bullet.GetComponent<Rigidbody2D>().velocity = firePoint.up * bulletSpeed ;


        // Calculate shooting direction
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 parentPos = player.transform.position;
        Vector2 dir = (mousePos - parentPos).normalized;

        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
        bullet.GetComponent<Rigidbody2D>().velocity = dir * bulletSpeed;


        player?.GetComponent<Rigidbody2D>().AddForce(-dir * recoilSpped, ForceMode2D.Impulse);
    

        if (bullet.GetComponent<Collider2D>().IsTouchingLayers(LayerMask.GetMask("Ground")))
        {
            Destroy(bullet);
        }
    }

    void ShootBomb()
    {
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 parentPos = player.transform.position;
        Vector2 dir = (mousePos - parentPos).normalized;

        GameObject bullet = Instantiate(bombPrefab, firePoint.position, firePoint.rotation);
        bullet.GetComponent<Rigidbody2D>().velocity = dir * bombSpeed;
    }

    private void OnDrawGizmos()
    {

        int mask = LayerMask.GetMask("Ground", "Destroyableyy");
        RaycastHit2D ray = Physics2D.Raycast(firePoint.position, firePoint.up, rayDistance, mask);

        if (ray.collider != null)
        {
            Gizmos.DrawLine(firePoint.position, ray.point); // stop at hit
        }
        else
        {
            Gizmos.DrawLine(firePoint.position, firePoint.position + firePoint.up * rayDistance);
        }
    }


}
