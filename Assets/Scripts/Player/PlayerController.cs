﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField]
    int playerFactionID = 0;
    [SerializeField]
    GalaxyNode currentlySelectedNode = null;
    [SerializeField]
    GalaxyNode previouslySelectedNode = null;
    [SerializeField]
    bool canInteract = false;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (canInteract)
        {
            //If left mouse button clicked
            if (Input.GetMouseButtonDown(0))
            {
                //Get mouse position
                Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                Vector2 mousePos2D = new Vector2(mousePos.x, mousePos.y);

                //Raycast and return information
                RaycastHit2D hit = Physics2D.Raycast(mousePos2D, Vector2.zero);
                //If something was hit
                if (hit.collider != null)
                {
                    //Disable info panel on last selected node
                    if (previouslySelectedNode != null)
                    {
                        previouslySelectedNode.DisableInfoPanel();
                    }
                    //Get node data
                    currentlySelectedNode = hit.transform.gameObject.GetComponent<GalaxyNode>();
                    if (currentlySelectedNode != null)
                    {
                        previouslySelectedNode = currentlySelectedNode;
                        //Do something to node, e.g. get resource data.
                        GalaxyNodeResourceData[] data = currentlySelectedNode.GetResourcesData();
                        for (int i = 0; i < data.Length; i++)
                        {
                            //Debug.Log(data[i].resourceType + " amount: " + data[i].totalResource + " at a rate of: " + data[i].productionRate);
                            currentlySelectedNode.EnableResource(i);
                        }
                        Factions.RemoveControlledSystem(currentlySelectedNode.GetOwningFactionID(), currentlySelectedNode);
                        Factions.AddControlledSystem(playerFactionID, currentlySelectedNode);
                        Factions.CheckForCapitulation();
                        currentlySelectedNode.AddShip(Ships.CreateNewShip(currentlySelectedNode, Ships.ShipType.Exploration));
                        currentlySelectedNode.EnableInfoPanel();
                    }
                }
                else
                {
                    if (previouslySelectedNode != null)
                    {
                        previouslySelectedNode.DisableInfoPanel();
                    }
                }
            }
        }
    }

    public void SetInteractStatus(bool status)
    {
        canInteract = status;
    }

    public bool GetInteractStatus()
    {
        return canInteract;
    }
}
