using UnityEngine.UI;
using UnityEngine;

public class GalaxyNode : MonoBehaviour
{

    public enum NodeType {
        None,
        Resource,
        Shop
    }

    public enum NodeResource
    {
        None,
        Energy,
        Fuel,
        Minerals
    }

    [SerializeField]
    private NodeType type = NodeType.None;

    [SerializeField]
    private NodeResource resource = NodeResource.None;

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

        switch (type) {
            case NodeType.None:
                break;
            case NodeType.Resource:
                break;
            case NodeType.Shop:
                break;
        }
    }

    public void SetResourceType(NodeResource m_resource, int resourceAmount)
    {
        resource = m_resource;
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

    public NodeResource GetResourceType()
    {
        return resource;
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
