
using UnityEngine;

namespace ProceduralLevelToolkit.Runtime.Placement
{
    public class ObjectPlacer : MonoBehaviour
    {
        public GameObject[] Prefabs;
        public int Count = 50;
        public Vector2 AreaSize = new Vector2(80, 80);
        public float MinScale = 0.8f;
        public float MaxScale = 1.2f;
        
        /// <summary>
        /// Very simple placer that scatters prefabs on a mesh within a rect.
        /// </summary>
        /// <param name="rng"></param>
        public void Scatter(System.Random rng = null)
        {
            if (Prefabs == null || Prefabs.Length == 0)
                return;

            if (rng == null) 
                rng = new System.Random();

            var root = new GameObject("PlacedObjects");
            root.transform.SetParent(transform, false);

            for (int i = 0; i < Count; i++)
            {
                var prefab = Prefabs[rng.Next(Prefabs.Length)];
#if UNITY_EDITOR
                var go = (GameObject)UnityEditor.PrefabUtility.InstantiatePrefab(prefab);
#else
                var go = Instantiate(prefab);
#endif
                go.transform.SetParent(root.transform, false);

                // random position within area
                float x = (float)rng.NextDouble() * AreaSize.x;
                float z = (float)rng.NextDouble() * AreaSize.y;
                go.transform.position = new Vector3(x, 0f, z);

                // random look direction
                float rotY = (float)rng.NextDouble() * 360f;
                go.transform.rotation = Quaternion.Euler(0f, rotY, 0f);

                // random scale within scale range
                float s = Mathf.Lerp(MinScale, MaxScale, (float)rng.NextDouble());
                go.transform.localScale = Vector3.one * s;
            }
        }
    }
}
