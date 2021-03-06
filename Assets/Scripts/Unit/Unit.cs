using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System;

public class Unit : NetworkBehaviour
{
    [SerializeField] private Health health = null;
    [SerializeField] private UnitMovement unitMovement = null;
    [SerializeField] private Targeter targeter = null;
    [SerializeField] private UnityEvent onSelected = null;
    [SerializeField] private UnityEvent onDeselected = null;

    public static event Action<Unit> ServerOnUnitSpawned;
    public static event Action<Unit> ServerOnUnitDespawned;

    public static event Action<Unit> AuthorityOnUnitSpawned;
    public static event Action<Unit> AuthorityOnUnitDespawned;

    


    public UnitMovement GetUnitMovement()
    {
        return unitMovement;
    }

    public Targeter GetTargeter()
    {
        return targeter;
    }

    #region Server


    public override void OnStartServer()
    {
        // if the units are not exixting (null) do not invoke on each server start
        // but if the units are existing and they subscribed to this event, Invoe the ServerOnUnitSpawned
        ServerOnUnitSpawned?.Invoke(this);
        //ServerOnUnitSpawned method is imlpemened on the subscriber script named: RTSPlayerScript
        // because events are unkown who subscribe - i.e. loosley coupled
        health.ServerOnDie += ServerHandleDie;
    }

    public override void OnStopServer()
    {
        // if the units are not exixting (null) do not invoke
        // but if the units are existing and they unsubscribed to this event, Invoke the ServerOnUnitSpawned
        ServerOnUnitDespawned?.Invoke(this);
        //ServerOnUnitSpawned method is imlpemened on the subscriber script named: RTSPlayerScript
        // because events are unkown who subscribe - i.e. loosley coupled
        // Note: when unsibscribing the RTSPlayerScript is also remove them from the List<Unit>
        health.ServerOnDie -= ServerHandleDie;
    }

    [Server]
    private void ServerHandleDie()
    {
        NetworkServer.Destroy(gameObject);
    }

    #endregion


    #region Client

    public override void OnStartAuthority()
    {
       // if( !hasAuthority) { return; }

        AuthorityOnUnitSpawned?.Invoke(this);
    }

    // we're not using a OnStopAuthority as it runs when Authoroty is removed. and  that is not good.
    // we need it when the client object stops(destroyed)
    public override void OnStopClient()
    {
        // so here we just work 1st on OnStopClient then hasAuthority is true
        if (!hasAuthority) { return; }
        AuthorityOnUnitDespawned?.Invoke(this);// then we despawn it.
        // and we run the implementation on this in the subscriber(s)
    }

    [Client]
    public void Select()
    {
        if (!hasAuthority) { return; }
        onSelected?.Invoke();// ? means if this is NULL do nothing, else Invoke();               
    }

    [Client]
    public void Deselect()
    {
        if (!hasAuthority) { return; }
        onDeselected?.Invoke();        
    }

    #endregion

    //[SerializeField] private SpriteRenderer spriteRenderer = null;


    //private void Start()
    //{
    //    spriteRenderer = GetComponent<SpriteRenderer>();
    //    spriteRenderer.gameObject.SetActive(false);


    //}

    //private void OnEnable()
    //{
    //    spriteRenderer.gameObject.SetActive(true);
    //}


}
