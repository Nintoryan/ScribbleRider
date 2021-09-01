#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

[InitializeOnLoad]
public static class TaxiEditorLayerManager
{
    static TaxiEditorLayerManager()
    {
        CreateLayer("Wheels");
        Physics.IgnoreLayerCollision(LayerMask.NameToLayer("Wheels"), LayerMask.NameToLayer("Wheels"), true);
    }

    /// <summary>
    /// Create a layer at the next available index. Returns silently if layer already exists.
    /// </summary>
    /// <param name="name">Name of the layer to create</param>
    private static void CreateLayer(string name)
    {
        if (string.IsNullOrEmpty(name))
            throw new System.ArgumentNullException("name", "New layer name string is either null or empty.");

        var tagManager = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset")[0]);
        var layerProps = tagManager.FindProperty("layers");
        var propCount = layerProps.arraySize;

        SerializedProperty firstEmptyProp = null;

        for (var i = 0; i < propCount; i++)
        {
            var layerProp = layerProps.GetArrayElementAtIndex(i);

            var stringValue = layerProp.stringValue;

            if (stringValue == name) return;

            if (i < 8 || stringValue != string.Empty) continue;

            if (firstEmptyProp == null)
                firstEmptyProp = layerProp;
        }

        if (firstEmptyProp == null)
        {
            UnityEngine.Debug.LogError("Maximum limit of " + propCount + " layers exceeded. Layer \"" + name +
                                       "\" not created.");
            return;
        }

        firstEmptyProp.stringValue = name;
        tagManager.ApplyModifiedProperties();
    }
}
#endif