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

    //Galaxy spawning settings
    [Space(20)]
    [SerializeField]
    int systemCount = 1000;
    [SerializeField]
    float galaxyRadius = 100.0f;
    [SerializeField]
    GameObject objectToSpawn = null;
    [SerializeField]
    GameObject[] systems;

    // Start is called before the first frame update
    void Start()
    {
        systems = new GameObject[systemCount];
        SpawnGalaxy(systemCount, galaxyRadius);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void SpawnGalaxy(int systemCount, float galaxyRadius)
    {
        //Positioning
        int currentCount = 0;
        while(currentCount < systemCount)
        {
            float a = Random.Range(0.0f, 1.0f) * 2 * 3.14f;
            float r = galaxyRadius * Mathf.Sqrt(Random.Range(0.025f, 1.0f));

            Vector2 position = new Vector2(r * Mathf.Cos(a), r * Mathf.Sin(a));

            GameObject system = Instantiate(objectToSpawn, position, Quaternion.identity, transform);

            bool valid = CheckValidSystem(system);
            if (valid)
            {
                systems[currentCount] = system;
                currentCount++;
            }
            else
            {
                Destroy(system);
            }

            //Node types
            GalaxyNode node = system.GetComponent<GalaxyNode>();
            if (currentCount % shopFrequency == 0)
            {
                node.SetNodeType(GalaxyNode.NodeType.ShopRepair);
                node.SetColor(shopRepairColor);
                node.name = "Node " + currentCount + " (Shop / Repair)";
            }
            else if (currentCount % fuelFrequency == 0)
            {
                node.SetNodeType(GalaxyNode.NodeType.Fuel);
                node.SetColor(fuelColor);
                node.name = "Node " + currentCount + " (Fuel)";
            } else if (currentCount % energyFrequency == 0)
            {
                node.SetNodeType(GalaxyNode.NodeType.Energy);
                node.SetColor(energyColor);
                node.name = "Node " + currentCount + " (Energy)";
            }
            else
            {
                node.SetNodeType(GalaxyNode.NodeType.None);
                node.SetColor(defaultColor);
                node.name = "Node " + currentCount + " (Default)";
            }
        }
    }

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
