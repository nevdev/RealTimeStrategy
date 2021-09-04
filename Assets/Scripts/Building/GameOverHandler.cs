using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameOverHandler : NetworkBehaviour
{

    private List<UnitBase> bases = new List<UnitBase>(); 

    #region Server

    public override void OnStartServer() // subscribing
    {
        UnitBase.ServerOnBaseSpawned += ServerHandleBaseSpawned;
        UnitBase.ServerOnBaseSpawned += ServerHandleBaseDespawned;
    }

    public override void OnStopServer() // un-subscribing
    {
        UnitBase.ServerOnBaseSpawned -= ServerHandleBaseSpawned;
        UnitBase.ServerOnBaseSpawned -= ServerHandleBaseDespawned;
    }

    [Server] // server only methods
    private void ServerHandleBaseSpawned(UnitBase unitBase)
    {
        Debug.Log("Adding Unit");
        bases.Add(unitBase);
    }

    [Server] // server only methods
    private void ServerHandleBaseDespawned(UnitBase unitBase)
    {
        Debug.Log("bases.Remove");
        Debug.Log($"Before Removing:{bases.Count}");
        bases.Remove(unitBase);
        Debug.Log($"After Removing:{bases.Count}");
        if (bases.Count !=  1) { return; }
        Debug.Log("Game Over");
    }

    #endregion

    #region Client

    #endregion
}
