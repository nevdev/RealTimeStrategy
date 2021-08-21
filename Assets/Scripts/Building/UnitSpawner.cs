using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class UnitSpawner : NetworkBehaviour, IPointerClickHandler // IPointerClickHandler is build for UI
{
    [SerializeField] private GameObject unitPrefab = null;
    [SerializeField] private Transform unitSpawnPoint = null;

    #region Server

    [Command]
    private void CmdSpawnUnit()
    {
        GameObject unitInstance = Instantiate(  // the unityInstance will be instatiated on the server
                                    unitPrefab,   
                                    unitSpawnPoint.position,
                                    unitSpawnPoint.rotation );
        //----------------
        // from the server side, will give authority to the coonection to client ONLY WHEN THE 'connectionToClient' PARAM IS ADDED, 
        //OTHEWISE AUTHORITY WILL BE SENT TO ALL

        // give that client / person the ownsership of the new unit/object
      NetworkServer.Spawn(unitInstance, connectionToClient); // passing the connection to client ..spawn object which client invoked with the current connection creating the object
                // so the unity/object that spawns will also belongs to me or which client created/invoked the spawn
        // -----------------
    }

    #endregion



    #region Client

    public void OnPointerClick(PointerEventData eventData)
    {
        // if the eventData.button is not the mouse left input button then return out from here
        if (eventData.button != PointerEventData.InputButton.Left) { return; }
        
        if (!hasAuthority) { return; }

        CmdSpawnUnit();
    }

    #endregion
}
