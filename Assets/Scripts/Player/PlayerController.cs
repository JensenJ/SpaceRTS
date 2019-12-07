using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    int playerFactionID = 0;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
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
            if(hit.collider != null)
            {
                Debug.Log(hit.transform.name);
                //Get node data
                GalaxyNode node = hit.transform.gameObject.GetComponent<GalaxyNode>();
                if(node != null)
                {
                    //Do something to node, e.g. get resource data.
                    
                }
            }
        }
    }
}
