using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class UnitCommandGiver : MonoBehaviour
{    
    [SerializeField] private UnitSelectionHandler unitSelectionHandler = null;// can be declared as null(or without) since it is a class
    [SerializeField] private LayerMask layerMask = new LayerMask(); // needs init with new as these are structs, not like classes 

    private Camera mainCamera;    

    private void Start()
    {
        mainCamera = Camera.main;

        // the implementaion of this handler will stop the update of this object...
        GameOverHandler.ClientOnGameOver += ClientHandleGameOver;
        // ..and by doing this will stop the ray cast and clicks on the object, because 
        // .. the game's over! 
    }

    private void OnDestroy()
    {
        GameOverHandler.ClientOnGameOver -= ClientHandleGameOver;
    }
    private void Update()
    {
        
        if (!Mouse.current.rightButton.wasPressedThisFrame) { return; }
        
        Ray ray = mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());
        
        if (!Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, layerMask)) { return; }
    
        if(hit.collider.TryGetComponent<Targetable>(out Targetable newTarget))
        {
            if (newTarget.hasAuthority)// if we have authority on that target ...
            {
                TryMove(hit.point);// ... than we just move there but then return and not attach because if we have authority it is out unit not the enemy
                return;
            }
            TryTarget(newTarget);// here we do not have authority then target it
            return;
        }

        TryMove(hit.point);

    }

    private void TryMove(Vector3 point)
    {

        foreach (Unit unit in unitSelectionHandler.SelectedUnits)
        {
            unit.GetUnitMovement().CmdMove(point);
        }
    }

    private void TryTarget(Targetable target)
    {
        foreach(Unit unit in unitSelectionHandler.SelectedUnits)
        {
            unit.GetTargeter().CmdSetTarget(target.gameObject);
        }
    }

    private void ClientHandleGameOver(string winnerName)
    {
        enabled = false;
    }
}
