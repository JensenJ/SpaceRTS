using System.Collections.Generic;
using UnityEngine;

public class Factions : MonoBehaviour
{
    static int numberOfFactions;
    static FactionData[] factions;

    //Create faction data array randomly.
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
            factions[i].resourceData[0].resourceStored = 1000;
            factions[i].resourceData[0].resourceInflux = 10;

            //Fuel allocation
            factions[i].resourceData[1].resourceType = Resources.ResourceType.Fuel;
            factions[i].resourceData[1].resourceStored = 1000;
            factions[i].resourceData[0].resourceInflux = 10;
            //Minerals allocation
            factions[i].resourceData[2].resourceType = Resources.ResourceType.Minerals;
            factions[i].resourceData[2].resourceStored = 1000;
            factions[i].resourceData[2].resourceInflux = 10;
        }
        return factions;
    }

    //Sets the faction name
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