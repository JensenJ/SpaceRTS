using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GalaxyNode : MonoBehaviour
{

    [SerializeField]
    int owningFactionID = -1;

    [SerializeField]
    GameObject resourceContentPanel = null;

    GameObject infoPanel = null;

    //Possible features that a system can have
    public enum SystemFeatures {
        None,
        Resource,
        Planet,
        Station
    }


    //Allows multiple galaxy features in a single system
    [SerializeField]
    private List<SystemFeatures> features = new List<SystemFeatures>();

    [SerializeField]
    private List<GalaxyNodeResourceData> resources = new List<GalaxyNodeResourceData>();

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
        resourceContentPanel = infoPanel.transform.GetChild(0).GetChild(1).GetChild(0).gameObject;
        for (int i = 0; i < resources.Count; i++)
        {
            Instantiate(resourcePrefab, transform.position, Quaternion.identity, resourceContentPanel.transform);
        }
        DisableInfoPanel();
        UpdateNodeUI();
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

    public void UpdateNodeUI()
    {
        transform.GetChild(0).GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>().text = gameObject.name;
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

    //Add a resource to a node.
    public void AddResource(Resources.ResourceType m_resource, int resourceAmount, int productionRate)
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

    public void EnableResource(int resourceID)
    {
        GalaxyNodeResourceData[] data = GetResourcesData();
        data[resourceID].isEnabled = true;
        resources[resourceID] = data[resourceID];
    }

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

    public GalaxyNodeResourceData[] GetResourcesData()
    {
        return resources.ToArray();
    }
    public SystemFeatures[] GetSystemFeatures()
    {
        return features.ToArray();
    }

    public int GetOwningFactionID()
    {
        return owningFactionID;
    }
}
