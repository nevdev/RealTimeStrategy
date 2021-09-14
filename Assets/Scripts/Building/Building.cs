using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Building : NetworkBehaviour
{
    [SerializeField] private GameObject buildingPreview= null;
    [SerializeField] private Sprite icon = null;
    [SerializeField] private int id = -1;
    [SerializeField] private int price = 100;

    // Events
    public static event Action<Building> ServerOnBuildingSpawned;
    public static event Action<Building> ServerOnBuildingDespawned;

    public static event Action<Building> AuthorityOnBuildingSpawned;
    public static event Action<Building> AuthorityOnBuildingDespawned;
    
    // Getters
    public GameObject GetBuildingPreview()
    {
        return buildingPreview;
    }
    public Sprite GetIcon()
    {
        return icon;
    }

    public int GetId()
    {
        return id;
    }

    public int GetPrice()
    {
        return price;
    }

    #region Server
    public override void OnStartServer()
    {

        ServerOnBuildingSpawned?.Invoke(this);

    }

    public override void OnStopServer()
    {
        ServerOnBuildingDespawned(this);
    }
    #endregion

    #region Client

    public override void OnStartAuthority()
    {
      //if (!hasAuthority) { return; }
        AuthorityOnBuildingSpawned?.Invoke(this);
    }

    public override void OnStopClient()
    {
        // make sure you have Authority otherwise you raise it for everybody
        if (!hasAuthority) { return; }
        AuthorityOnBuildingDespawned?.Invoke(this);
    }

    #endregion

}
