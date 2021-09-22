using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Mirror;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using System;

public class BuildingButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{    
    [SerializeField] private Building building = null;
    [SerializeField] private Image iconImage = null;
    [SerializeField] private TMP_Text priceText = null;
    [SerializeField] private LayerMask floorMask = new LayerMask();

    private Camera mainCamera; 
    private RTSPlayer player;
    private GameObject buildingPreviewInstance;
    private Renderer buildingRendererInstance;

    private void Start()
    {
        mainCamera = Camera.main;

        iconImage.sprite = building.GetIcon(); // display the values from getters
        priceText.text = building.GetPrice().ToString();
        Invoke("GetPlayer", 1.0f);
    }

    private void GetPlayer()
    {
        player = NetworkClient.connection.identity.GetComponent<RTSPlayer>();
    }

    private void Update()
    {
        if (player == null)
        {
            //player = NetworkClient.connection.identity.GetComponent<RTSPlayer>();
        }   

        if(buildingPreviewInstance == null) { return; }

        // Method called while we are dragging
         UpdateBuildingPreview();
    }
    // OnPointerDown & OnPointerUp is when we click
    public void OnPointerDown(PointerEventData eventData)
    {
        // if it is not the left button - return
        if (eventData.button != PointerEventData.InputButton.Left) { return; }

        buildingPreviewInstance = Instantiate(building.GetBuildingPreview());
        // The renderer is a component in a child of this preview..
        // .. that the previes itself does not have a renderer
        buildingRendererInstance = buildingPreviewInstance.GetComponentInChildren<Renderer>();

        // keep it invisible as soon it is spawned & until starts being soemwhat valid
        // because dragging it active is more expensive
        buildingPreviewInstance.SetActive(false);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (buildingPreviewInstance == null) { return; }

        // Storing or referencing a ray...
        // Move the preview where the mouse is by continously get the 
        // ..mouse poistion (camera sight aiming on mouse position)  during dragging 
        Ray ray = mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());
        // until the raycasting of the mouse position is on the floor...
        // Do the raycast - hit is the data coming out from RaycastHit becasue we asked for the out information
        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, floorMask))
        {
            // place building
            player.CmdTryPlaceBuilding(building.GetId(), hit.point);
        }

        // destroy the preview after release of the mouse button
        Destroy(buildingPreviewInstance);
    }

    // Move the preview while we are dragging OR where the mouse pointer is
    private void UpdateBuildingPreview()
    {
        Ray ray = mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());
        // below, if the ray does not hit something then just return
        if (!Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, floorMask)) { return; }
        //.. but if it hits the floor continue here
        // set the transform.poisition of the buildingPreviewInstance wherever we hit
        buildingPreviewInstance.transform.position = hit.point;

        // if it is somwhere valid, make it active
        if (!buildingPreviewInstance.activeSelf)
        {
            buildingPreviewInstance.SetActive(true);
        }
    }

    
}
