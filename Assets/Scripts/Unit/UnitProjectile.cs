using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitProjectile : NetworkBehaviour
{
    [SerializeField] private Rigidbody rb = null;
    [SerializeField] private float destroyAfterSeconds = 5f;
    [SerializeField] private float launchForce = 10f;
    
    void Start()
    {
        rb.velocity = transform.forward * launchForce;
    }

    // to call DestroySelf() after seconds  we can made an override OnStartMethod()
    // So when the Server starts this object will call this up after seconds 
    public override void OnStartServer()
    {
        Invoke(nameof(DestroySelf), destroyAfterSeconds);
        base.OnStartServer();
    }



    // make a method on the server only to destroy the strayed projectile
    [Server]
    private void DestroySelf()
    {

        NetworkServer.Destroy(gameObject);
    }

}
