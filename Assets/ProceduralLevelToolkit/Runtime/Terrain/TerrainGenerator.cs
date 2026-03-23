
using UnityEngine;

namespace ProceduralLevelToolkit.Runtime.Terrain
{
    [RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
    public class TerrainGenerator : MonoBehaviour
    {
        public int Width = 100;
        public int Height = 100;
        public float Scale = 24f;
        public float HeightMultiplier = 8f;
        public Gradient ColorGradient;

        [System.NonSerialized] public System.Random Rng;

        /// <summary>
        /// Generate terrain.
        /// </summary>
        public void Generate()
        {
            if (Rng == null) Rng = new System.Random();

            var mesh = new Mesh();
            int vCount = (Width + 1) * (Height + 1);
            var vertices = new Vector3[vCount];
            var uvs = new Vector2[vCount];
            var colors = new Color[vCount];
            var triangles = new int[Width * Height * 6];

            int i = 0;
            for (int z = 0; z <= Height; z++)
            {
                for (int x = 0; x <= Width; x++, i++)
                {
                    float nx = (float)x / Scale;
                    float nz = (float)z / Scale;
                    float height = Mathf.PerlinNoise(nx, nz) * HeightMultiplier;

                    vertices[i] = new Vector3(x, height, z);
                    uvs[i] = new Vector2((float)x / Width, (float)z / Height);

                    float t = Mathf.InverseLerp(0f, HeightMultiplier, height);
                    if (ColorGradient != null) colors[i] = ColorGradient.Evaluate(t);
                    else colors[i] = new Color(t, t, t, 1f);
                }
            }

            int vi = 0, ti = 0;
            for (int z = 0; z < Height; z++)
            {
                for (int x = 0; x < Width; x++)
                {
                    triangles[ti + 0] = vi + 0;
                    triangles[ti + 1] = vi + Width + 1;
                    triangles[ti + 2] = vi + 1;
                    triangles[ti + 3] = vi + 1;
                    triangles[ti + 4] = vi + Width + 1;
                    triangles[ti + 5] = vi + Width + 2;
                    vi++;
                    ti += 6;
                }
                vi++;
            }

            mesh.indexFormat = (vCount > 65000) ? UnityEngine.Rendering.IndexFormat.UInt32 : UnityEngine.Rendering.IndexFormat.UInt16;
            mesh.vertices = vertices;
            mesh.uv = uvs;
            mesh.colors = colors;
            mesh.triangles = triangles;
            mesh.RecalculateNormals();

            var mf = GetComponent<MeshFilter>();
            mf.sharedMesh = mesh;

            var mr = GetComponent<MeshRenderer>();
#if UNITY_EDITOR
            if (mr.sharedMaterial == null)
                mr.sharedMaterial = UnityEditor.AssetDatabase.GetBuiltinExtraResource<Material>("Default-Diffuse.mat");
#endif
        }
    }
}
