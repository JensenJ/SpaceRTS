﻿using System.Collections.Generic;
using UnityEngine;

public static class Factions
{
    static int numberOfFactions;
    static FactionData[] factions;

    //Create faction data array randomly.
    public static FactionData[] CreateFactions(int numberOfFactionsToCreate, GameObject[] systems)
    {
        numberOfFactions = numberOfFactionsToCreate;
        factions = new FactionData[numberOfFactions];
        GameObject[] homeSystems = new GameObject[numberOfFactions];
        //For every faction to create
        for (int i = 0; i < numberOfFactionsToCreate; i++)
        {
            //Faction identifiers
            factions[i].factionID = i;
            factions[i].factionName = "Faction" + Random.Range(0, 100);
            factions[i].factionColour = new Color(Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f));

            //Home / Starting system
            GalaxyNode homeSystem = systems[Random.Range(0, systems.Length)].GetComponent<GalaxyNode>();

            //Validating faction system, prevents positioning multiple factions inside same system.
            bool valid = true;
            for (int j = 0; j < homeSystems.Length; j++)
            {
                //If the system is already in the array
                if(homeSystems[j] == homeSystem.gameObject)
                {
                    //It is not valid
                    valid = false;
                }
            }

            //If positioning is valid
            if (valid == true)
            {
                factions[i].homeSystem = homeSystem;
                homeSystem.AddSystemFeature(GalaxyNode.SystemFeatures.Planet);
                homeSystem.SetOwningFaction(i);
                homeSystems[i] = homeSystem.gameObject;

                //Assigning Galaxy node arrays
                factions[i].exploredSystems = new List<GalaxyNode>();
                factions[i].ownedSystems = new List<GalaxyNode>();

                //Add homesystem to explored and owned systems
                factions[i].exploredSystems.Add(homeSystem);
                factions[i].ownedSystems.Add(homeSystem);

                //Resource assigning
                factions[i].resourceData = new FactionResourceData[3];

                //Energy allocation
                factions[i].resourceData[0].resourceType = Resources.ResourceType.Energy;
                factions[i].resourceData[0].resourceStored = 1000;
                factions[i].resourceData[0].resourceInflux = 10;
                //Fuel allocation
                factions[i].resourceData[1].resourceType = Resources.ResourceType.Fuel;
                factions[i].resourceData[1].resourceStored = 1000;
                factions[i].resourceData[1].resourceInflux = 10;
                //Minerals allocation
                factions[i].resourceData[2].resourceType = Resources.ResourceType.Minerals;
                factions[i].resourceData[2].resourceStored = 1000;
                factions[i].resourceData[2].resourceInflux = 10;
                //Update resources
                UpdateResourceInflux(i, homeSystem);
            }
            else
            {
                i--;
            }
        }
        return factions;
    }

    //Sets the faction name
    public static void SetFactionName(int factionID, string newName)
    {
        if(factionID < factions.Length && factionID >= 0)
        {
            factions[factionID].factionName = newName;
        }
    }
    //Sets the faction colour
    public static void SetFactionColour(int factionID, Color factionColor)
    {
        if (factionID < factions.Length && factionID >= 0)
        {
            factions[factionID].factionColour = factionColor;
        }
    }

    //Adds an explored system to the list
    public static void AddExploredSystem(int factionID, GalaxyNode newExploredSystem)
    {
        if (factionID < factions.Length && factionID >= 0)
        {
            factions[factionID].exploredSystems.Add(newExploredSystem);
        }
    }

    //Adds an owned system to the list
    public static void AddControlledSystem(int factionID, GalaxyNode newControlledSystem)
    {
        if (factionID < factions.Length && factionID >= 0)
        {
            factions[factionID].ownedSystems.Add(newControlledSystem);
            newControlledSystem.SetOwningFaction(factionID);
            UpdateResourceInflux(factionID, newControlledSystem);
        }
    }

    //Update the amount of a resource a faction collects for a period of time.
    private static void UpdateResourceInflux(int factionID, GalaxyNode resourceNode)
    {
        if (factionID < factions.Length && factionID >= 0)
        {
            GalaxyNodeResourceData[] resources = resourceNode.GetResourcesData();
            //For every resource in the node
            for (int i = 0; i < resources.Length; i++)
            {
                //Get faction data
                FactionData data = GetFactionData(factionID);
                //For every resource the faction can collect
                for (int j = 0; j < data.resourceData.Length; j++)
                {
                    if (data.resourceData[j].resourceType == resources[i].resourceType)
                    {
                        //If the producing resource node is enabled (has the faction built a mining rig? etc..)
                        if (resources[i].isEnabled)
                        {
                            //Add to resource influx
                            data.resourceData[j].resourceInflux += resources[i].productionRate;
                        }
                    }
                }
            }
        }
    }

    //Get Faction Data
    public static FactionData GetFactionData(int factionID)
    {
        //If faction ID is valid.
        if (factionID < factions.Length && factionID >= 0)
        {
            return factions[factionID];
        }
        else
        {
            //Return player faction if id is invalid.
            return factions[0];
        }
    }
}

//Faction data struct
[System.Serializable]
public struct FactionData
{
    public int factionID;
    public string factionName;
    public Color factionColour;
    public GalaxyNode homeSystem;
    public List<GalaxyNode> exploredSystems;
    public List<GalaxyNode> ownedSystems;
    public FactionResourceData[] resourceData;
}