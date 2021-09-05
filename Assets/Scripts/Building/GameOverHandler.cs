using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameOverHandler : NetworkBehaviour
{

    private List<UnitBase> bases = new List<UnitBase>(); 

    #region Server

    public override void OnStartServer() // subscribing for Spawned & Despawned events hanlders 
    {
        UnitBase.ServerOnBaseSpawned += ServerHandleBaseSpawned;
        UnitBase.ServerOnBaseDespawned += ServerHandleBaseDespawned;
    }

    public override void OnStopServer() // un-subscribing after they were removed or killed for Spawned & Despawned events hanlders 
    {
        Debug.Log("Server Stop");
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
