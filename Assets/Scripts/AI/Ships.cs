using System.Collections.Generic;
using UnityEngine;

public static class Ships
{

    //Ship list
    [SerializeField]
    private static List<ShipData> ships = new List<ShipData>();

    //Possible ship types
    public enum ShipType
    {
        None,
        Exploration,
        Construction,
        Transporter,
        Fighter,
        Bomber,
        Frigate,
        Destroyer,
        Capital,
    }

    // Start is called before the first frame update
    static void Start()
    {
        
    }

    // Update is called once per frame
    static void Update()
    {
        
    }

    //Creates a new ship and adds it to the array
    public static void CreateNewShip(GalaxyNode spawnSystem, ShipType type)
    {
        int owningID = spawnSystem.GetOwningFactionID();
        ShipData ship = new ShipData
        {
            shipID = ships.Count,
            owningFactionID = owningID,
            shipType = type,
            shipHealth = 100,
            shipShield = 100,
            shipDamage = 10,
            shipSpeed = 100,
        };
        ships.Add(ship);
    }

    //Function to change the owner of a ship, could be used to capture ships later on.
    public static void SetShipOwner(int shipID, int newOwningID)
    {
        ShipData[] data = GetShipData();
        data[shipID].owningFactionID = newOwningID;
        ships[shipID] = data[shipID];
    }

    //Returns ship data in form of an array
    public static ShipData[] GetShipData()
    {
        return ships.ToArray();
    }
}

[System.Serializable]
public struct ShipData
{
    public int shipID;
    public int owningFactionID;
    public Ships.ShipType shipType;
    public int shipHealth;
    public int shipShield;
    public int shipDamage;
    public int shipSpeed;
}