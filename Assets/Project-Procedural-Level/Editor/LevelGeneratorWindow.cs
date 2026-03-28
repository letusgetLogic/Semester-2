
using UnityEditor;
using UnityEngine;

namespace ProceduralLevelToolkit.Editor
{
    public class LevelGeneratorWindow : EditorWindow
    {
        private enum GeneratorType { Dungeon, Terrain }
        private GeneratorType selectedType = GeneratorType.Dungeon;

        // Common
        private string seed = "";
        private bool autoClearPrevious = true;

        // Dungeon
        private int dungeonWidth = 60;
        private int dungeonHeight = 60;
        private float dungeonFillPercent = 0.45f;
        private int dungeonSmoothSteps = 5;
        private GameObject wallPrefab;
        private GameObject floorPrefab;

        // Terrain
        private int terrainWidth = 100;
        private int terrainHeight = 100;
        private float terrainScale = 24f;
        private float terrainHeightMultiplier = 8f;
        private Gradient terrainGradient = new Gradient();
        private Material terrainMaterial;

        [MenuItem("Tools/Procedural Level Toolkit")]
        public static void ShowWindow()
        {
            GetWindow<LevelGeneratorWindow>("Procedural Level Toolkit");
        }

        /// <summary>
        /// OnGUI.
        /// </summary>
        private void OnGUI()
        {
            GUILayout.Label("Procedural Level Toolkit", EditorStyles.boldLabel);

            selectedType = (GeneratorType)EditorGUILayout.EnumPopup("Generator Type", selectedType);
            seed = EditorGUILayout.TextField(new GUIContent("Seed (optional)", "Leave empty for random seed"), seed);
            autoClearPrevious = EditorGUILayout.Toggle(new GUIContent("Auto Clear Previous", "Deletes old generated content before generating"), autoClearPrevious);

            EditorGUILayout.Space(8);

            if (selectedType == GeneratorType.Dungeon)
            {
                DrawDungeonGUI();
            }
            else
            {
                DrawTerrainGUI();
            }

            EditorGUILayout.Space(10);
            if (GUILayout.Button("Clear Generated"))
            {
                ClearGenerated();
            }
        }

        /// <summary>
        /// Draw dungeon.
        /// </summary>
        private void DrawDungeonGUI()
        {
            GUILayout.Label("Dungeon Settings", EditorStyles.boldLabel);
            dungeonWidth = EditorGUILayout.IntSlider("Width", dungeonWidth, 10, 300);
            dungeonHeight = EditorGUILayout.IntSlider("Height", dungeonHeight, 10, 300);
            dungeonFillPercent = EditorGUILayout.Slider(new GUIContent("Fill %", "Initial wall fill percent for cellular automata"), dungeonFillPercent, 0.10f, 0.70f);
            dungeonSmoothSteps = EditorGUILayout.IntSlider("Smooth Steps", dungeonSmoothSteps, 0, 10);

            wallPrefab = (GameObject)EditorGUILayout.ObjectField(new GUIContent("Wall Prefab", "Optional—uses Cube if null"), wallPrefab, typeof(GameObject), false);
            floorPrefab = (GameObject)EditorGUILayout.ObjectField(new GUIContent("Floor Prefab", "Optional—uses Quad if null"), floorPrefab, typeof(GameObject), false);

            EditorGUILayout.Space(8);
            if (GUILayout.Button("Generate Dungeon"))
            {
                if (autoClearPrevious) ClearGenerated();
                GenerateDungeon();
            }
        }

        /// <summary>
        /// Draw terrain.
        /// </summary>
        private void DrawTerrainGUI()
        {
            GUILayout.Label("Terrain Settings", EditorStyles.boldLabel);
            terrainWidth = EditorGUILayout.IntSlider("Width", terrainWidth, 16, 512);
            terrainHeight = EditorGUILayout.IntSlider("Height", terrainHeight, 16, 512);
            terrainScale = EditorGUILayout.Slider("Noise Scale", terrainScale, 4f, 128f);
            terrainHeightMultiplier = EditorGUILayout.Slider("Height Multiplier", terrainHeightMultiplier, 1f, 100f);
            terrainGradient = EditorGUILayout.GradientField("Vertex Gradient", terrainGradient);
            terrainMaterial = (Material)EditorGUILayout.ObjectField(new GUIContent("Material", "Optional—Unity default if null"), terrainMaterial, typeof(Material), false);

            EditorGUILayout.Space(8);
            if (GUILayout.Button("Generate Terrain"))
            {
                if (autoClearPrevious) ClearGenerated();
                GenerateTerrain();
            }
        }

        /// <summary>
        /// Clear generated map.
        /// </summary>
        private void ClearGenerated()
        {
            var found = GameObject.FindObjectsOfType<ProceduralLevelToolkit.Runtime.GeneratedTag>();
            foreach (var g in found)
            {
                if (g != null)
                {
                    if (Application.isPlaying) Destroy(g.gameObject);
                    else DestroyImmediate(g.gameObject);
                }
            }
        }

        /// <summary>
        /// Build random generator.
        /// </summary>
        /// <returns></returns>
        private System.Random BuildRng()
        {
            int s = string.IsNullOrEmpty(seed) ? UnityEngine.Random.Range(int.MinValue, int.MaxValue) : seed.GetHashCode();
            return new System.Random(s);
        }

        /// <summary>
        /// Generate dungeon.
        /// </summary>
        private void GenerateDungeon()
        {
            var go = new GameObject("Dungeon_Generated");
            go.AddComponent<ProceduralLevelToolkit.Runtime.GeneratedTag>();
            var gen = go.AddComponent<ProceduralLevelToolkit.Runtime.Dungeon.DungeonGenerator>();
            gen.Width = dungeonWidth;
            gen.Height = dungeonHeight;
            gen.FillPercent = dungeonFillPercent;
            gen.SmoothSteps = dungeonSmoothSteps;
            gen.WallPrefab = wallPrefab;
            gen.FloorPrefab = floorPrefab;
            gen.Rng = BuildRng();

            gen.Generate();
            Selection.activeGameObject = go;
        }

        /// <summary>
        /// Generate terrain.
        /// </summary>
        private void GenerateTerrain()
        {
            var go = new GameObject("Terrain_Generated");
            go.AddComponent<ProceduralLevelToolkit.Runtime.GeneratedTag>();
            var mf = go.AddComponent<MeshFilter>();
            var mr = go.AddComponent<MeshRenderer>();
            if (terrainMaterial != null) mr.sharedMaterial = terrainMaterial;

            var gen = go.AddComponent<ProceduralLevelToolkit.Runtime.Terrain.TerrainGenerator>();
            gen.Width = terrainWidth;
            gen.Height = terrainHeight;
            gen.Scale = terrainScale;
            gen.HeightMultiplier = terrainHeightMultiplier;
            gen.ColorGradient = terrainGradient;
            gen.Rng = BuildRng();

            gen.Generate();
            Selection.activeGameObject = go;
        }
    }
}
