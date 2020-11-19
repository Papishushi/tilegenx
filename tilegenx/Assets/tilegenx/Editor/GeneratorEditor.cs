using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps.tilegenX;

[CustomEditor(typeof(WorldCreator)), CanEditMultipleObjects]
public class GeneratorEditor : Editor
{

    public SerializedProperty
        grid_property,
        tilemap_property,
        player_property,

        seed_property,
        amplitude_property,
        lacunarity_property,

        layerMode_Property,
        size_Property,
        x_Property,
        y_Property,
        offsetX_Property,
        offsetY_Property,
        circularGridMode_Property;

    void OnEnable()
    {
        // Setup the SerializedProperties
        grid_property = serializedObject.FindProperty("grid");
        tilemap_property = serializedObject.FindProperty("tilemap");
        player_property = serializedObject.FindProperty("player");

        seed_property = serializedObject.FindProperty("seed");
        amplitude_property = serializedObject.FindProperty("amplitude");
        lacunarity_property = serializedObject.FindProperty("lacunarity");


        layerMode_Property = serializedObject.FindProperty("layerMode");
        size_Property = serializedObject.FindProperty("size");
        x_Property = serializedObject.FindProperty("x");
        y_Property = serializedObject.FindProperty("y");
        offsetX_Property = serializedObject.FindProperty("offsetX");
        offsetY_Property = serializedObject.FindProperty("offsetY");
        circularGridMode_Property = serializedObject.FindProperty("circularGridMode");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.PropertyField(grid_property);
        EditorGUILayout.PropertyField(tilemap_property);
        EditorGUILayout.PropertyField(player_property);
        EditorGUILayout.Space(20);
        EditorGUILayout.PropertyField(layerMode_Property);
        EditorGUILayout.PropertyField(seed_property);
        EditorGUILayout.Slider(amplitude_property, 0.000001f, 5, new GUIContent("Amplitude"));
        EditorGUILayout.Slider(lacunarity_property, 0.000001f, 5, new GUIContent("Lacunarity"));
        EditorGUILayout.Space(20);

        Generator.TileLayerMode layerMode = (Generator.TileLayerMode)layerMode_Property.enumValueIndex;

        switch (layerMode)
        {
            case Generator.TileLayerMode.Standard:
                EditorGUILayout.IntSlider(x_Property, 1, 100, new GUIContent("X Size"));
                EditorGUILayout.IntSlider(y_Property, 1, 100, new GUIContent("Y Size"));
                break;

            case Generator.TileLayerMode.Cross:
                EditorGUILayout.IntSlider(x_Property, 1, 100, new GUIContent("X Size"));
                EditorGUILayout.IntSlider(y_Property, 1, 100, new GUIContent("Y Size"));
                EditorGUILayout.IntSlider(size_Property, 0, 50, new GUIContent("Size"));
                EditorGUILayout.IntSlider(offsetX_Property, -50, 50, new GUIContent("X Offset"));
                EditorGUILayout.IntSlider(offsetY_Property, -50, 50, new GUIContent("Y Offset"));
                break;

            case Generator.TileLayerMode.Circular:
                EditorGUILayout.IntSlider(size_Property, 0, 50, new GUIContent("Size"));
                EditorGUILayout.PropertyField(circularGridMode_Property);
                break;
            default:

                break;

        }


        serializedObject.ApplyModifiedProperties();
    }
}
