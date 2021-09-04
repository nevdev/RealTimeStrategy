using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.SceneManagement;

public class RTSNetworkManager : NetworkManager
{
    [SerializeField] private GameObject unitSpawnerPrefab = null;
    [SerializeField] private GameOverHandler gameOverHandlerPrefab = null;

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


    // this is called after the screen has changed on the server side will first happen
    public override void OnServerSceneChanged(string sceneName)
    {
      if(SceneManager.GetActiveScene().name.StartsWith("Scene_Map")) // .. tee server says 'is this new scene a Map?'
        {
            // .. if so spawn in the game over handler
            GameOverHandler gameOverHandlerInstance = Instantiate(gameOverHandlerPrefab);// since it is am RTS Network Manager it is only instatiated on Server

            // This will cause a new object to be instantiated from the registered prefab
            NetworkServer.Spawn(gameOverHandlerInstance.gameObject); // since it is a gameobjecthendler and we want it as a gameobject on the scene then reference .gameobject
            // .. and using NetworkServer.Spawn, everyone will get this!
        }
    }
}
