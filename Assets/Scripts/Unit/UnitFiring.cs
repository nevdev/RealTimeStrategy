using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitFiring : NetworkBehaviour
{
    [SerializeField] private Targeter targeter = null;
    [SerializeField] private GameObject projectilePrefab = null;
    [SerializeField] private Transform projectileSpawnPoint = null;
    [SerializeField] private float fireRange = 5f;
    [SerializeField] private float fireRate = 1f;
    [SerializeField] private float rotationSpeed = 20f;

    private float lastFireTime;



    /** We can't stop unity actually calling Update method.
    So by saying it's a sort of a callback, it just won't log warnings every single frame when it tries
    to call  on clients. But it will have the end goal of stopping clients actually running this logic.
    */
    
    [ServerCallback]
    void Update()
    {
        Targetable target = targeter.GetTarget();
        if(target == null) { return; }

        if(!CanFireAtTarget()) { return; }
        //first we need to know where we need to aim, then we can rotate between our current rotationa dn the target rotation.
        Quaternion targetRotation = 
            Quaternion.LookRotation(target.transform.position - transform.position);
        //.. this gives us a vector poiting towrds our enemy
        transform.rotation = 
            Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

        // if 1 / 1 = 1 then it will fire every 1 Sec. If 1 / 2 = 0.5 will fire every half a second
        if (Time.time > (1 / fireRate) + lastFireTime) 
        {
            // Before Instatiate the projectile since other tank can be lightly smaller or shorter in height whilst the barrel can be pointing upward
            //we need to calculate the exact rotation 
            Quaternion projectileRotation =  // AimAtPoint is the actual centre of the other tank or of building(spawnpint) - (Minus) from where the projectile is being spawed from
                    Quaternion.LookRotation(target.GetAimAtPoint().position - projectileSpawnPoint.position);

            // this only spawns on the server
            GameObject projectileInstance = Instantiate(projectilePrefab, projectileSpawnPoint.position, projectileRotation);
            
            

            // So we want to spawn this on the network. 
            NetworkServer.Spawn(projectileInstance, connectionToClient); // connection To Client is whoeverowns this particulat unit firing script

            lastFireTime = Time.time;

        }
    }

    [Server]
    private bool CanFireAtTarget()
    {       
        // we are using squared and the other method (squareroot is less efficient to contimously be called from an Update method
        // if the range is equal or less from the target transform position less the current transform position (Squared), fire!
        return (targeter.GetTarget().transform.position - transform.position).sqrMagnitude 
                    <= fireRange * fireRange;// range squared
    }
}
