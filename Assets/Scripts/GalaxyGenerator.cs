using UnityEngine;
using System.Linq;

public class GalaxyGenerator : MonoBehaviour
{
    //Node Colours
    [Space(20)]
    [SerializeField]
    Color defaultColor;

    [Space(10)]
    [SerializeField]
    Color shopRepairColor;
    [SerializeField]
    int shopFrequency;

    [Space(10)]
    [SerializeField]
    Color fuelColor;
    [SerializeField]
    int fuelFrequency;

    [Space(10)]
    [SerializeField]
    Color energyColor;
    [SerializeField]
    int energyFrequency;

    private float segments = 0;

    [Header("Galaxy Generation Settings: ")]
    [Space(20)]
    [SerializeField]
    [Range(1.0f, 10.0f)]
    float systemDensity = 0.6f;
    [SerializeField]
    [Range(50.0f, 200.0f)]
    float systemRadius = 100.0f;
    [SerializeField]
    [Range(0, 20)]
    int systemRingCount = 8;
    [SerializeField]
    [Range(0.01f, 10.0f)]
    float systemCollisionDistance = 1.5f;

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
            float segmentSize = systemRadius / systemRingCount;
            float ringRadius = segmentSize * (i + 1);

            //Calculate ring radius min/max
            float minRingRadius = ringRadius - systemRingWidth;
            float maxRingRadius = ringRadius + systemRingWidth;

            //Calculate ring area
            float upperArea = maxRingRadius * maxRingRadius * 3.14f;
            float lowerArea = minRingRadius * minRingRadius * 3.14f;
            float ringArea = upperArea - lowerArea;

            //Calculate number of systems for each ring using formula: mass (number of systems) = density * volume (area).
            int numberOfSystemsForRing = Mathf.FloorToInt(systemDensity * ringArea);

            //Instantiate ring object for ring image, give gameobject ring name
            GameObject galaxyRing = Instantiate(ringPrefab, new Vector2(0, 0), Quaternion.identity, transform);
            galaxyRing.name = "GalaxyRing " + (i + 1);

            //For each system in a ring
            for (int j = 0; j < numberOfSystemsForRing; j++)
            {
                //Generate position
                float a = Random.Range(0.0f, 1.0f) * 2 * 3.14f;
                float r = systemRadius * Mathf.Sqrt(Random.Range(minRingRadius, maxRingRadius));
                Vector2 position = new Vector2(r * Mathf.Cos(a) / 10.0f, r * Mathf.Sin(a) / 10.0f);

                //GameObject system = Instantiate(systemPrefab, position, Quaternion.identity, transform.GetChild(i + 1));
                //Instantiate system object
                Instantiate(systemPrefab, position, Quaternion.identity, transform);
            }
        }
        systems = GetAllGalaxySystems();

        for (int i = 0; i < systems.Length; i++)
        {
            CheckValidSystem(systems[i]);
        }

        systems = GetAllGalaxySystems();

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
            else
            {
                print("error");
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

    GameObject[] GetAllGalaxySystems()
    {
        return GameObject.FindGameObjectsWithTag("GalaxySystem");
    }
}
