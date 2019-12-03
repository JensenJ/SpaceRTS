using System.Collections.Generic;
using System.Collections;
using UnityEngine;

public class Factions : MonoBehaviour
{
    static int numberOfFactions;
    static FactionData[] factions;

    public static FactionData[] CreateFactions(int numberOfFactionsToCreate, GameObject[] systems)
    {
        numberOfFactions = numberOfFactionsToCreate;
        factions = new FactionData[numberOfFactions];
        for (int i = 0; i < numberOfFactionsToCreate; i++)
        {
            //Faction identifiers
            factions[i].factionID = i;
            factions[i].factionName = "Faction" + Random.Range(0, 100);
            factions[i].factionColour = new Color(Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f));

            //Home / Starting system
            GalaxyNode homeSystem = systems[Random.Range(0, systems.Length)].GetComponent<GalaxyNode>();
            factions[i].homeSystem = homeSystem;

            //Assigning Galaxy node arrays
            factions[i].exploredSystems = new List<GalaxyNode>();
            factions[i].ownedSystems = new List<GalaxyNode>();

            factions[i].exploredSystems.Add(homeSystem);
            factions[i].ownedSystems.Add(homeSystem);

            //Resource assigning
            factions[i].resourceData = new FactionResourceData[3];


            //Energy allocation
            factions[i].resourceData[0].resourceType = Resources.ResourceType.Energy;
            factions[i].resourceData[0].amountStored = 1000;

            //Fuel allocation
            factions[i].resourceData[1].resourceType = Resources.ResourceType.Fuel;
            factions[i].resourceData[1].amountStored = 1000;
            //Minerals allocation
            factions[i].resourceData[2].resourceType = Resources.ResourceType.Minerals;
            factions[i].resourceData[2].amountStored = 1000;
        }
        return factions;
    }

    public static bool SetFactionName(int factionID, string newName)
    {
        if(factionID > factions.Length || factionID < 0)
        {
            return false;
        }
        factions[factionID].factionName = newName;
        return true;
    }
}

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