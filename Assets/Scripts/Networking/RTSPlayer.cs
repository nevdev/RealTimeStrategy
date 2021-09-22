using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System;

public class RTSPlayer : NetworkBehaviour
{
    // we're not changing any size here so we should use an fixed sized  container
    // collection contigiuos of objects
    [SerializeField] private Building[] buildings = new Building[0]; 
    // zero here because it is actually predifine in the inspector componemt 

    // Storing Units and Buildings in Lists
    private List<Unit> myUnits = new List<Unit>();
    private List<Building> myBuildings = new List<Building>();


    // Getters - a way externally to grab these objects
    public List<Unit> GetMyUnits()
    {
        return myUnits;
    }

    public List<Building> GetMyBuildings()
    {
        return myBuildings;
    }

    public object NetworkClient { get; internal set; }

    // This is a server side process

    // Subscribe handlers 

    #region Server
    public override void OnStartServer()
    {
        Unit.ServerOnUnitSpawned += ServerHandleUnitSpawned;
        Unit.ServerOnUnitDespawned += ServerHandleUnitDespawned;
        Building.ServerOnBuildingSpawned += ServerHandleBuildingSpawned;
        Building.ServerOnBuildingDespawned += ServerHandleBuildingDespawned;
    }

    public override void OnStopServer()
    {
        Unit.ServerOnUnitSpawned -= ServerHandleUnitSpawned;
        Unit.ServerOnUnitDespawned -= ServerHandleUnitDespawned;
        Building.ServerOnBuildingSpawned -= ServerHandleBuildingSpawned;
        Building.ServerOnBuildingDespawned -= ServerHandleBuildingDespawned;
    }

    [Command]
    public void CmdTryPlaceBuilding(int buildingId, Vector3 point)
    {
        Debug.Log($" buildingId:{buildingId} 4444444444444 ");
        Building buildingToPlace = null;
        // Let start by assuming it is the incorrect one
        // but we need to loop inot the least to check them each item
        foreach(Building building in buildings)
        {// temprary building var takes an item from buildings array and work on it in below statements block
            Debug.Log($" buildingId in array:{building} 55555555555 ");
            if (building.GetId() == buildingId)
            {
                buildingToPlace = building;
                break; //if we found it we're sure to be here so break the loop
            }
        }
        // if the ID is invalid return!
        if(buildingToPlace == null) { Debug.Log(" buildingToPlace == null 3333333333"); return;  }

        // Below it'll just spawn in in the server
        // Spawning the gameobject of building into point(Vector3 position) with the building's transform rotation
        GameObject buildingInstance = 
        Instantiate(buildingToPlace.gameObject, point, buildingToPlace.transform.rotation);// in this case we're not using Quaternion.identity which means "no rotation" - the object is perfectly aligned with the world or parent axes.

        // Now spawn it into network by the person who ownes this player(connection to client :)
        NetworkServer.Spawn(buildingInstance, connectionToClient);
        // it will apprear on all clients machine
    }

    private void ServerHandleUnitSpawned(Unit unit)
    {
        // if the unit is the has the same connection id as with the client's connection id means it is his unit...
        if (unit.connectionToClient.connectionId != this.connectionToClient.connectionId) { return; }
        // ...then we can added in our list
        myUnits.Add(unit);
    }
    private void ServerHandleUnitDespawned(Unit unit)
    {
        if(unit.connectionToClient.connectionId != this.connectionToClient.connectionId) { return; }
        myUnits.Remove(unit);
    }

    private void ServerHandleBuildingSpawned(Building building)
    {
        if(building.connectionToClient.connectionId != this.connectionToClient.connectionId) { return; }
        myBuildings.Add(building);
    }

    private void ServerHandleBuildingDespawned(Building building)
    {
        if(building.connectionToClient.connectionId != this.connectionToClient.connectionId) { return; }
        myBuildings.Remove(building);
    }

    #endregion

    #region Client
    public override void OnStopAuthority()
    {
        if (NetworkServer.active) return;
        Unit.AuthorityOnUnitSpawned += AuthorityHandleUnitSpawned;
        Unit.AuthorityOnUnitDespawned += AuthorityHandleUnitDespawned;
        Building.AuthorityOnBuildingSpawned += AuthorityHandleBuildingSpawned;
        Building.AuthorityOnBuildingDespawned += AuthorityHandleBuildingDespawned;
    }

    public override void OnStopClient()
    {
        if (!isClientOnly || !hasAuthority) { return; }
        Unit.AuthorityOnUnitSpawned -= AuthorityHandleUnitSpawned;
        Unit.AuthorityOnUnitDespawned -= AuthorityHandleUnitDespawned;
        Building.AuthorityOnBuildingSpawned -= AuthorityHandleBuildingSpawned;
        Building.AuthorityOnBuildingDespawned -= AuthorityHandleBuildingDespawned;
    }

    private void AuthorityHandleUnitSpawned(Unit unit)
    {
        myUnits.Add(unit);
    }

    private void AuthorityHandleUnitDespawned(Unit unit)
    {
        myUnits.Remove(unit);
    }

    private void AuthorityHandleBuildingSpawned(Building building)
    {
        myBuildings.Add(building);
    }

    private void AuthorityHandleBuildingDespawned(Building building)
    {
        myBuildings.Remove(building);
    }

    #endregion
}
