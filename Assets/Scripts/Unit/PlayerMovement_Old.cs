﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.AI;

public class PlayerMoevement : NetworkBehaviour
{

    [SerializeField]
    private NavMeshAgent agent = null;
    private Camera mainCamera;

    #region Server
    [Command]
     private void CmdMove(Vector3 position)
    {
        if (!NavMesh.SamplePosition(position, out NavMeshHit hit,  1f, NavMesh.AllAreas)) { return; }
        agent.SetDestination(hit.position);
    }


    #endregion

    #region Client
    public override void OnStartAuthority()
    {
        mainCamera = Camera.main;
        base.OnStartAuthority();
    }

    [ClientCallback]         // Prevents the server form running this mathod.
    private void Update() // now this is  Client Only Update method()!
    {
        if (!hasAuthority) { return; }
        if (!Input.GetMouseButtonDown(1)) { return; }
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        if(!Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity)) { return; }

        CmdMove(hit.point);
    }

    #endregion
}
