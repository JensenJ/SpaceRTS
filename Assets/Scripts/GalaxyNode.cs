using UnityEngine.UI;
using UnityEngine;

public class GalaxyNode : MonoBehaviour
{

    public enum NodeType {
        None,
        Energy,
        Fuel,
        ShopRepair
    }

    [SerializeField]
    private NodeType type = NodeType.None;

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
            case NodeType.Energy:
                break;
            case NodeType.Fuel:
                break;
            case NodeType.ShopRepair:
                break;
        }
    }

    public void SetColor(Color newColor)
    {
        SpriteRenderer renderer = GetComponent<SpriteRenderer>();
        renderer.color = newColor;
    }
}
