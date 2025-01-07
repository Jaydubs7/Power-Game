using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using Electricity;

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
        private int minConnectionsPerConsumer = 1;  // Minimum number of connections per consumer

        private List<GameObject> generatorNodesList = new List<GameObject>();
        private List<GameObject> consumerNodesList = new List<GameObject>();
        private List<GameObject> resourceNodesList = new List<GameObject>();
        private List<GameObject> connectionLines = new List<GameObject>();

        private void Start()
        {
            GenerateNodes();
            CreateInitialDistribution();
        }

        private void GenerateNodes()
        {
            // Generate resource nodes
            for (int i = 0; i < resourceNodes.count; i++)
            {
                Vector2 randomPosition = GetRandomPosition();
                GameObject node = Instantiate(resourceNodes.nodePrefab, new Vector3(randomPosition.x, randomPosition.y, 0), Quaternion.identity);
                resourceNodesList.Add(node);
            }

            // Generate generator nodes
            for (int i = 0; i < generatorNodes.count; i++)
            {
                Vector2 randomPosition = GetRandomPosition();
                GameObject node = Instantiate(generatorNodes.nodePrefab, new Vector3(randomPosition.x, randomPosition.y, 0), Quaternion.identity);
                generatorNodesList.Add(node);
            }

            // Generate consumer nodes
            for (int i = 0; i < consumerNodes.count; i++)
            {
                Vector2 randomPosition = GetRandomPosition();
                GameObject node = Instantiate(consumerNodes.nodePrefab, new Vector3(randomPosition.x, randomPosition.y, 0), Quaternion.identity);
                consumerNodesList.Add(node);
            }
        }

        private void CreateInitialDistribution()
        {

            var randomGenerators = generatorNodesList
                    .OrderBy(x => Random.value)
                    .Take(minConnectionsPerConsumer)
                    .ToList();

            foreach (var generator in randomGenerators)
            {
                var nearestConsumers = consumerNodesList.OrderBy(x => Vector2.Distance(x.transform.position, generator.transform.position)).ToList();

                for (int i = 0; i < minConnectionsPerConsumer; i++)
                {

                    var consumer = nearestConsumers[i];
                    CreateConnectionLine(generator.transform.position, consumer.transform.position);
                    var object1 = new InfrastructureLine(generator.transform.position, consumer.transform.position);
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

        private Vector2 GetRandomPosition()
        {
            return new Vector2(
                Random.Range(-mapSize.x / 2, mapSize.x / 2),
                Random.Range(-mapSize.y / 2, mapSize.y / 2)
            );
        }
    }