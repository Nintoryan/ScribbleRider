#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
namespace YTaxi
{

    [InitializeOnLoad]
    public static class LayerEditorManager
    {
        static LayerEditorManager()
        {
            CreateLayer("Wheels");
            IgnoreLayerCollision("Wheels","Wheels",true);
        }
        private static void CreateLayer(string name)
        {
            if (string.IsNullOrEmpty(name))
                throw new System.ArgumentNullException("name", "Layer name string is null.");

            var tagManager =
                new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset")[0]);
            var layerProps = tagManager.FindProperty("layers");
            var propCount = layerProps.arraySize;

            SerializedProperty firstEmptyProp = null;

            for (var i = 0; i < propCount; i++)
            {
                var layerProp = layerProps.GetArrayElementAtIndex(i);

                var stringValue = layerProp.stringValue;

                if (stringValue == name) return;

                if (i < 8 || stringValue != string.Empty) continue;

                firstEmptyProp ??= layerProp;
            }

            if (firstEmptyProp == null)
            {
                Debug.LogError("Maximum of " + propCount + " layers exceeded. Layer \"" + name +
                                           "\" not created.");
                return;
            }

            firstEmptyProp.stringValue = name;
            tagManager.ApplyModifiedProperties();
        }

        private static void IgnoreLayerCollision(string layer1, string layer2, bool ignore)
        {
            Physics.IgnoreLayerCollision(LayerMask.NameToLayer(layer1), LayerMask.NameToLayer(layer2), ignore);
        }
    }
}
#endif