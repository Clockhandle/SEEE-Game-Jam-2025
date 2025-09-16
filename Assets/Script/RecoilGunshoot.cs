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
    public float bulletSpeed;


    [Header("Recoil")]
    private Player player;
    public float recoilSpped;

    void Start()
    {
        player = GetComponentInParent<Player>();
    }
    void Update()
    {
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

     
        Vector2 parentPos = transform.parent.position;

        
        Vector2 dir = (mousePos - parentPos).normalized;

       
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg - offset;

        // Rotate gun around parent center
        transform.rotation = Quaternion.Euler(0, 0, angle);

        if(Input.GetMouseButtonDown(0))
        {
            Shoot();
        }   
    }   

    void Shoot()
    {
        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation); 
        bullet.GetComponent<Rigidbody2D>().velocity =  transform.forward * bulletSpeed ;

        player?.GetComponent<Rigidbody2D>().AddForce(-firePoint.forward * recoilSpped, ForceMode2D.Impulse);        

        if (bullet.GetComponent<Collider2D>().IsTouchingLayers(LayerMask.GetMask("Ground")))
        {
            Destroy(bullet);
        }
    }
    
}
