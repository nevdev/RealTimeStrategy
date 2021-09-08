using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Targeter : NetworkBehaviour
{

    [SerializeField] private Targetable target;

    public Targetable GetTarget()
    {
        return target;
    }

    #region Server 

    public override void OnStopServer()
    {
        GameOverHandler.ServerOnGameOver += ServerHandleGameOver;
    }

    public override void OnStartServer()
    {
        GameOverHandler.ServerOnGameOver -= ServerHandleGameOver;
    }

    // the client will tell the server 'This is what I want the target to be 
    [Command] 
    public void CmdSetTarget(GameObject targetGameObject)
    {
        // validate that this object has the targetable component on it
        if (!targetGameObject.TryGetComponent <Targetable>(out Targetable newTarget)) { return; }
        this.target = newTarget;
    }

    [Server] // so we do not call it on the client
    public void ClearTarget()
    {
        target = null;
    }

    // we need to use this handler to know when the game is over and we can clear the target and with this
    // .. the UNitMovement script will not have a target to follow and will stop moving when the game is over.
    // but we are also reseting the navmesh agent stop any object that already been clicked to follow. Go UnitMovement script, ServerHandleGameOver->
    private void ServerHandleGameOver()
    {
        ClearTarget();
    }

    #endregion

}
