using System.Collections;
using UnityEngine;

public class GalaxyGenerator : MonoBehaviour
{

    //Inspector settings for galaxy generation
    [Header("System Position Generation Settings: ")]
    [SerializeField]
    [Range(0.01f, 1.0f)]
    float systemDensity = 0.6f;
    [SerializeField]
    [Range(50.0f, 250.0f)]
    float systemRadius = 100.0f;
    [SerializeField]
    [Range(0.0f, 50.0f)]
    float systemCenterRadius = 25.0f;
    [SerializeField]
    [Range(0, 8)]
    int systemRingCount = 4;
    [SerializeField]
    [Range(0.01f, 2.0f)]
    float systemCollisionDistance = 1.5f;
    [SerializeField]
    bool systemCollisions = true;

    [Space(10)]
    [SerializeField]
    bool systemEquidistant = true;
    [SerializeField]
    [Range(0.25f, 1.5f)]
    float systemRingWidth = 1.0f;

    [Space(20)]
    [SerializeField]
    GameObject systemPrefab = null;
    [SerializeField]
    GameObject ringPrefab = null;

    GameObject[] systems;

    //Node Colours
    [Header("System Resource Generation Settings: ")]
    [SerializeField]
    bool systemResources = true;

    [Space(10)]
    [SerializeField]
    Color defaultColor = Color.white;

    [SerializeField]
    public GalaxyNodeResource[] systemResourcesData;

    IEnumerator currentGenerateCoroutine;

    [Header("Debug: ")]
    [SerializeField]
    int coroutineYieldIntervals = 10;

    // Start is called before the first frame update
    void Start()
    {
        if (currentGenerateCoroutine != null)
        {
            StopCoroutine(currentGenerateCoroutine);
        }

        currentGenerateCoroutine = SpawnGalaxy();
        StartCoroutine(currentGenerateCoroutine);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator SpawnGalaxy()
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

            for (int j = 0; j < numberOfSystemsForRing; j++)
            {
                Vector2 newPos;
                if (systemEquidistant)
                {
                    //Spawn systems that have an equal distance from each other.
                    float angle = j * Mathf.PI * 2f / numberOfSystemsForRing;
                    newPos = new Vector2(Mathf.Cos(angle) * (ringRadius + systemCenterRadius), Mathf.Sin(angle) * (ringRadius + systemCenterRadius));
                }
                else
                {
                    //Spawn systems randomly
                    float angle = Random.Range(0.0f, 1.0f) * Mathf.PI * 2f;
                    newPos = new Vector2(Mathf.Cos(angle) * (ringRadius + systemCenterRadius) + Random.Range(-systemRingCount, systemRingWidth), 
                        Mathf.Sin(angle) * (ringRadius + systemCenterRadius) + Random.Range(-systemRingWidth, systemRingWidth));
                }
                //Calculate position
                Instantiate(systemPrefab, newPos, Quaternion.identity, transform.GetChild(i + 1));
                if(j % (coroutineYieldIntervals * 10) == 0)
                {
                    yield return null;
                }
            }
        }
        //Get all current systems within scene
        systems = GetAllGalaxySystems();

        if (systemCollisions == true)
        {
            //Remove colliding systems (check if they are valid)
            for (int i = 0; i < systems.Length; i++)
            {
                CheckValidSystem(systems[i]);
                if(i % coroutineYieldIntervals == 0)
                {
                    yield return null;
                }
            }
            //Get all current systems within scene after some have been removed
            systems = GetAllGalaxySystems();
        }

        //For every system.
        for (int i = 0; i < systems.Length; i++)
        {
            if (systems[i] != null)
            {
                GalaxyNode node = systems[i].GetComponent<GalaxyNode>();
                node.SetNodeType(GalaxyNode.NodeType.None);
                node.SetResourceColor(defaultColor);
                node.name = "Node " + i;

            }
        }

        //If resources are enabled
        if (systemResources == true)
        {
            //Prevents infinite loop
            float totalPercentage = 0;
            for (int i = 0; i < systemResourcesData.Length; i++)
            {
                totalPercentage += systemResourcesData[i].resourcePercentage;
                if(totalPercentage > 100)
                {
                    Debug.LogError("Galaxy Resource Generation Failed: total percentages exceed 100%");
                    yield break;
                }

                systemResourcesData[i].currentNodeCount = Mathf.FloorToInt(systemResourcesData[i].resourcePercentage / 100 * systems.Length);
                GenerateResourceNodes(systemResourcesData[i].currentNodeCount, systemResourcesData[i].nodeResourceType, systemResourcesData[i].nodeColour, systemResourcesData[i].resourceRichnessMultiplier);
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

    //Determines which nodes should be resource nodes
    void GenerateResourceNodes(int count, GalaxyNode.NodeResource resourceType, Color nodeColour, float resourceRichness)
    {
        //Repeat for the number of nodes that are designated as the nodetype
        for (int j = 0; j < count; j++)
        {
            //Random node
            GalaxyNode node = systems[Random.Range(0, systems.Length)].GetComponent<GalaxyNode>();
            //Null pointer check
            if (node != null)
            {
                //Check node is not another resource
                if (node.GetNodeType() == GalaxyNode.NodeType.None)
                {
                    //Node settings
                    node.SetResourceType(resourceType, Mathf.FloorToInt(resourceRichness * Random.Range(3.0f, 7.0f)));
                    node.SetResourceColor(nodeColour);
                    node.SetNodeType(GalaxyNode.NodeType.Resource);
                    node.name = node.name + " (" + resourceType.ToString() + ")";
                }
                else
                {
                    //Keep repeating until node count fulfilled.
                    j--;
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

[System.Serializable]
public struct GalaxyNodeResource
{
    public string name;
    [Range(0.1f, 4.0f)]
    public float resourceRichnessMultiplier;
    [Range(0, 100)]
    public float resourcePercentage;
    public int currentNodeCount;
    public GalaxyNode.NodeResource nodeResourceType;
    public Color nodeColour;
}
