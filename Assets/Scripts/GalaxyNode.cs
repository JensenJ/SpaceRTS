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
    int totalResource = 100;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public NodeType GetNodeType()
    {
        return type;
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

    public void SetResourceType(NodeResource m_resource)
    {
        resource = m_resource;
    }

    public void SetColor(Color newColor)
    {
        SpriteRenderer renderer = GetComponent<SpriteRenderer>();
        renderer.color = newColor;
    }
}
