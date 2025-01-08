using UnityEngine;

public class NodeBehaviour : MonoBehaviour, INode
{
    [SerializeField] private NodeType nodeType;

    public Vector2 Position => transform.position;
    public NodeType Type => nodeType;

    public void SetNodeType(NodeType type)
    {
        nodeType = type;
    }

    // Optional: Add methods for resource management
    public virtual void ProcessResources()
    {
        // Implementation depends on node type
    }
}
