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
    [Range(10.0f, 100.0f)]
    float ringIntervalMultiplier = 25.0f;
    [SerializeField]
    [Range(0.0f, 50.0f)]
    float systemCenterRadius = 25.0f;
    [SerializeField]
    [Range(0, 8)]
    int systemRingCount = 4;
    [SerializeField]
    [Range(0.01f, 5.0f)]
    float systemCollisionDistance = 1.5f;
    [SerializeField]
    bool systemCollisions = true;
    [SerializeField]
    bool randomGeneration = false;

    [SerializeField]
    int numberOfSystemsGenerated = 0;

    [Space(10)]
    [SerializeField]
    bool systemEquidistant = true;
    [SerializeField]
    [Range(0.25f, 4.0f)]
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
    public GalaxyGenerationResourceData[] systemResourcesData;

    [Header("System Faction Generation Setting:")]
    [SerializeField]
    bool systemFactions = true;
    [SerializeField]
    int numberOfFactions = 8;

    IEnumerator currentGenerateCoroutine;
    float startTime;

    [Header("Debug: ")]
    [SerializeField]
    bool debugMode = false;
    [SerializeField]
    int coroutineGenerateYieldIntervals = 50;
    [SerializeField]
    int coroutineCollisionYieldIntervals = 10;
    [SerializeField]
    bool displayGenerationTime = false;
    [SerializeField]
    bool printGenerationDataToCSV = false;

    [SerializeField]
    bool repeatGenerations = false;
    [SerializeField]
    int numberOfTimesToGenerate = 1;
    [SerializeField]
    int currentGeneration = 0;

    float[] timings;
    float[] densities;
    float[] centerRadii;
    int[] ringCounts;
    float[] ringWidths;
    float[] collisionDistances;
    float[] radii;
    int[] numberOfSystems;
    float[] ringIntervals;

    // Start is called before the first frame update
    void Start()
    {
        if(repeatGenerations == false || debugMode == false)
        {
            numberOfTimesToGenerate = 1;
        }


        //Initialise diagnostics arrays
        timings = new float[numberOfTimesToGenerate];
        densities = new float[numberOfTimesToGenerate];
        centerRadii = new float[numberOfTimesToGenerate];
        ringCounts = new int[numberOfTimesToGenerate];
        ringWidths = new float[numberOfTimesToGenerate];
        collisionDistances = new float[numberOfTimesToGenerate];
        radii = new float[numberOfTimesToGenerate];
        ringIntervals = new float[numberOfTimesToGenerate];
        numberOfSystems = new int[numberOfTimesToGenerate];


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
        for (int k = 0; k < numberOfTimesToGenerate; k++)
        {
            numberOfSystemsGenerated = 0;
            startTime = Time.time;
            if (randomGeneration == true)
            {
                systemDensity = Random.Range(0.2f, 0.25f);
                systemCenterRadius = Random.Range(0.0f, 25.0f);
                systemRingCount = Random.Range(4, 6);
                systemRingWidth = Random.Range(1.25f, 3.0f);
                systemCollisionDistance = Random.Range(1.5f, 2.25f);
                systemRadius = systemRingCount * ringIntervalMultiplier;

                densities[k] = systemDensity;
                centerRadii[k] = systemCenterRadius;
                ringCounts[k] = systemRingCount;
                ringWidths[k] = systemRingWidth;
                collisionDistances[k] = systemCollisionDistance;
                radii[k] = systemRadius;
                ringIntervals[k] = ringIntervalMultiplier;
            }
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
                    if(j % coroutineGenerateYieldIntervals == 0)
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
                    if(i % coroutineCollisionYieldIntervals == 0)
                    {
                        yield return null;
                    }
                }
                //Get all current systems within scene after some have been removed
                systems = GetAllGalaxySystems();
            }

            //Calculate number of systems
            numberOfSystemsGenerated = systems.Length;
            numberOfSystems[k] = numberOfSystemsGenerated;

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

            if (systemFactions == true)
            {
                FactionData[] factions = Factions.CreateFactions(numberOfFactions, systems);
                for (int i = 0; i < factions.Length; i++)
                {
                    GalaxyNode startSystem = factions[i].homeSystem;
                    Color factionColour = factions[i].factionColour;
                    startSystem.SetOwningFaction(i, factionColour);
                    startSystem.SetNodeType(GalaxyNode.NodeType.Planet);
                }
            }

            //Calculate time taken
            timings[k] = Time.time - startTime;
            if (debugMode == true)
            {
                //Display generation times
                if (displayGenerationTime == true)
                {
                    Debug.Log("Time taken to generate galaxy " + k + ": " + timings[k] + " seconds");
                    //Total time taken for generation
                    if(numberOfTimesToGenerate == k + 1)
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
                if(printGenerationDataToCSV == true && numberOfTimesToGenerate == k + 1)
                {
                    int numberOfHeadings = 9;
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
                    }
                    Utilities.WriteToCSV("GenerationData", headings, data);
                }
                //Repeatedly destroy galaxy and recreate until the number of generations is met
                if (repeatGenerations == true)
                {
                    //Only destroy if not last galaxy to generate.
                    if (numberOfTimesToGenerate != k + 1)
                    {
                        for (int i = 0; i < systemRingCount; i++)
                        {
                            //timings[i] = Time.time - startTime;
                            Destroy(transform.GetChild(i + 1).gameObject);
                        }
                    }
                }
                currentGeneration++;
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
    void GenerateResourceNodes(int count, Resources.ResourceType resourceType, Color nodeColour, float resourceRichness)
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