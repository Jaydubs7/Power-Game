# Region-based Node Placement Configuration

The map generator now supports regional grouping of nodes through the RegionConfig system. Each node type (Resource, Generator, Consumer) can be configured with its own region settings in the Unity Inspector.

## Region Configuration Parameters

- `center`: Vector2 defining the center point of the region
- `size`: Vector2 defining the width and height of the region
- `density`: Float value that controls how tightly clustered the nodes are (higher values = tighter clustering)

## Setup Instructions

1. In the Unity Inspector, locate the MapGenerator component
2. Configure the three region sections:
   - Resource Region
   - Generator Region
   - Consumer Region
3. For each region, set:
   - Center point coordinates
   - Region size
   - Density factor (default = 1.0)

## Tips

- Keep regions within the map boundaries (defined by mapSize)
- Use density values > 1.0 for tighter clustering
- Use density values < 1.0 for more spread out nodes
- Regions can overlap if desired