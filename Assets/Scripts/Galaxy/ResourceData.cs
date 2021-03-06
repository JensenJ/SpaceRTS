﻿using UnityEngine;

public class ResourceData
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
    [System.Serializable]
    public enum ResourceType
    {
        None,
        Energy,
        Fuel,
        Minerals
    }
}

//Resource data for generation of resources
[System.Serializable]
public struct GalaxyGenerationResourceData
{
    public string name;
    [Range(0.1f, 4.0f)]
    public float resourceRichnessMultiplier;
    [Range(1000, 9000)]
    public int minResourceRichness;
    [Range(4000, 12000)]
    public int maxResourceRichness;
    [Range(10, 25)]
    public int minProductionRate;
    [Range(25, 50)]
    public int maxProductionRate;
    [Range(0, 100)]
    public float resourcePercentage;
    public int currentNodeCount;
    public ResourceData.ResourceType nodeResourceType;
}

//Data for the node in the galaxy map.
[System.Serializable]
public struct GalaxyNodeResourceData
{
    public int resourceID;
    public ResourceData.ResourceType resourceType;
    public int totalResource;
    public int productionRate;
    public bool isEnabled;
}

//Data for the faction covering all resources.
[System.Serializable]
public struct FactionResourceData
{
    public ResourceData.ResourceType resourceType;
    public int resourceStored;
    public int resourceInflux;
    public int resourceDrain;
}