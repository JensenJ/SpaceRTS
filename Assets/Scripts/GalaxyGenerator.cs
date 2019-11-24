using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GalaxyGenerator : MonoBehaviour
{
    [SerializeField]
    GameObject objectToSpawn = null;
    [SerializeField]
    GameObject[] systems;

    [SerializeField]
    int systemCount = 1000;
    [SerializeField]
    float galaxyRadius = 100.0f;

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
            
        }
    }

    bool CheckValidSystem(GameObject system)
    {
        Vector3 location = system.transform.position;
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
