using System.Collections.Generic;
using UnityEngine;
using TMPro;

[System.Serializable]
public class GalaxyNode : MonoBehaviour
{
    [SerializeField]
    public int currentRing;
    [SerializeField]
    public Vector3 position;
    [SerializeField]
    public string nodeName;

    [SerializeField]
    int owningFactionID = -1;

    GameObject resourceContentPanel = null;
    GameObject infoPanel = null;

    //Possible features that a system can have
    [System.Serializable]
    public enum SystemFeatures {
        None,
        Resource,
        Planet,
        Station
    }


    //Allows multiple galaxy features in a single system
    [SerializeField]
    private List<SystemFeatures> features = new List<SystemFeatures>();

    //Resource list
    [SerializeField]
    private List<GalaxyNodeResourceData> resources = new List<GalaxyNodeResourceData>();

    [SerializeField]
    private List<GalaxyNode> connectingNodes = new List<GalaxyNode>();

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void CreateNodeUI(GameObject nodePrefab, GameObject resourcePrefab)
    {
        //Spawn info panel
        Vector2 position = new Vector2(transform.position.x, transform.position.y + resources.Count);
        infoPanel = Instantiate(nodePrefab, position, Quaternion.identity, transform);
        resourceContentPanel = infoPanel.transform.GetChild(0).GetChild(0).GetChild(1).GetChild(0).gameObject;
        //Create new node UI
        for (int i = 0; i < resources.Count; i++)
        {
            Instantiate(resourcePrefab, transform.position, Quaternion.identity, resourceContentPanel.transform);
        }
        //Disable resources panel if there are no resources on the node
        if(resources.Count <= 0)
        {
            resourceContentPanel.SetActive(false);
        }
        DisableInfoPanel();
        UpdateNodeUI();
    }

    public void UpdateGalaxyNodeData()
    {
        position = transform.position;
        nodeName = transform.gameObject.name;
    }

    //Info panel functions
    public void EnableInfoPanel()
    {
        infoPanel.SetActive(true);
    }
    public void DisableInfoPanel()
    {
        infoPanel.SetActive(false);
    }

    public bool IsInfoPanelActive()
    {
        return infoPanel.activeSelf;
    }

    //Updates the node UI if any changes are present, e.g. some resources have been drained
    public void UpdateNodeUI()
    {
        transform.GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>().text = gameObject.name;
        for (int i = 0; i < resourceContentPanel.transform.childCount; i++)
        {
            //Resource panel
            resourceContentPanel.transform.GetChild(i).GetChild(0).GetComponent<TextMeshProUGUI>().text = resources[i].resourceType.ToString();
            resourceContentPanel.transform.GetChild(i).GetChild(1).GetComponent<TextMeshProUGUI>().text = resources[i].totalResource.ToString() + " (" + resources[i].productionRate + ")";
        }
    }

    //Adding features and resources
    public void AddSystemFeature(SystemFeatures m_feature)
    {
        features.Add(m_feature);
    }

    public void AddConnectingNode(GalaxyNode nodeToConnect)
    {
        if (!connectingNodes.Contains(nodeToConnect))
        {
            connectingNodes.Add(nodeToConnect);
        }
    }

    //Add a resource to a node.
    public void AddResource(ResourceData.ResourceType m_resource, int resourceAmount, int productionRate)
    {
        AddSystemFeature(SystemFeatures.Resource);
        GalaxyNodeResourceData resource = new GalaxyNodeResourceData
        {
            resourceID = resources.Count,
            resourceType = m_resource,
            totalResource = resourceAmount,
            productionRate = productionRate
        };

        resources.Add(resource);
    }

    //Turns the resource on, e.g. allows production
    public void EnableResource(int resourceID)
    {
        GalaxyNodeResourceData[] data = GetResourcesData();
        data[resourceID].isEnabled = true;
        resources[resourceID] = data[resourceID];
    }

    //Turns the resource off, e.g. disallows production
    public void DisableResource(int resourceID)
    {
        GalaxyNodeResourceData[] data = GetResourcesData();
        data[resourceID].isEnabled = false;
        resources[resourceID] = data[resourceID];
    }

    //Getters and Setters
    public void SetOwningFaction(int factionID)
    {
        owningFactionID = factionID;
        SpriteRenderer renderer = GetComponent<SpriteRenderer>();
        FactionData data = Factions.GetFactionData(factionID);
        renderer.color = data.factionColour;
    }

    //Returns the resource data of this node
    public GalaxyNodeResourceData[] GetResourcesData()
    {
        return resources.ToArray();
    }
    //Returns all system features
    public SystemFeatures[] GetSystemFeatures()
    {
        return features.ToArray();
    }

    //Returns the owning faction id
    public int GetOwningFactionID()
    {
        return owningFactionID;
    }
}