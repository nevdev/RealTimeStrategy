using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

// inheriting Monobehaviour means this class is completely client side
public class UnitSelectionHandler : MonoBehaviour
{
    [SerializeField] private RectTransform unitSelectionArea = null; // to manipulate the dimensions of the DragBox when selecting objcts on screen
    [SerializeField] private LayerMask layerMask = new LayerMask();

    // DragBox dimension:  we just nedd the x & y position to select on the GameScene the objects
    [SerializeField] private Vector2 startPosition; // we need to store coords when we start clicking in order to start showing the DragBox at that point



    private RTSPlayer player;
    private Camera mainCamera;
    
    public List<Unit> SelectedUnits { get;} = new List<Unit>();


    private void Start()
    {
        mainCamera = Camera.main;
        Unit.AuthorityOnUnitDespawned += AuthorityHandleUnitDespawned;
        Invoke("GetPlayer", 0.1f); // modified, not like Nathan's otherwise will crash somehow

        // we need to listen 
        GameOverHandler.ClientOnGameOver += ClientHandleGameOver;
    }

    private void OnDestroy()
    {
        Unit.AuthorityOnUnitDespawned -= AuthorityHandleUnitDespawned;
        GameOverHandler.ClientOnGameOver -= ClientHandleGameOver;
    }

    private void GetPlayer()
    {
        player = NetworkClient.connection.identity.GetComponent<RTSPlayer>();
    }
    private void Update()
    {
        if(player == null)
        {
         //   Invoke("GetPlayer", 0.2f);
           // player = NetworkClient.connection.identity.GetComponent<RTSPlayer>();
        }

        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            // start the selection area
            StartSelectionArea();            
        }
        else if (Mouse.current.leftButton.wasReleasedThisFrame)
        {
            // call this method after releasing the left mouse button
            ClearSelectionArea();
        } 
        else if (Mouse.current.leftButton.isPressed) // Update the DragBox size every frame when we have the mouse button down
        {
            UpdateSelectionArea();
        }
    }

    private void StartSelectionArea()
    {
        if (!Keyboard.current.leftShiftKey.isPressed)
        {
            foreach (Unit selectedUnit in SelectedUnits)
            {
                selectedUnit.Deselect();
            }
            SelectedUnits.Clear();
        }

        unitSelectionArea.gameObject.SetActive(true);
        startPosition = Mouse.current.position.ReadValue();
        UpdateSelectionArea();
    }

    private void UpdateSelectionArea()
    {
        Vector2 mousePosition = Mouse.current.position.ReadValue();
        float areaWidth = mousePosition.x - startPosition.x; // get the X how much it was dragged or increased the dragbox from start of click to current position
        float areaHeight = mousePosition.y - startPosition.y; // same as above but for y
        // in total X and Y gets results the area of X and Y
        //   O---------------O
        //   |               |
        //   |               |
        //   |       X       |   DragBox - X = AncorPoint, O = Coordinate areaWidth * areaHeight
        //   |               |
        //   |               |
        //   O---------------O

        // now from where we started dragging till ended (released the mouse button) get the coordinate and 
        // change the size of the DragBox
        //REMEMBER THAT IF WE DRAGGED IN DOWNWARDS OR RIGHT-TO-LEFT THE VALUE WILL BE IN NEGATIVE SO WE NEED TO CHANGE TO POSITIVE
        unitSelectionArea.sizeDelta = new Vector2(Mathf.Abs(areaWidth), Mathf.Abs(areaHeight));


        // SiezeDelta
        // The size of this RectTransform relative to the distances between the anchors.
        // If the anchors are together, sizeDelta is the same as size.If the anchors are in each of 
        // the four corners of the parent,
        //   the sizeDelta is how much bigger or smaller the rectangle is compared to its parent.

        unitSelectionArea.anchoredPosition = startPosition + 
                                            new Vector2(areaWidth / 2, areaHeight / 2);


       // ClearSelectionArea();
    }

    private void ClearSelectionArea()
    {
        unitSelectionArea.gameObject.SetActive(false);

        if(unitSelectionArea.sizeDelta.magnitude == 0)
        {

            Ray ray = mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());// take its postion

            // returns true if intersects with a collider
            //params:
            // ray -> origin
            // hit -> direction
            // Mathf.Infonity: maxDistance
            // layerMask: layer the objet is masked
            if (!Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, layerMask)) { return; }
            // if hit collides a Unit(Tank) 
            if (!hit.collider.TryGetComponent<Unit>(out Unit unit)) { return; }

            if (!unit.hasAuthority) { return; }// unit is a server class .... NetworkBehaviour

            SelectedUnits.Add(unit);

            foreach (Unit selectedUnit in SelectedUnits)
            {
                //Select unit/s currently in the list
                selectedUnit.Select();
            }
            return;
        }

        Vector2 min = unitSelectionArea.anchoredPosition - (unitSelectionArea.sizeDelta / 2);
        Vector2 max = unitSelectionArea.anchoredPosition + (unitSelectionArea.sizeDelta / 2);

        foreach (Unit unit in player.GetMyUnits())
        {
            if (SelectedUnits.Contains(unit)){ continue; }

            // get the unit position which is in teh World Position into a Vector3 variable
            Vector3 screenPosition = mainCamera.WorldToScreenPoint(unit.transform.position);

            // if the unit is within the DragBox
            if(screenPosition.x > min.x 
            && screenPosition.x < max.x
            && screenPosition.y > min.y
            && screenPosition.y < max.y)
            {
                //... then select the Units the unit
                SelectedUnits.Add(unit);
                unit.Select();
            }
        }
    }

    // this subscribed method will remove the units from the Listwhich have died 
    private void AuthorityHandleUnitDespawned(Unit unit)
    {
        SelectedUnits.Remove(unit);
    }

    private void ClientHandleGameOver(string winnerName)
    {
        // we fdnont need the string but we need to use the handler
        enabled = false; // this will stop the default Update of this object  .. 
        // ..hence Stopping to run the statments inside the Updade 

    }

}
