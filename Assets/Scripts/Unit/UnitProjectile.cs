using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitProjectile : NetworkBehaviour
{
    [SerializeField] private Rigidbody rb = null;
    [SerializeField] private int damageToDeal = 20;
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

    [ServerCallback]
    private void OnTriggerEnter(Collider other)
    {
        // we are getting other collider and getting its network identity information...
        if (other.TryGetComponent<NetworkIdentity>(out NetworkIdentity networkIdentity))
        {
            // ... with that information we are reading we are checking the connoection to the cliens
            if (networkIdentity.connectionToClient == connectionToClient) { return; }
            // ... and if the connection to the client matches this projectile connectionToClient means that the collider we hit is our own.
            // so return as we do not want to damage our own units.            
        }

        // othwerwise...
        if (other.TryGetComponent<Health>(out Health health))
        {
            health.DealDamage(damageToDeal);
        }
        DestroySelf();
    }

    // make a method on the server only to destroy the strayed projectile
    [Server]
    private void DestroySelf()
    {

        NetworkServer.Destroy(gameObject);
    }

}
