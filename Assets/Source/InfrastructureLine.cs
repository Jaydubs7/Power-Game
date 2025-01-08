using UnityEngine;

public class InfrastructureLine 
{
    private INode startNode;
    private INode endNode;
    private LineRenderer lineRenderer;
    private bool isResourceFlowing;
    private float distance;
    private Color activeColor = Color.green;
    private Color inactiveColor = Color.gray;
    public Vector2 Start => startNode.Position;
    public Vector2 End => endNode.Position;
    public float Distance => distance;
    public bool IsActive => isResourceFlowing;
    public InfrastructureLine(INode start, INode end, GameObject lineObject)
    {
        startNode = start;
        endNode = end;
        
        // Setup line renderer
        lineRenderer = lineObject.AddComponent<LineRenderer>();
        InitializeLineRenderer();
        
        // Calculate initial distance
        UpdateDistance();
        
        // Initial state is inactive
        isResourceFlowing = false;
        UpdateLineState();
    }
    private void InitializeLineRenderer()
    {
        lineRenderer.startWidth = 0.1f;
        lineRenderer.endWidth = 0.1f;
        lineRenderer.positionCount = 2;
        lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        UpdateLinePosition();
    }
    public void UpdateLinePosition()
    {
        lineRenderer.SetPosition(0, new Vector3(Start.x, Start.y, 0));
        lineRenderer.SetPosition(1, new Vector3(End.x, End.y, 0));
        UpdateDistance();
    }
    private void UpdateDistance()
    {
        distance = Vector2.Distance(Start, End);
    }
    public void SetResourceFlowState(bool isFlowing)
    {
        isResourceFlowing = isFlowing;
        UpdateLineState();
    }
    private void UpdateLineState()
    {
        lineRenderer.startColor = isResourceFlowing ? activeColor : inactiveColor;
        lineRenderer.endColor = isResourceFlowing ? activeColor : inactiveColor;
    }
    public float CalculateResourceCost(float costPerUnit)
    {
        return distance * costPerUnit;
    }
}
public enum NodeType
{
    Producer,
    Consumer,
    Storage,
    Distribution
}
