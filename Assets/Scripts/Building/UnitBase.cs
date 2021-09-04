using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitBase : NetworkBehaviour
{
    [SerializeField] private Health health = null;

    // everythime a base is spawned will be added to a list and will store all the remaining players
    // so whenever one of them despawned, either leave the server or die, then that is raised.

    [SerializeField] public static event Action<UnitBase> ServerOnBaseSpawned;
    [SerializeField] public static event Action<UnitBase> ServerOnBaseDespawned;
    #region Server
    
    public override void OnStartServer() // subscribing
    {
        health.ServerOnDie += ServerHandleDie;
        ServerOnBaseSpawned?.Invoke(this);
    }

    public override void OnStopServer()// un-subscribing
    {
        ServerOnBaseDespawned?.Invoke(this);
        health.ServerOnDie -= ServerHandleDie;        
    }

    // Below is the implementation of the event invoked in Health object when health is Zero
    private void ServerHandleDie()
    {
        NetworkServer.Destroy(gameObject);
    }
    #endregion // server

    #region Client

    #endregion
}
