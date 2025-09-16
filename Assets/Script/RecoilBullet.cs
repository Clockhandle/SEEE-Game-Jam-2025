using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RecoilBullet : MonoBehaviour
{
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            Destroy(gameObject);  
        }

        else if(collision.gameObject.layer == LayerMask.NameToLayer("Destroyable"))
        {
            Destroy(collision.gameObject);
            Destroy(gameObject);
        }
    }
}
