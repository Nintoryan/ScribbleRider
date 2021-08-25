using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using XDPaint.Tools;

namespace XDPaint.Editor
{
    public class TrianglesDataWindow : EditorWindow
    {
        private PaintManager _paintManager;

        public void SetPaintManager(PaintManager paintManager)
        {
            _paintManager = paintManager;
            TrianglesData.OnUpdate = progress =>
            {
                if (EditorUtility.DisplayCancelableProgressBar("Updating", "Updating triangles data, please wait...", progress))
                {
                    TrianglesData.Break();
                    _paintManager.ClearTrianglesNeighborsData();
                };
            };
            TrianglesData.OnFinish = EditorUtility.ClearProgressBar;
        }

        void OnGUI()
        {
            EditorGUI.BeginDisabledGroup(true);
            GUILayout.TextArea(string.Empty, GUI.skin.horizontalSlider, GUILayout.Height(EditorGUIUtility.singleLineHeight));
            EditorGUI.EndDisabledGroup();
            GUILayout.Label("Press 'Fill triangles data' to fill mesh triangles data.", EditorStyles.label);
            GUILayout.Label("Note, that it may take a few minutes.", EditorStyles.boldLabel);
            EditorGUI.BeginDisabledGroup(true);
            GUILayout.TextArea(string.Empty, GUI.skin.horizontalSlider, GUILayout.Height(EditorGUIUtility.singleLineHeight));
            EditorGUI.EndDisabledGroup();
            if (GUILayout.Button("Fill triangles data"))
            {
                if (Selection.activeGameObject != null)
                {
                    var paintManager = Selection.activeGameObject.GetComponent<PaintManager>();
                    if (paintManager != null)
                    {
                        _paintManager = paintManager;
                    }
                }
                else
                {
                    Debug.LogWarning("Selected GameObject is null.");
                    return;
                }
                if (_paintManager == null)
                {
                    Debug.LogWarning("Can't find PaintManager in Selected GameObject.");
                    return;
                }
                _paintManager.FillTrianglesData();
                if (!Application.isPlaying)
                {
                    EditorUtility.SetDirty(_paintManager);
                    EditorSceneManager.MarkSceneDirty(_paintManager.gameObject.scene);
                }
                Close();
            }
        }
    }
}