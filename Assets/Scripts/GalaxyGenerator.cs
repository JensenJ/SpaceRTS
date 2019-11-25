using UnityEngine;

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
    [Range(0.01f, 1.0f)]
    float systemRingWidth = 0.1f;

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
        int totalSystemCount = 0;
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
                GameObject system = Instantiate(systemPrefab, position, Quaternion.identity, transform);
                system.name = "System: " + totalSystemCount;
                totalSystemCount++;
            }
        }

        systems = GameObject.FindGameObjectsWithTag("GalaxySystem");
        for (int i = 0; i < systems.Length; i++)
        {
            //bool valid = CheckValidSystem(systems[i]);
            //if(valid == false)
            //{
            //    Destroy(systems[i]);
            //}


            //TODO: Calculate frequency of objects correctly, treat frequency as percentage. e.g. 17% of all nodes are energy resource. 
            //Calculate 17% of how many nodes there are, then iterate over systems array this many times and set nodes to that type.

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

    //void SpawnGalaxy(int systemCount, float galaxyRadius)
    //{
    //    //Positioning
    //    int currentCount = 0;
    //    while(currentCount < systemCount)
    //    {
    //        for (int i = 0; i < numberOfRings; i++)
    //        {
    //            float b = segments * (i + 1);
    //            float c = b - (widthOfRings / 100);
    //            float d = b + (widthOfRings / 100);
    //            print(c);
    //            print(d);

    //            float a = Random.Range(0.0f, 1.0f) * 2 * 3.14f;
    //            float r = galaxyRadius * Mathf.Sqrt(Random.Range(c, d));
    //            Vector2 position = new Vector2((r * Mathf.Cos(a)) / 10.0f, (r * Mathf.Sin(a)) / 10.0f);

    //            GameObject system = Instantiate(objectToSpawn, position, Quaternion.identity, transform);

    //            //bool valid = CheckValidSystem(system);
    //            bool valid = true;
    //            if (valid)
    //            {
    //                systems[currentCount] = system;
    //                currentCount++;
    //            }
    //            else
    //            {
    //                Destroy(system);
    //            }

    //            //Node types
    //            GalaxyNode node = system.GetComponent<GalaxyNode>();
    //            if (currentCount % shopFrequency == 0)
    //            {
    //                node.SetNodeType(GalaxyNode.NodeType.ShopRepair);
    //                node.SetColor(shopRepairColor);
    //                node.name = "Node " + currentCount + " (Shop / Repair)";
    //            }
    //            else if (currentCount % fuelFrequency == 0)
    //            {
    //                node.SetNodeType(GalaxyNode.NodeType.Fuel);
    //                node.SetColor(fuelColor);
    //                node.name = "Node " + currentCount + " (Fuel)";
    //            }
    //            else if (currentCount % energyFrequency == 0)
    //            {
    //                node.SetNodeType(GalaxyNode.NodeType.Energy);
    //                node.SetColor(energyColor);
    //                node.name = "Node " + currentCount + " (Energy)";
    //            }
    //            else
    //            {
    //                node.SetNodeType(GalaxyNode.NodeType.None);
    //                node.SetColor(defaultColor);
    //                node.name = "Node " + currentCount + " (Default)";
    //            }
    //        }
    //    }
    //}

    bool CheckValidSystem(GameObject system)
    {
        Vector2 location = system.transform.position;
        for (int i = 0; i < systems.Length; i++)
        {
            if (systems[i] != null)
            {
                Vector2 currentNodePosition = systems[i].transform.position;
                float distance = Vector2.Distance(currentNodePosition, location);
                if (distance <= 1.5f)
                {
                    return false;
                }
            }
        }
        return true;
    }
}
