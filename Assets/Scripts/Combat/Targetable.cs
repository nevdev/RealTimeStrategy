using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Targetable : NetworkBehaviour
{
    // by having this script on our tank we can have where we should aim. Instead aiming on the bottom of the object where the original transform is
    // the aimAtpoint will be somewhere in teh centre.

    [SerializeField] private Transform aimAtPoint = null;

    // a getter were we can call it when we need this aimAtPoint
    public Transform GetAimAtPoint()
    {
        return aimAtPoint;
    }
}
