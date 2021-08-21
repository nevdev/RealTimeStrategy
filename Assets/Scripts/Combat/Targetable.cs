using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Targetable : MonoBehaviour
{
    [SerializeField] private Transform aimAtPoint = null;

    private Transform GetAimAtPoint()
    {
        return aimAtPoint;
    }
}
