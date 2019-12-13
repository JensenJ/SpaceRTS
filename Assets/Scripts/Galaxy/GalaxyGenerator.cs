using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class GalaxyGenerator : MonoBehaviour
{

    [Header("References:")]
    [SerializeField]
    GameObject systemPrefab = null;
    [SerializeField]
    GameObject ringPrefab = null;
    [SerializeField]
    ProgressBar galaxyLoadingBar = null;
    [SerializeField]
    Camera playerCamera = null;

    //Inspector settings for galaxy generation
    [Header("System Position Generation Settings: ")]
    [SerializeField]
    [Range(0.01f, 1.0f)]
    float systemDensity = 0.6f;
    [SerializeField]
    [Range(50.0f, 250.0f)]
    float systemRadius = 100.0f;
    [SerializeField]
    [Range(10.0f, 100.0f)]
    float ringIntervalMultiplier = 25.0f;
    [SerializeField]
    [Range(0.0f, 50.0f)]
    float systemCenterRadius = 25.0f;
    [SerializeField]
    [Range(1, 8)]
    int systemRingCount = 4;
    [SerializeField]
    [Range(0.01f, 5.0f)]
    float systemCollisionDistance = 1.5f;
    [SerializeField]
    [Range(0.25f, 6.0f)]
    float systemRingWidth = 1.0f;
    [SerializeField]
    [Range(3.0f, 20.0f)]
    float systemConnectRange = 5.0f;
    [SerializeField]
    bool systemCollisions = true;
    [SerializeField]
    bool randomGeneration = false;

    [SerializeField]
    int numberOfSystemsGenerated = 0;

    [SerializeField]
    bool systemConnections = true;

    GameObject[] systems;

    //Resource Settings
    [Header("System Resource Generation Settings: ")]
    [SerializeField]
    bool systemResources = true;

    [SerializeField]
    public GalaxyGenerationResourceData[] systemResourcesData;

    [Header("System Faction Generation Setting:")]
    [SerializeField]
    bool systemFactions = true;
    [SerializeField]
    [Range(1, 50)]
    int numberOfFactions = 8;

    [SerializeField]
    FactionData[] factions;

    [Header("System Misc settings: ")]
    [SerializeField]
    GameObject nodeUIPrefab = null;
    [SerializeField]
    GameObject nodeResourceInfoUI = null;

    //Debug settings
    [Header("Debug: ")]
    [SerializeField]
    bool debugMode = false;
    [SerializeField]
    int coroutineGenerateYieldIntervals = 50;
    [SerializeField]
    int coroutineCollisionYieldIntervals = 10;
    [SerializeField]
    int coroutineResourceYieldIntervals = 10;
    [SerializeField]
    bool displayGenerationTime = false;
    [SerializeField]
    bool printGenerationDataToCSV = false;
    [SerializeField]
    float generationCost = 0;
    [SerializeField]
    bool repeatGenerations = false;
    [SerializeField]
    int numberOfTimesToGenerate = 1;
    [SerializeField]
    int currentGeneration = 0;

    //Loading bars
    [Space(10)]
    [SerializeField]
    float positionLoading = 0.0f;
    [SerializeField]
    float collisionLoading = 0.0f;
    [SerializeField]
    float resourceLoading = 0.0f;
    [SerializeField]
    float connectLoading = 0.0f;
    [SerializeField]
    float totalLoading = 0.0f;

    //Data arrays
    float[] timings;
    float[] densities;
    float[] centerRadii;
    int[] ringCounts;
    float[] ringWidths;
    float[] collisionDistances;
    float[] radii;
    int[] numberOfSystems;
    float[] ringIntervals;
    float[] generationCosts;
    
    //Other
    IEnumerator currentGenerateCoroutine;
    float startTime;

    // Start is called before the first frame update
    void Start()
    {
        if (galaxyLoadingBar == null)
        {
            Debug.LogError("Loading bar is not assigned!");
        }

        if(repeatGenerations == false || debugMode == false)
        {
            numberOfTimesToGenerate = 1;
        }

        //Initialise data arrays for CSV
        timings = new float[numberOfTimesToGenerate];
        densities = new float[numberOfTimesToGenerate];
        centerRadii = new float[numberOfTimesToGenerate];
        ringCounts = new int[numberOfTimesToGenerate];
        ringWidths = new float[numberOfTimesToGenerate];
        collisionDistances = new float[numberOfTimesToGenerate];
        radii = new float[numberOfTimesToGenerate];
        ringIntervals = new float[numberOfTimesToGenerate];
        numberOfSystems = new int[numberOfTimesToGenerate];
        generationCosts = new float[numberOfTimesToGenerate];

        if (currentGenerateCoroutine != null)
        {
            StopCoroutine(currentGenerateCoroutine);
        }

        currentGenerateCoroutine = CreateGalaxyCoroutine();
        StartCoroutine(currentGenerateCoroutine);
        
    }

    // Update is called once per frame
    void Update()
    {
        totalLoading = Mathf.Clamp01((positionLoading + collisionLoading + resourceLoading + connectLoading) / 4);
        galaxyLoadingBar.SetCurrentFill(totalLoading, 0.0f, 1.0f);
        //If finished loading
        if (totalLoading >= 1)
        {
            galaxyLoadingBar.DisableBar();
        }
        else if(galaxyLoadingBar.IsBarEnabled() == false)
        {
            galaxyLoadingBar.EnableBar();
        }
    }

    //Public function to load a galaxy
    public void LoadGalaxy(List<GalaxyNode> nodes)
    {
        ClearGalaxyMap();
        if (currentGenerateCoroutine != null)
        {
            StopCoroutine(currentGenerateCoroutine);
        }

        currentGenerateCoroutine = LoadGalaxyCoroutine(nodes);
        StartCoroutine(currentGenerateCoroutine);

    }

    //function to clear the galaxy map by destroying the rings
    public void ClearGalaxyMap()
    {
        //For every galaxy ring, destroy it
        for (int i = 0; i < transform.childCount; i++)
        {
            Destroy(transform.GetChild(i).gameObject);
        }
    }

    //Coroutine to load the galaxy map
    IEnumerator LoadGalaxyCoroutine(List<GalaxyNode> galaxyNodes)
    {
        //Reset loading bars
        positionLoading = 0.0f;
        collisionLoading = 0.0f;
        resourceLoading = 0.0f;
        connectLoading = 0.0f;
        totalLoading = 0.0f;

        //Calculate number of rings
        int numberOfRings = 0;
        for (int i = 0; i < galaxyNodes.Count; i++)
        {
            numberOfRings = galaxyNodes[i].currentRing;
        }
        //Instantiate those rings
        for (int i = 0; i < numberOfRings; i++)
        {
            GameObject ring = Instantiate(ringPrefab, new Vector3(0, 0), Quaternion.identity, transform);
            ring.name = "GalaxyRing " + (i + 1);
        }

        //For every node to be created
        for (int i = 0; i < galaxyNodes.Count; i++)
        {
            //Instantiate to a position
            GameObject node = Instantiate(systemPrefab, galaxyNodes[i].position, Quaternion.identity, transform.GetChild(galaxyNodes[i].currentRing - 1));
            node.name = "Node " + (i - 1);

            GalaxyNode galaxyNode = node.GetComponent<GalaxyNode>();
            galaxyNode.UpdateGalaxyNodeData(galaxyNodes[i].currentRing, galaxyNodes[i].features);

            if (i % coroutineGenerateYieldIntervals == 0)
            {
                yield return null;
            }
            positionLoading = Mathf.Clamp01((float)i / (float)systemRingCount);
        }

        positionLoading = 1.0f;
        collisionLoading = 1.0f;

        resourceLoading = 1.0f;

        GameObject[] nodes = GetAllGalaxySystems();

        for (int i = 0; i < nodes.Length; i++)
        {
            if (nodes[i] != null)
            {
                //Get node
                GalaxyNode node = nodes[i].GetComponent<GalaxyNode>();

                if (systemConnections)
                {
                    //Generate connecting nodes for every node, used in AI pathfinding
                    GameObject[] systemsToConnect = CheckRangeOfSystems(nodes[i], nodes, systemConnectRange, false);
                    //Connect these nodes
                    for (int j = 0; j < systemsToConnect.Length; j++)
                    {
                        //Add to connecting node list
                        GalaxyNode currentConnectNode = systemsToConnect[j].GetComponent<GalaxyNode>();;
                        node.AddConnectingNode(currentConnectNode);
                        //Coroutine yield interval
                        if (i % coroutineCollisionYieldIntervals == 0)
                        {
                            yield return null;
                        }
                    }
                    //Loading calculation
                    connectLoading = Mathf.Clamp01((float)i / (float)nodes.Length);
                }

                //Node setup
                node.CreateNodeUI(nodeUIPrefab, nodeResourceInfoUI);
                node.UpdateGalaxyNodeData(node.currentRing, node.features);
                //coroutine yield check
                if (i % coroutineResourceYieldIntervals == 0)
                {
                    yield return null;
                }
            }
        }
        connectLoading = 1.0f;
    }

    //Coroutine to spawn the galaxy, coroutine needed otherwise performance is awful.
    IEnumerator CreateGalaxyCoroutine()
    {
        for (int k = 0; k < numberOfTimesToGenerate; k++)
        {
            //Resetting loading bars
            positionLoading = 0.0f;
            collisionLoading = 0.0f;
            resourceLoading = 0.0f;
            connectLoading = 0.0f;
            totalLoading = 0.0f;

            numberOfSystemsGenerated = 0;
            startTime = Time.time;

            //If random generation
            if (randomGeneration == true)
            {
                systemDensity = Random.Range(0.10f, 0.12f);
                systemCenterRadius = Random.Range(0.0f, 25.0f);
                systemRingCount = Random.Range(4, 7);
                systemRingWidth = Random.Range(3.5f, 4.5f);
            }

            //Calculating other variables using randomly generated or selected values
            systemRadius = systemRingCount * ringIntervalMultiplier;
            generationCost = systemRingCount * systemRingWidth * systemDensity;

            //Generation cost warnings
            if(generationCost > 6.0f)
            {
                Debug.LogWarning("High generation cost");
            }
            else if(generationCost > 4.0f)
            {
                Debug.LogWarning("Medium generation cost");
            }
            else
            {
                Debug.Log("Low generation cost");
            }

            //Array assignments
            densities[k] = systemDensity;
            centerRadii[k] = systemCenterRadius;
            ringCounts[k] = systemRingCount;
            ringWidths[k] = systemRingWidth;
            collisionDistances[k] = systemCollisionDistance;
            radii[k] = systemRadius;
            ringIntervals[k] = ringIntervalMultiplier;
            generationCosts[k] = generationCost;

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
                    //Spawn systems randomly
                    float angle = Random.Range(0.0f, 1.0f) * Mathf.PI * 2f;
                    Vector2 newPos = new Vector2(Mathf.Cos(angle) * (ringRadius + systemCenterRadius) + Random.Range(-systemRingCount, systemRingWidth), 
                        Mathf.Sin(angle) * (ringRadius + systemCenterRadius) + Random.Range(-systemRingWidth, systemRingWidth));
                    
                    //Calculate position
                    GameObject node = Instantiate(systemPrefab, newPos, Quaternion.identity, transform.GetChild(i));
                    node.GetComponent<GalaxyNode>().currentRing = i + 1;
                    if(j % coroutineGenerateYieldIntervals == 0)
                    {
                        yield return null;
                    }
                }
                //Loading calculation
                positionLoading = Mathf.Clamp01((float)i / (float)systemRingCount);
            }
            positionLoading = 1.0f;
            //Get all current systems within scene
            systems = GetAllGalaxySystems();

            if (systemCollisions == true)
            {
                //Remove colliding systems (check if they are valid)
                for (int i = 0; i < systems.Length; i++)
                {
                    //Get all systems within range of systems[i]
                    GameObject[] systemsToDestroy = CheckRangeOfSystems(systems[i], systems, systemCollisionDistance, true);
                    //Destroy those systems
                    for (int j = 0; j < systemsToDestroy.Length; j++)
                    {
                        DestroyImmediate(systemsToDestroy[j]);
                    }
                    //Coroutine yield interval
                    if (i % coroutineCollisionYieldIntervals == 0)
                    {
                        yield return null;
                    }
                    //Loading calculation
                    collisionLoading = Mathf.Clamp01((float)i / (float)systems.Length);
                }
                //Get all current systems within scene after some have been removed
                systems = GetAllGalaxySystems();
            }
            collisionLoading = 1.0f;

            //Calculate number of systems
            numberOfSystemsGenerated = systems.Length;
            numberOfSystems[k] = numberOfSystemsGenerated;

            //If resources are enabled
            if (systemResources == true)
            {
                //For every resource in galaxy gen data
                for (int i = 0; i < systemResourcesData.Length; i++)
                {
                    //Generating resource data
                    systemResourcesData[i].currentNodeCount = Mathf.FloorToInt(systemResourcesData[i].resourcePercentage / 100 * systems.Length);
                    GenerateResourceNodes(systemResourcesData[i], i);
                }
            }
            resourceLoading = 1.0f;

            //Assign systems a node id
            for (int i = 0; i < systems.Length; i++)
            {
                systems[i].GetComponent<GalaxyNode>().nodeID = i;
            }

            //For every system
            SaveData.current.galaxyNodes = new List<GalaxyNode>();
            for (int i = 0; i < systems.Length; i++)
            {
                if(systems[i] != null)
                {
                    //Get node
                    GalaxyNode node = systems[i].GetComponent<GalaxyNode>();

                    if (systemConnections)
                    {
                        //Generate connecting nodes for every node, used in AI pathfinding
                        GameObject[] systemsToConnect = CheckRangeOfSystems(systems[i], systems, systemConnectRange, false);
                        //Connect these nodes
                        for (int j = 0; j < systemsToConnect.Length; j++)
                        {
                            //Add to connecting node list
                            GalaxyNode currentConnectNode = systemsToConnect[j].GetComponent<GalaxyNode>();
                            node.AddConnectingNode(currentConnectNode);
                            //Coroutine yield interval
                            if (i % coroutineCollisionYieldIntervals == 0)
                            {
                                yield return null;
                            }

                        }
                        //Loading calculation
                        connectLoading = Mathf.Clamp01((float)i / (float)systems.Length);
                    }

                    //Node setup
                    node.name = "Node " + i;
                    node.CreateNodeUI(nodeUIPrefab, nodeResourceInfoUI);
                    node.UpdateGalaxyNodeData(node.currentRing, node.features);
                    //coroutine yield check
                    if(i % coroutineResourceYieldIntervals == 0)
                    {
                        yield return null;
                    }
                    SaveData.current.galaxyNodes.Add(node);
                }
            }
            connectLoading = 1.0f;

            //If factions are enabled.
            if (systemFactions == true)
            {
                //Create faction starting systems.
                factions = Factions.CreateFactions(numberOfFactions, systems);
                float x = factions[0].homeSystem.transform.position.x;
                float y = factions[0].homeSystem.transform.position.y;

                playerCamera.GetComponent<CameraMovement>().InitializeCameraSettings(new Vector3(x, y, -10), systemRadius, 15.0f, systemRadius);
            }

            //Calculate time taken
            timings[k] = Time.time - startTime;
            if (debugMode == true)
            {
                RunDebugging(k);
                currentGeneration++;
            }
        }
    }

    //Checks the range of systems and returns an array with all overlapping actors
    GameObject[] CheckRangeOfSystems(GameObject system, GameObject[] systems, float checkDistance, bool returnTarget)
    {
        //Create list
        List<GameObject> rangedSystems = new List<GameObject>();
        //Null pointer check
        if (system != null)
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
                    if (systems[i] == system)
                    {
                        continue;
                    }
                    //Calculate distance
                    Vector2 currentNodePosition = systems[i].transform.position;
                    float distance = Vector2.Distance(location, currentNodePosition);
                    //Destroy object if closer than collisionDistance
                    if (distance <= checkDistance)
                    {
                        //Should the function return the parameter system passed in?
                        if (returnTarget)
                        {
                            rangedSystems.Add(system);
                        }
                        else
                        {
                            rangedSystems.Add(systems[i]);
                        }
                    }
                }
            }
        }
        return rangedSystems.ToArray();
    }

    //Determines which nodes should be resource nodes
    void GenerateResourceNodes(GalaxyGenerationResourceData data, int currentResource)
    {
        //Repeat for the number of nodes that are designated as the nodetype
        for (int i = 0; i < data.currentNodeCount; i++)
        {
            //Random node
            GalaxyNode node = systems[Random.Range(0, systems.Length)].GetComponent<GalaxyNode>();
            //Null pointer check
            if (node != null)
            {
                //Node resource addition
                node.AddResource(data.nodeResourceType, Mathf.FloorToInt(data.resourceRichnessMultiplier * Random.Range(data.minResourceRichness, data.maxResourceRichness)), Random.Range(data.minProductionRate, data.maxProductionRate));
                
                //Loading calculation
                resourceLoading = Mathf.Clamp01((float)(i  * (currentResource+1)) / (float)(systemResourcesData.Length * data.currentNodeCount));
            }
        }
    }

    //Return an array of gameobjects that are all tagged with "GalaxySystem"
    GameObject[] GetAllGalaxySystems()
    {
        return GameObject.FindGameObjectsWithTag("GalaxySystem");
    }

    void RunDebugging(int currentGeneration)
    {
        //Display generation times
        if (displayGenerationTime == true)
        {
            Debug.Log("Time taken to generate galaxy " + (currentGeneration + 1)+ ": " + timings[currentGeneration] + " seconds");
            //Total time taken for generation
            if (numberOfTimesToGenerate == currentGeneration + 1)
            {
                float total = 0.0f;
                //For every timing, add to total
                for (int i = 0; i < timings.Length; i++)
                {
                    total += timings[i];
                }
                //Print total
                Debug.Log("Total time taken to generate " + numberOfTimesToGenerate + " galaxies: " + total + " seconds");
            }
        }
        //print data to CSV only when on last generation
        if (printGenerationDataToCSV == true && numberOfTimesToGenerate == currentGeneration + 1)
        {
            int numberOfHeadings = 13;
            string[,] data = new string[numberOfHeadings, numberOfTimesToGenerate];
            string[] headings = new string[numberOfHeadings];

            //Headings
            headings[0] = "Time (s):";
            headings[1] = "Density";
            headings[2] = "Center Radius";
            headings[3] = "Ring Count";
            headings[4] = "Ring Width";
            headings[5] = "Collision Dist";
            headings[6] = "Radius";
            headings[7] = "Ring Intervals";
            headings[8] = "System Count";
            headings[9] = "Generation Cost";

            //Data for each heading
            for (int i = 0; i < numberOfTimesToGenerate; i++)
            {
                data[0, i] = timings[i].ToString("n4");
                data[1, i] = densities[i].ToString("n4");
                data[2, i] = centerRadii[i].ToString("n4");
                data[3, i] = ringCounts[i].ToString();
                data[4, i] = ringWidths[i].ToString("n4");
                data[5, i] = collisionDistances[i].ToString("n4");
                data[6, i] = radii[i].ToString();
                data[7, i] = ringIntervals[i].ToString("n4");
                data[8, i] = numberOfSystems[i].ToString();
                data[9, i] = generationCosts[i].ToString();
            }
            Utilities.WriteToCSV("GenerationData", headings, data);
        }
        //Repeatedly destroy galaxy and recreate until the number of generations is met
        if (repeatGenerations == true)
        {
            //Only destroy if not last galaxy to generate.
            if (numberOfTimesToGenerate != currentGeneration + 1)
            {
                for (int i = 0; i < systemRingCount; i++)
                {
                    Destroy(transform.GetChild(i + 1).gameObject);
                }
            }
        }
    }
}