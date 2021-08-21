using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class RTSNetworkManager : NetworkManager
{
    [SerializeField] private GameObject unitSpawnerPrefab = null;

    public override void OnServerAddPlayer(NetworkConnection conn)
    {
        base.OnServerAddPlayer(conn);

        // this will spawn the base(cube) for us, using the roundrobin method
        // and the spawnPoint obkects on the scene which as a component have
        // the Network Start Position. The UnitSpawner is istatiated using these
        // start points positions

        GameObject unitSpawnerInstance = Instantiate(
            unitSpawnerPrefab, 
            conn.identity.transform.position, 
            conn.identity.transform.rotation);

        // After it is instatiated on the server into 'unitSpawnerInstance'
        // below will spawn the instatiation into all clients and give 
        // ownership OR authority to the origin client(who clicked)
        // so we are now own our base(cube).

        NetworkServer.Spawn(unitSpawnerInstance, conn);

    }
}
