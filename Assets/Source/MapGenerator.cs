using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class MapGenerator : MonoBehaviour
{
    [System.Serializable]
    public class NodeTypeConfig
    {
        public GameObject nodePrefab;
        public int count;
    }
    [Header("Node Types")]
    [SerializeField]
    private NodeTypeConfig resourceNodes;
    [SerializeField]
    private NodeTypeConfig generatorNodes;
    [SerializeField]
    private NodeTypeConfig consumerNodes;
    [Header("Map Settings")]
    [SerializeField]
    private Vector2 mapSize = new Vector2(10f, 10f);  // Size of the map in units
    [Header("Distribution Settings")]
    [SerializeField]
    private GameObject connectionLinePrefab;  // Prefab for the connection line
    [SerializeField]
    private int minConnectionsPerGenerator = 1;  // Minimum number of connections per consumer
    [SerializeField]
    private int minConnectionsPerGenToResource = 1;  // Minimum number of connections per consumer
    private List<NodeBehaviour> generatorNodesList = new List<NodeBehaviour>();
    private List<NodeBehaviour> consumerNodesList = new List<NodeBehaviour>();
    private List<NodeBehaviour> resourceNodesList = new List<NodeBehaviour>();
    private List<GameObject> connectionLines = new List<GameObject>();
    private List<InfrastructureLine> infrastructureLines = new List<InfrastructureLine>();
    private GameObject linesParent;
    private void Start()
    {
        linesParent = new GameObject("Infrastructure Lines");
        linesParent.transform.SetParent(transform);
        
        GenerateNodes();
        CreateInitialDistribution();
    }
    private void GenerateNodes()
    {
        // Generate resource nodes
        for (int i = 0; i < resourceNodes.count; i++)
        {
            Vector2 randomPosition = GetRandomPosition();
            GameObject nodeObj = Instantiate(resourceNodes.nodePrefab, 
                new Vector3(randomPosition.x, randomPosition.y, 0), 
                Quaternion.identity);
            
            NodeBehaviour node = SetupNode(nodeObj, NodeType.Storage);
            resourceNodesList.Add(node);
        }

        // Generate generator nodes
        for (int i = 0; i < generatorNodes.count; i++)
        {
            Vector2 randomPosition = GetRandomPosition();
            GameObject nodeObj = Instantiate(generatorNodes.nodePrefab, 
                new Vector3(randomPosition.x, randomPosition.y, 0), 
                Quaternion.identity);
            
            NodeBehaviour node = SetupNode(nodeObj, NodeType.Producer);
            generatorNodesList.Add(node);
        }

        // Generate consumer nodes
        for (int i = 0; i < consumerNodes.count; i++)
        {
            Vector2 randomPosition = GetRandomPosition();
            GameObject nodeObj = Instantiate(consumerNodes.nodePrefab, 
                new Vector3(randomPosition.x, randomPosition.y, 0), 
                Quaternion.identity);
            
            NodeBehaviour node = SetupNode(nodeObj, NodeType.Consumer);
            consumerNodesList.Add(node);
        }
    }

    private NodeBehaviour SetupNode(GameObject nodeObject, NodeType type)
    {
        nodeObject.transform.SetParent(transform);

        // Add NodeBehaviour if it doesn't exist
        NodeBehaviour nodeBehaviour = nodeObject.GetComponent<NodeBehaviour>();
        if (nodeBehaviour == null)
        {
            nodeBehaviour = nodeObject.AddComponent<NodeBehaviour>();
        }

        // Set the node type
        nodeBehaviour.SetNodeType(type);

        return nodeBehaviour;
    }


    private void CreateInitialDistribution()
    {

        foreach (var generator in generatorNodesList)
        {
            var nearestConsumers = consumerNodesList.OrderBy(x => Vector2.Distance(x.transform.position, generator.transform.position)).ToList();
            var nearestResources = resourceNodesList.OrderBy(x => Vector2.Distance(x.transform.position, generator.transform.position)).ToList();   
            if (nearestConsumers.Count < minConnectionsPerGenToResource)
                minConnectionsPerGenerator= nearestConsumers.Count;
            for (int i = 0; i < minConnectionsPerGenerator; i++)
            {
                var consumer = nearestConsumers[i];
                CreateConnectionLine(generator.transform.position, consumer.transform.position);
                CreateInfrastructureLine(generator, consumer);
            }

            if (nearestResources.Count < minConnectionsPerGenToResource)
                minConnectionsPerGenToResource = nearestResources.Count;
            for (int i = 0; i < minConnectionsPerGenerator; i++)
            {
                var resource = nearestResources[i];
                CreateConnectionLine(generator.transform.position, resource.transform.position);
                CreateInfrastructureLine(generator, resource);
            }


        }
    }

    private void CreateConnectionLine(Vector3 start, Vector3 end)
    {
        GameObject line = Instantiate(connectionLinePrefab, Vector3.zero, Quaternion.identity);
        LineRenderer lineRenderer = line.GetComponent<LineRenderer>();
        if (lineRenderer == null)
        {
            Debug.LogError("LineRenderer component not found on the connection line prefab.");
            return;
        }
        
        lineRenderer.SetPosition(0, start);
        lineRenderer.SetPosition(1, end);
        lineRenderer.startWidth = 0.1f;  
        connectionLines.Add(line);
    }

    private void CreateInfrastructureLine(NodeBehaviour start, NodeBehaviour end)
    {
        GameObject lineObject = new GameObject($"Line_{start.name}_to_{end.name}");
        lineObject.transform.SetParent(linesParent.transform);

        var line = new InfrastructureLine(start, end, lineObject);
        infrastructureLines.Add(line);

        // Optional: Set initial flow state based on distance or other criteria
        float maxDistance = 10f; // Example maximum distance
        bool shouldFlow = Vector2.Distance(start.Position, end.Position) <= maxDistance;
        line.SetResourceFlowState(shouldFlow);
    }

    private Vector2 GetRandomPosition()
    {
        return new Vector2(
            Random.Range(-mapSize.x / 2, mapSize.x / 2),
            Random.Range(-mapSize.y / 2, mapSize.y / 2)
        );
    }
}