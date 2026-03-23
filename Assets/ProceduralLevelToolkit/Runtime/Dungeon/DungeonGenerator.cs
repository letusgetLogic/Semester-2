
using UnityEngine;

namespace ProceduralLevelToolkit.Runtime.Dungeon
{
    public class DungeonGenerator : MonoBehaviour
    {
        [Header("Grid Size")]
        public int Width = 60;
        public int Height = 60;

        [Range(0.1f, 0.7f)]
        public float FillPercent = 0.45f;

        [Range(0, 10)]
        public int SmoothSteps = 5;

        [Header("Prefabs (optional)")]
        public GameObject WallPrefab;
        public GameObject FloorPrefab;

        [System.NonSerialized] public System.Random Rng;

        private int[,] map;

        /// <summary>
        /// Generate dungeon.
        /// </summary>
        public void Generate()
        {
            if (Rng == null) Rng = new System.Random();
            map = new int[Width, Height];

            // Initialize with random walls (cellular automata seed)
            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    bool isBorder = x == 0 || y == 0 || x == Width - 1 || y == Height - 1;
                    
                    // Is the target border? Value is always 1 (wall) else
                    // if random value is less than fill percent, is 1 / more than, is 0 (floor)
                    map[x, y] = (isBorder || Rng.NextDouble() < FillPercent) ? 1 : 0;
                }
            }

            // Smooth
            for (int i = 0; i < SmoothSteps; i++)
                SmoothMap();

            // Build geometry
            Build();
        }

        /// <summary>
        /// Smooth the floor and wall.
        /// </summary>
        private void SmoothMap()
        {
            int[,] newMap = new int[Width, Height];
            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    // number of neighbours of wall
                    int neighbours = CountWallNeighbours(x, y);

                    // when around the target are more than 4 walls, make it to wall
                    if (neighbours > 4) 
                        newMap[x, y] = 1;
                    else 
                    if (neighbours < 4)  // else less than 4, make it to floor
                        newMap[x, y] = 0;
                    else 
                        newMap[x, y] = map[x, y]; // has 4 wall neighbours
                }
            }
            map = newMap;
        }

        /// <summary>
        /// Count wall neighbours.
        /// </summary>
        /// <param name="cx"> represents current row </param>
        /// <param name="cy"> represents current column </param>
        /// <returns></returns>
        private int CountWallNeighbours(int cx, int cy)
        {
            int count = 0;

            // we count x from -1 to +1
            for (int x = cx - 1; x <= cx + 1; x++)
            {
                // foreach x count y from -1 to +1
                for (int y = cy - 1; y <= cy + 1; y++)
                {
                    // when the checked index is equal to origin, continue
                    if (x == cx && y == cy) 
                        continue;

                    // when the index is out of bounds, it is wall, add 1 to count
                    if (x < 0 || y < 0 || x >= Width || y >= Height) 
                        count++;
                    else // otherwise add index number (0 = floor, 1 = wall) to count
                        count += map[x, y];
                }
            }
            return count;
        }

        /// <summary>
        /// Build the map.
        /// </summary>
        private void Build()
        {
            // Simple tile instantiation
            var parent = new GameObject("Tiles");
            parent.transform.SetParent(transform, false);

#if UNITY_EDITOR
            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    if (map[x, y] == 1)
                    {
                        // Wall
                        GameObject g = WallPrefab != null
                            ? (GameObject)UnityEditor.PrefabUtility.InstantiatePrefab(WallPrefab)
                            : GameObject.CreatePrimitive(PrimitiveType.Cube);

                        if (WallPrefab == null)
                        {
                            var mr = g.GetComponent<MeshRenderer>();
                            if (mr != null)
                                mr.sharedMaterial = UnityEditor.AssetDatabase.GetBuiltinExtraResource<Material>("Default-Diffuse.mat");
                        }

                        g.transform.SetParent(parent.transform, false);
                        g.transform.position = new Vector3(x, 0.5f, y);
                    }
                    else
                    {
                        // Floor
                        if (FloorPrefab != null)
                        {

                            GameObject g = (GameObject)UnityEditor.PrefabUtility.InstantiatePrefab(FloorPrefab);
                            g.transform.SetParent(parent.transform, false);
                            g.transform.position = new Vector3(x, 0f, y);
                        }
                        else
                        {
                            GameObject q = GameObject.CreatePrimitive(PrimitiveType.Quad);
                            q.transform.SetParent(parent.transform, false);
                            q.transform.rotation = Quaternion.Euler(90f, 0f, 0f);
                            q.transform.position = new Vector3(x, 0f, y);
                        }
                    }
                }
            }
#endif
        }
    }
}
