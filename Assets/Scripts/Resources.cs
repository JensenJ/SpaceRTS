using UnityEngine;

public class Resources
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //Different types of resources
    public enum ResourceType
    {
        None,
        Energy,
        Fuel,
        Minerals
    }
}

//Resource data for spawning
[System.Serializable]
public struct GalaxyGenerationResourceData
{
    public string name;
    [Range(0.1f, 4.0f)]
    public float resourceRichnessMultiplier;
    [Range(0, 100)]
    public float resourcePercentage;
    public int currentNodeCount;
    public Resources.ResourceType nodeResourceType;
    public Color nodeColour;
}

[System.Serializable]
public struct FactionResourceData
{
    public Resources.ResourceType resourceType;
    public int amountStored;
}
