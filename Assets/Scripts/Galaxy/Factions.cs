using System.Collections.Generic;
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
                //Basic setup
                factions[i].homeSystem = homeSystem;
                homeSystem.AddSystemFeature(GalaxyNode.SystemFeatures.Planet);
                homeSystem.SetOwningFaction(i);
                factions[i].homeSystemID = homeSystem.nodeID;
                homeSystems[i] = homeSystem.gameObject;
                factions[i].hasCapitulated = false;

                //Assigning Galaxy node arrays
                factions[i].exploredSystems = new List<GalaxyNode>();
                factions[i].ownedSystems = new List<GalaxyNode>();
                factions[i].exploredSystemIDs = new List<int>();
                factions[i].ownedSystemIDs = new List<int>();

                //Add homesystem to explored and owned systems
                factions[i].exploredSystems.Add(homeSystem);
                factions[i].ownedSystems.Add(homeSystem);
                factions[i].exploredSystemIDs.Add(homeSystem.nodeID);
                factions[i].ownedSystemIDs.Add(homeSystem.nodeID);

                //Resource assigning
                factions[i].resourceData = ResetFactionResourceData(factions[i].factionID);

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

    //Function to reset faction resources to default
    public static FactionResourceData[] ResetFactionResourceData(int factionID)
    {
        //Resource assigning
        factions[factionID].resourceData = new FactionResourceData[3];

        //Energy allocation
        factions[factionID].resourceData[0].resourceType = ResourceData.ResourceType.Energy;
        factions[factionID].resourceData[0].resourceStored = 1000;
        factions[factionID].resourceData[0].resourceInflux = 10;
        //Fuel allocation
        factions[factionID].resourceData[1].resourceType = ResourceData.ResourceType.Fuel;
        factions[factionID].resourceData[1].resourceStored = 1000;
        factions[factionID].resourceData[1].resourceInflux = 10;
        //Minerals allocation
        factions[factionID].resourceData[2].resourceType = ResourceData.ResourceType.Minerals;
        factions[factionID].resourceData[2].resourceStored = 1000;
        factions[factionID].resourceData[2].resourceInflux = 10;

        return factions[factionID].resourceData;
    }

    //Function to load faction data
    public static FactionData[] LoadFactions(FactionData[] factionData)
    {
        //Clears current faction data
        ClearFactions(factionData.Length);
        for (int i = 0; i < factionData.Length; i++)
        {
            //Faction variable assignment
            factions[i] = factionData[i];
            factions[i].homeSystem = GalaxyGenerator.GetGalaxyNodeFromID(factionData[i].homeSystemID, GalaxyGenerator.GetAllGalaxySystems());
            factions[i].exploredSystems = new List<GalaxyNode>();
            factions[i].ownedSystems = new List<GalaxyNode>();
            factions[i].ownedSystemIDs = new List<int>();
            factions[i].exploredSystemIDs = new List<int>();
            factions[i].resourceData = ResetFactionResourceData(factions[i].factionID);
            

            //Get all explored systems
            for (int j = 0; j < factionData[i].exploredSystemIDs.Count; j++)
            {
                GalaxyNode newNode = GalaxyGenerator.GetGalaxyNodeFromID(factionData[i].exploredSystemIDs[j], GalaxyGenerator.GetAllGalaxySystems());
                AddExploredSystem(factions[i].factionID, newNode);
            }
            //Get all owned systems
            for (int j = 0; j < factionData[i].ownedSystemIDs.Count; j++)
            {
                GalaxyNode newNode = GalaxyGenerator.GetGalaxyNodeFromID(factionData[i].ownedSystemIDs[j], GalaxyGenerator.GetAllGalaxySystems());
                AddControlledSystem(factions[i].factionID, newNode);
            }
        }
        SaveData.current.factions = factions;
        return factions;
    }

    public static void ClearFactions(int newLength)
    {
        factions = new FactionData[newLength];
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
            //Check the system is not already in the list
            bool alreadyInList = false;
            bool alreadyInIDList = false;
            FactionData data = factions[factionID];
            for (int i = 0; i < data.exploredSystems.Count; i++)
            {
                if (data.exploredSystems.Contains(newExploredSystem))
                {
                    alreadyInList = true;
                }
                if (data.exploredSystemIDs.Contains(newExploredSystem.nodeID))
                {
                    alreadyInIDList = true;
                }
            }

            //If allowed to proceed
            if (alreadyInList == false)
            {
                factions[factionID].exploredSystems.Add(newExploredSystem);
            }
            if(alreadyInIDList == false)
            {
                factions[factionID].exploredSystemIDs.Add(newExploredSystem.nodeID);
            } 
        }
    }

    //Adds an owned system to the list
    public static void AddControlledSystem(int factionID, GalaxyNode newControlledSystem)
    {
        if (factionID < factions.Length && factionID >= 0)
        {
            //Check the system is not already in the list
            bool alreadyInList = false;
            bool alreadyInIDList = false;
            FactionData data = factions[factionID];
            for (int i = 0; i < data.ownedSystems.Count; i++)
            {
                if (data.ownedSystems.Contains(newControlledSystem))
                {
                    alreadyInList = true;
                }
                if (data.ownedSystemIDs.Contains(newControlledSystem.nodeID))
                {
                    alreadyInIDList = true;
                }
            }

            //If its new to the list
            if (alreadyInList == false)
            {
                //Add to explored systems, incase it wasnt added before
                AddExploredSystem(factionID, newControlledSystem);
                //Adding to controlled lists
                factions[factionID].ownedSystems.Add(newControlledSystem);
                newControlledSystem.SetOwningFaction(factionID);
                UpdateResourceInflux(factionID, newControlledSystem);
            }
            if (alreadyInIDList == false)
            {
                factions[factionID].ownedSystemIDs.Add(newControlledSystem.nodeID);
            }
        }
    }

    //Function to check for capitulation
    public static void CheckForCapitulation()
    {
        //Check for capitulation of other nation
        for (int i = 0; i < factions.Length; i++)
        {
            FactionData data = GetFactionData(i);
            int homeSystemOwner = data.homeSystem.GetOwningFactionID();
            if (data.factionID != homeSystemOwner)
            {
                SetCapitulatedStatus(data.factionID, true);
            }
        }
    }

    //Sets the capitulation status on a faction.
    public static void SetCapitulatedStatus(int factionID, bool status)
    {
        if(factionID < factions.Length && factionID >= 0)
        {
            factions[factionID].hasCapitulated = status;
            Debug.Log(factions[factionID].factionName + " capitulation status: " + status);
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

    public static int GetNumberOfFactions()
    {
        return numberOfFactions;
    }
}

//Faction data struct
[System.Serializable]
public struct FactionData
{
    public int factionID;
    public string factionName;
    public bool hasCapitulated;
    public Color factionColour;
    public int homeSystemID;
    public GalaxyNode homeSystem;
    public List<int> exploredSystemIDs;
    public List<int> ownedSystemIDs;
    public List<GalaxyNode> exploredSystems;
    public List<GalaxyNode> ownedSystems;
    public FactionResourceData[] resourceData;
}