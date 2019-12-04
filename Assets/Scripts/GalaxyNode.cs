using System.Collections.Generic;
using UnityEngine;

public class GalaxyNode : MonoBehaviour
{

    [SerializeField]
    int owningFactionID = -1;

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
            resourceType = m_resource,
            totalResource = resourceAmount,
            productionRate = productionRate
        };

        resources.Add(resource);
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
