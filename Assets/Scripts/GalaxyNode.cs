using UnityEngine.UI;
using UnityEngine;

public class GalaxyNode : MonoBehaviour
{

    public enum NodeType {
        None,
        Resource,
        Planet,
        Station
    }

    [SerializeField]
    private NodeType type = NodeType.None;

    [SerializeField]
    private Resources.ResourceType resourceType = Resources.ResourceType.None;

    [SerializeField]
    int currentResource = 10;

    [SerializeField]
    int owningFactionID = 0;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void SetNodeType(NodeType m_type)
    {
        type = m_type;
    }

    public void SetResourceType(Resources.ResourceType m_resource, int resourceAmount)
    {
        resourceType = m_resource;
        currentResource = resourceAmount;
    }

    public void SetOwningFaction(int factionID, Color factionColour)
    {
        owningFactionID = factionID;
        SpriteRenderer renderer = transform.GetChild(1).GetComponent<SpriteRenderer>();
        renderer.color = factionColour;
    }

    public void SetResourceColor(Color resourceColor)
    {
        SpriteRenderer renderer = transform.GetChild(0).GetComponent<SpriteRenderer>();
        renderer.color = resourceColor;
    }

    public Resources.ResourceType GetResourceType()
    {
        return resourceType;
    }
    public NodeType GetNodeType()
    {
        return type;
    }

    public int GetOwningFactionID()
    {
        return owningFactionID;
    }
}
