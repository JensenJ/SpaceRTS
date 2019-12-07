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
    [Range(1, 8)]
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
    //Debug settings
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

    IEnumerator currentGenerateCoroutine;
    float startTime;
    [SerializeField]
    Camera playerCamera = null;

    // Start is called before the first frame update
    void Start()
    {
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

    //Coroutine to spawn the galaxy, coroutine needed otherwise performance is awful.
    IEnumerator SpawnGalaxy()
    {
        for (int k = 0; k < numberOfTimesToGenerate; k++)
        {
            numberOfSystemsGenerated = 0;
            startTime = Time.time;

            //If random generation
            if (randomGeneration == true)
            {
                systemDensity = Random.Range(0.2f, 0.25f);
                systemCenterRadius = Random.Range(0.0f, 25.0f);
                systemRingCount = Random.Range(4, 7);
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
                    node.name = "Node " + i;

                }
            }

            //If resources are enabled
            if (systemResources == true)
            {
                //Prevents infinite loop
                for (int i = 0; i < systemResourcesData.Length; i++)
                {
                    //Generating resource data
                    systemResourcesData[i].currentNodeCount = Mathf.FloorToInt(systemResourcesData[i].resourcePercentage / 100 * systems.Length);
                    GenerateResourceNodes(systemResourcesData[i]);
                }
            }

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
                    headings[9] = "Faction Count";

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
                        data[9, i] = numberOfFactions.ToString();
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
    void GenerateResourceNodes(GalaxyGenerationResourceData data)
    {
        //Repeat for the number of nodes that are designated as the nodetype
        for (int j = 0; j < data.currentNodeCount; j++)
        {
            //Random node
            GalaxyNode node = systems[Random.Range(0, systems.Length)].GetComponent<GalaxyNode>();
            //Null pointer check
            if (node != null)
            {
                //Node settings
                node.AddResource(data.nodeResourceType, Mathf.FloorToInt(data.resourceRichnessMultiplier * Random.Range(3000.0f, 7000.0f)), Random.Range(data.minProductionRate, data.maxProductionRate));
                node.name = node.name + " (" + data.nodeResourceType.ToString() + ")";
            }
        }
    }

    //Return an array of gameobjects that are all tagged with "GalaxySystem"
    GameObject[] GetAllGalaxySystems()
    {
        return GameObject.FindGameObjectsWithTag("GalaxySystem");
    }
}