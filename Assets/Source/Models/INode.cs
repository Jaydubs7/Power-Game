using UnityEngine;
// Interface for nodes to implement
public interface INode
{
    Vector2 Position { get; }
    NodeType Type { get; }
}