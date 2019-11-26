using UnityEngine;
using System.Linq;

public class GalaxyGenerator : MonoBehaviour
{
    //Node Colours
    [Space(20)]
    [SerializeField]
    Color defaultColor = Color.black;

    [Space(10)]
    [SerializeField]
    Color shopRepairColor = Color.black;
    [SerializeField]
    int shopFrequency = 5;

    [Space(10)]
    [SerializeField]
    Color fuelColor = Color.black;
    [SerializeField]
    int fuelFrequency = 20;

    [Space(10)]
    [SerializeField]
    Color energyColor = Color.black;
    [SerializeField]
    int energyFrequency = 30;

    [Header("Galaxy Generation Settings: ")]
    [Space(20)]
    [SerializeField]
    [Range(1.0f, 4.0f)]
    float systemDensity = 0.6f;
    [SerializeField]
    [Range(50.0f, 250.0f)]
    float systemRadius = 100.0f;
    [SerializeField]
    [Range(0.0f, 50.0f)]
    float systemCenterRadius = 25.0f;
    [SerializeField]
    [Range(0, 10)]
    int systemRingCount = 8;
    [SerializeField]
    [Range(0.01f, 10.0f)]
    float systemCollisionDistance = 1.5f;
    [SerializeField]
    bool systemEquidistant = true;

    private float systemRingWidth = 0.1f;

    [Space(20)]
    [SerializeField]
    GameObject systemPrefab = null;
    [SerializeField]
    GameObject ringPrefab = null;

    [SerializeField]
    GameObject[] systems;

    // Start is called before the first frame update
    void Start()
    {
        SpawnGalaxy();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void SpawnGalaxy()
    {
        //For each system ring
        for (int i = 0; i < systemRingCount; i++)
        {
            //Calculate segment size and ring radius
            
            float segmentSize = (systemRadius - systemCenterRadius) / systemRingCount;
            float ringRadius = segmentSize * (i + 1);

            //Calculate ring radius min/max
            float minRingRadius = ringRadius - systemRingWidth;
            float maxRingRadius = ringRadius + systemRingWidth;

            //Calculate ring area
            float upperArea = maxRingRadius * maxRingRadius * Mathf.PI;
            float lowerArea = minRingRadius * minRingRadius * Mathf.PI;
            float ringArea = upperArea - lowerArea;

            //Calculate number of systems for each ring using formula: mass (number of systems) = density * volume (area).
            int numberOfSystemsForRing = Mathf.FloorToInt(systemDensity * ringArea);

            //Instantiate ring object for ring image, give gameobject ring name
            GameObject galaxyRing = Instantiate(ringPrefab, new Vector2(0, 0), Quaternion.identity, transform);
            galaxyRing.name = "GalaxyRing " + (i + 1);

            //For each system in a ring
            //for (int j = 0; j < numberOfSystemsForRing; j++)
            //{
            //    //Generate position
            //    float a = Random.Range(0.0f, 1.0f) * 2 * Mathf.PI;
            //    float r = systemRadius * Mathf.Sqrt(Random.Range(minRingRadius, maxRingRadius));
            //    Vector2 position = new Vector2(r * Mathf.Cos(a) / 10.0f, r * Mathf.Sin(a) / 10.0f);

            //    //GameObject system = Instantiate(systemPrefab, position, Quaternion.identity, transform.GetChild(i + 1));
            //    //Instantiate system object
            //    Instantiate(systemPrefab, position, Quaternion.identity, transform);
            //}

            for (int j = 0; j < numberOfSystemsForRing; j++)
            {
                float angle;
                if (systemEquidistant)
                {
                    //Spawn systems that have an equal distance from each other.
                    angle = j * Mathf.PI * 2f / numberOfSystemsForRing;
                }
                else
                {
                    //Spawn systems randomly
                    angle = Random.Range(0.0f, 1.0f) * Mathf.PI * 2f;
                }
                //Calculate position
                Vector2 newPos = new Vector2(Mathf.Cos(angle) * (ringRadius + systemCenterRadius), Mathf.Sin(angle) * (ringRadius + systemCenterRadius));
                Instantiate(systemPrefab, newPos, Quaternion.identity, transform.GetChild(i + 1));
            }
        }
        //Get all current systems within scene
        systems = GetAllGalaxySystems();

        //Remove colliding systems (check if they are valid)
        for (int i = 0; i < systems.Length; i++)
        {
            CheckValidSystem(systems[i]);
        }
        //Get all current systems within scene after some have been removed
        systems = GetAllGalaxySystems();

        //For every system.
        for (int i = 0; i < systems.Length; i++)
        {
            //TODO: Calculate frequency of objects correctly, treat frequency as percentage. e.g. 17% of all nodes are energy resource. 
            //Calculate 17% of how many nodes there are, then iterate over systems array this many times and set nodes to that type.
            if (systems[i] != null)
            {
                GalaxyNode node = systems[i].GetComponent<GalaxyNode>();
                if (i % shopFrequency == 0)
                {
                    node.SetNodeType(GalaxyNode.NodeType.ShopRepair);
                    node.SetColor(shopRepairColor);
                    node.name = "Node " + i + " (Shop / Repair)";
                }
                else if (i % fuelFrequency == 0)
                {
                    node.SetNodeType(GalaxyNode.NodeType.Fuel);
                    node.SetColor(fuelColor);
                    node.name = "Node " + i + " (Fuel)";
                }
                else if (i % energyFrequency == 0)
                {
                    node.SetNodeType(GalaxyNode.NodeType.Energy);
                    node.SetColor(energyColor);
                    node.name = "Node " + i + " (Energy)";
                }
                else
                {
                    node.SetNodeType(GalaxyNode.NodeType.None);
                    node.SetColor(defaultColor);
                    node.name = "Node " + i + " (Default)";
                }
            }
        }
    }

    void CheckValidSystem(GameObject system)
    {
        //Get location of current system
        Vector2 location = system.transform.position;
        //For each system in galaxy
        for (int i = 0; i < systems.Length; i++)
        {
            //Check its not null
            if (systems[i] != null) 
            {
                //If current system is equal to iteration system, then skip it
                if(systems[i] == system)
                {
                    continue;
                }
                //Calculate distance
                Vector2 currentNodePosition = systems[i].transform.position;
                float distance = Vector2.Distance(location, currentNodePosition);
                //Destroy object if closer than collisionDistance
                if (distance <= systemCollisionDistance)
                {
                    DestroyImmediate(system);
                }
            }
        }
    }

    //Return an array of gameobjects that are all tagged with "GalaxySystem"
    GameObject[] GetAllGalaxySystems()
    {
        return GameObject.FindGameObjectsWithTag("GalaxySystem");
    }
}
