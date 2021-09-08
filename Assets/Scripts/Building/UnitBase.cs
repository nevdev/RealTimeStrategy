using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitBase : NetworkBehaviour
{
    [SerializeField] private Health health = null;

    // int = ID of the player that died
    public static event Action<int> ServerOnPlayerDie;
    // in health script can listen this event and when player dies and if ID matches the 
    // ID of the person/player with this unit building, then destroy

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
    [Server]
    private void ServerHandleDie()
    {
        // coonectionID as we need to know which Player died so we know thar this UnitBase
        // belongs to that player that died
        ServerOnPlayerDie?.Invoke(connectionToClient.connectionId);

        NetworkServer.Destroy(gameObject);
    }
    #endregion // server

    #region Client

    #endregion
}
