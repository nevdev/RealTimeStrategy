using Mirror;
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

    #endregion

}
