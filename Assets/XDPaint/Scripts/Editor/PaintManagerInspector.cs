using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
using XDPaint.Core.PaintObject.Base;
using XDPaint.Tools;

namespace XDPaint.Editor
{
	[CustomEditor(typeof(PaintManager))]
	public class PaintManagerInspector : UnityEditor.Editor
	{
		private SerializedProperty _objectForPaintingProperty;
		private SerializedProperty _paintMaterialProperty;
		private SerializedProperty _shaderTextureNameProperty;
		private SerializedProperty _overrideCameraProperty;
		private SerializedProperty _defaultTextureWidth;
		private SerializedProperty _defaultTextureHeight;
		private SerializedProperty _cameraProperty;
		private SerializedProperty _useNeighborsVerticesForRaycastsProperty;
		private SerializedProperty _trianglesContainerProperty;
		private SerializedProperty _copySourceTextureToPaintTextureProperty;
		private SerializedProperty _useSourceTextureAsBackgroundProperty;

		private PaintManager _paintManager;
		private BasePaintObject _paintObject;
		private Component _component;

		private string[] _renderTextureModes;
		private GUIContent[] _renderTextureModesContent;
		private int _renderTextureModeId;
		private int _shaderTextureNameSelectedId;
		private bool _isMeshObject;
		private bool _shouldCheckTexture = true;
		private bool _hasTexture;
		
		[MenuItem("GameObject/2D\u22153D Paint", false, 32)]
		static void AddPaintManagerObject()
		{
			var clothObject = new GameObject("2D/3D Paint");
			clothObject.AddComponent<PaintManager>();
			Selection.activeObject = clothObject.gameObject;
		}

		[MenuItem("Component/2D\u22153D Paint")]
		static void AddPaintManagerComponent()
		{
			if (Selection.activeGameObject != null && Selection.activeGameObject.GetComponent<PaintManager>() == null)
			{
				Selection.activeGameObject.AddComponent<PaintManager>();
			}
		}

		[MenuItem("CONTEXT/PaintManager/Fill Triangles Data")]
		static void AddTrianglesDataWithNeighbors(MenuCommand command)
		{
			var paintManager = (PaintManager)command.context;
			paintManager.UseNeighborsVerticesForRaycasts = true;
			OpenTrianglesDataWindow(paintManager);
		}

		[MenuItem("CONTEXT/PaintManager/Fill Triangles Data", true)]
		static bool ValidationAddTrianglesDataWithNeighbors()
		{
			var paintManager = Selection.activeGameObject.GetComponent<PaintManager>();
			var supportedComponent = PaintManagerHelper.GetSupportedComponent(paintManager.ObjectForPainting);
			return supportedComponent != null && PaintManagerHelper.IsMeshObject(supportedComponent);
		}

		[MenuItem("CONTEXT/PaintManager/Clear Triangles Data")]
		static void ClearTrianglesData(MenuCommand command)
		{
			var paintManager = (PaintManager)command.context;
			paintManager.ClearTrianglesData();
			paintManager.UseNeighborsVerticesForRaycasts = false;
			if (!Application.isPlaying)
			{
				EditorUtility.SetDirty(paintManager);
				EditorSceneManager.MarkSceneDirty(paintManager.gameObject.scene);
			}
		}
		
		[MenuItem("CONTEXT/PaintManager/Clear Triangles Data", true)]
		static bool ValidationClearTrianglesData()
		{
			var paintManager = Selection.activeGameObject.GetComponent<PaintManager>();
			var supportedComponent = PaintManagerHelper.GetSupportedComponent(paintManager.ObjectForPainting);
			return supportedComponent != null && PaintManagerHelper.IsMeshObject(supportedComponent);
		}
		
		private static void OpenTrianglesDataWindow(PaintManager paintManager)
		{
			var progressBar = (TrianglesDataWindow)EditorWindow.GetWindow(typeof(TrianglesDataWindow), false, PaintManagerHelper.TrianglesDataWindowTitle);
			progressBar.maxSize = new Vector2(PaintManagerHelper.TrianglesDataWindowSize.x, PaintManagerHelper.TrianglesDataWindowSize.y);
			progressBar.SetPaintManager(paintManager);
			progressBar.Show(false);
		}

		void OnEnable()
		{
			_paintManager = (PaintManager)target;
			_paintObject = _paintManager.PaintObject;

			_objectForPaintingProperty = serializedObject.FindProperty("ObjectForPainting");
			_paintMaterialProperty = serializedObject.FindProperty("Material.SourceMaterial");
			_shaderTextureNameProperty = serializedObject.FindProperty("Material.shaderTextureName");
			_overrideCameraProperty = serializedObject.FindProperty("ShouldOverrideCamera");
			_defaultTextureWidth = serializedObject.FindProperty("Material.defaultTextureWidth");
			_defaultTextureHeight = serializedObject.FindProperty("Material.defaultTextureHeight");
			_cameraProperty = serializedObject.FindProperty("overrideCamera");
			_useNeighborsVerticesForRaycastsProperty = serializedObject.FindProperty("useNeighborsVerticesForRaycasts");
			_trianglesContainerProperty = serializedObject.FindProperty("trianglesContainer");
			_copySourceTextureToPaintTextureProperty = serializedObject.FindProperty("CopySourceTextureToPaintTexture");
			_useSourceTextureAsBackgroundProperty = serializedObject.FindProperty("useSourceTextureAsBackground");

			UpdateTexturesList();
		}

		private void UpdateTexturesList()
		{
			var material = _paintMaterialProperty.objectReferenceValue as Material;
			if (material != null)
			{
				var shaderTextureNames = PaintManagerHelper.GetTexturesListFromShader(material);
				_shaderTextureNameSelectedId = Array.IndexOf(shaderTextureNames, _shaderTextureNameProperty.stringValue);
			}
			_paintManager.Material.InitMaterial(material);
		}

		public override void OnInspectorGUI()
		{
			serializedObject.Update();
			EditorGUI.BeginChangeCheck();
			EditorGUILayout.PropertyField(_objectForPaintingProperty, new GUIContent("Object For Painting", PaintManagerHelper.ObjectForPaintingTooltip));
			if (EditorGUI.EndChangeCheck())
			{
				_shouldCheckTexture = true;
			}
			
			if (EditorGUILayout.BeginFadeGroup(Convert.ToSingle(_objectForPaintingProperty.objectReferenceValue != null)))
			{
				_paintManager = (PaintManager)target;
				_paintObject = _paintManager.PaintObject;
				_component = PaintManagerHelper.GetSupportedComponent(_objectForPaintingProperty.objectReferenceValue as GameObject);
				_isMeshObject = PaintManagerHelper.IsMeshObject(_component);
				DrawMaterialBlock();
				if (EditorGUILayout.BeginFadeGroup(Convert.ToSingle(_paintMaterialProperty.objectReferenceValue != null)))
				{
					DrawCheckboxesBlock();
					DrawButtonsBlock();
				}
				EditorGUILayout.EndFadeGroup();
			}
			EditorGUILayout.EndFadeGroup();
			DrawAutoFillButton();
			serializedObject.ApplyModifiedProperties();
		}
		
		private void DrawAutoFillButton()
		{
			var disabled = _objectForPaintingProperty.objectReferenceValue == null || _paintMaterialProperty.objectReferenceValue == null;
			EditorGUI.BeginDisabledGroup(!disabled);
			if (EditorGUILayout.BeginFadeGroup(Convert.ToSingle(disabled)))
			{
				if (GUILayout.Button(new GUIContent("Auto fill", PaintManagerHelper.AutoFillButtonTooltip), GUILayout.ExpandWidth(true)))
				{
					var objectForPaintingFillResult = FindObjectForPainting();
					var findMaterialResult = FindMaterial();
					if (!objectForPaintingFillResult && !findMaterialResult)
					{
						Debug.Log("Can't find ObjectForPainting and Material.");
					}
					else if (!objectForPaintingFillResult)
					{
						Debug.Log("Can't find ObjectForPainting.");
					}
					else if (!findMaterialResult)
					{
						Debug.Log("Can't find Material.");
					}
					else
					{
						MarkAsDirty();
					}
				}
			}
			EditorGUILayout.EndFadeGroup();
			EditorGUI.EndDisabledGroup();
		}

		private bool FindObjectForPainting()
		{
			if (_objectForPaintingProperty.objectReferenceValue == null)
			{
				var supportedComponent = PaintManagerHelper.GetSupportedComponent(_paintManager.gameObject);
				if (supportedComponent != null)
				{
					_objectForPaintingProperty.objectReferenceValue = supportedComponent.gameObject;
					return true;
				}
				if (_paintManager.gameObject.transform.childCount > 0)
				{
					var compatibleComponents = new List<Component>();
					var allComponents = _paintManager.gameObject.transform.GetComponentsInChildren<Component>();
					foreach (var component in allComponents)
					{
						var childComponent = PaintManagerHelper.GetSupportedComponent(component.gameObject);
						if (childComponent != null)
						{
							compatibleComponents.Add(childComponent);
							break;
						}
					}
					if (compatibleComponents.Count > 0)
					{
						_objectForPaintingProperty.objectReferenceValue = compatibleComponents[0].gameObject;
						return true;
					}
				}
				return false;
			}
			return true;
		}

		private bool FindMaterial()
		{
			var result = false;
			_component = PaintManagerHelper.GetSupportedComponent(_objectForPaintingProperty.objectReferenceValue as GameObject);
			if (_component != null)
			{
				var renderer = _component as Renderer;
				if (renderer != null && renderer.sharedMaterial != null)
				{
					_paintMaterialProperty.objectReferenceValue = renderer.sharedMaterial;
					result = true;
				}
				var maskableGraphic = _component as RawImage;
				if (maskableGraphic != null && maskableGraphic.material != null)
				{
					_paintMaterialProperty.objectReferenceValue = maskableGraphic.material;
					result = true;
				}
			}
			UpdateTexturesList();
			return result;
		}

		private void DrawMaterialBlock()
		{
			EditorGUI.BeginChangeCheck();
			EditorGUILayout.PropertyField(_paintMaterialProperty, new GUIContent("Material", PaintManagerHelper.MaterialTooltip));
			if (EditorGUI.EndChangeCheck())
			{
				UpdateTexturesList();
				_shouldCheckTexture = true;
			}
			if (EditorGUILayout.BeginFadeGroup(Convert.ToSingle(_paintMaterialProperty.objectReferenceValue != null)))
			{
				var shaderTextureNames = PaintManagerHelper.GetTexturesListFromShader(_paintMaterialProperty.objectReferenceValue as Material);
				var shaderTextureNamesContent = new GUIContent[shaderTextureNames.Length];
				for (var i = 0; i < shaderTextureNamesContent.Length; i++)
				{
					shaderTextureNamesContent[i] = new GUIContent(shaderTextureNames[i]);
				}

				var shaderTextureName = _paintManager.Material.ShaderTextureName;
				if (shaderTextureNames.Contains(shaderTextureName))
				{
					for (var i = 0; i < shaderTextureNames.Length; i++)
					{
						if (shaderTextureNames[i] == shaderTextureName)
						{
							_shaderTextureNameSelectedId = i;
							break;
						}
					}
				}
				
				_shaderTextureNameSelectedId = Mathf.Clamp(_shaderTextureNameSelectedId, 0, int.MaxValue);
				EditorGUI.BeginChangeCheck();
				_shaderTextureNameSelectedId = EditorGUILayout.Popup(new GUIContent("Shader Texture Name", PaintManagerHelper.ShaderTextureNameTooltip), _shaderTextureNameSelectedId, shaderTextureNamesContent);
				if (EditorGUI.EndChangeCheck())
				{
					_shouldCheckTexture = true;
				}

				if (shaderTextureNames.Length > 0 && shaderTextureNames.Length > _shaderTextureNameSelectedId && shaderTextureNames[_shaderTextureNameSelectedId] != shaderTextureName)
				{
					_shaderTextureNameProperty.stringValue = shaderTextureNames[_shaderTextureNameSelectedId];
					_paintManager.Material.ShaderTextureName = _shaderTextureNameProperty.stringValue;
					MarkAsDirty();
				}
				
				if (!_hasTexture || _shouldCheckTexture)
				{
					if (_shouldCheckTexture)
					{
						serializedObject.ApplyModifiedProperties();
						_hasTexture = PaintManagerHelper.HasTexture(_paintManager);
						_shouldCheckTexture = false;
					}
					if (!_hasTexture)
					{
						EditorGUILayout.HelpBox("Object does not have source texture, transparent texture will be created. Please specify the texture size", MessageType.Warning);
						EditorGUILayout.PropertyField(_defaultTextureWidth, new GUIContent("Texture Width", PaintManagerHelper.TextureSizeTip));
						EditorGUILayout.PropertyField(_defaultTextureHeight, new GUIContent("Texture Height", PaintManagerHelper.TextureSizeTip));
					}
				}
			}
			EditorGUILayout.EndFadeGroup();
		}

		private void DrawCheckboxesBlock()
		{
			EditorGUI.BeginChangeCheck();
			EditorGUILayout.PropertyField(_overrideCameraProperty, new GUIContent("Override Camera", PaintManagerHelper.OverrideCameraTooltip));
			if (EditorGUI.EndChangeCheck())
			{
				_paintManager.ShouldOverrideCamera = _overrideCameraProperty.boolValue;
			}
			using (var group = new EditorGUILayout.FadeGroupScope(Convert.ToSingle(_overrideCameraProperty.boolValue)))
			{
				if (group.visible)
				{
					EditorGUI.BeginChangeCheck();
					EditorGUILayout.PropertyField(_cameraProperty, new GUIContent("Camera", PaintManagerHelper.CameraTooltip));
					if (EditorGUI.EndChangeCheck())
					{
						_paintManager.Camera = _cameraProperty.objectReferenceValue as Camera;
					}
				}
			}
			
			EditorGUI.BeginChangeCheck();
			EditorGUILayout.PropertyField(_copySourceTextureToPaintTextureProperty, new GUIContent("Copy Source Texture To Paint Texture", PaintManagerHelper.CopySourceTextureToPaintTextureTooltip));
			if (EditorGUI.EndChangeCheck())
			{
				_paintManager.CopySourceTextureToPaintTexture = _copySourceTextureToPaintTextureProperty.boolValue;
				MarkAsDirty();
			}
			
			EditorGUI.BeginChangeCheck();
			EditorGUILayout.PropertyField(_useSourceTextureAsBackgroundProperty, new GUIContent("Use Source Texture as Background", PaintManagerHelper.UseSourceTextureAsBackgroundTooltip));
			if (EditorGUI.EndChangeCheck())
			{
				_paintManager.UseSourceTextureAsBackground = _useSourceTextureAsBackgroundProperty.boolValue;
				MarkAsDirty();
			}
			
			if (EditorGUILayout.BeginFadeGroup(Convert.ToSingle(_isMeshObject)))
			{
				if (!_useNeighborsVerticesForRaycastsProperty.boolValue || !_paintManager.UseNeighborsVerticesForRaycasts || _paintManager.UseNeighborsVerticesForRaycasts && !_paintManager.HasTrianglesData && _trianglesContainerProperty.objectReferenceValue == null)
				{
					EditorGUILayout.HelpBox("Object does not have neighbors triangles data, to fix it please check 'Use Neighbors Vertices For Raycasts'", MessageType.Warning);
				}
				
				EditorGUI.BeginChangeCheck();
				EditorGUILayout.PropertyField(_useNeighborsVerticesForRaycastsProperty, new GUIContent("Use Neighbors Vertices For Raycasts", PaintManagerHelper.UseNeighborsVerticesForRaycastTooltip));
				if (EditorGUI.EndChangeCheck())
				{
					_paintManager.UseNeighborsVerticesForRaycasts = _useNeighborsVerticesForRaycastsProperty.boolValue;
					if (_useNeighborsVerticesForRaycastsProperty.boolValue)
					{
						OpenTrianglesDataWindow(_paintManager);
					}
					MarkAsDirty();
				}

				EditorGUI.BeginChangeCheck();
				EditorGUILayout.PropertyField(_trianglesContainerProperty, new GUIContent("Triangles Container"));
				if (EditorGUI.EndChangeCheck())
				{
					if (_trianglesContainerProperty.objectReferenceValue != null)
					{
						_useNeighborsVerticesForRaycastsProperty.boolValue = true;
						serializedObject.ApplyModifiedProperties();
						_paintManager.ClearTrianglesData();
						serializedObject.Update();
					}
					else if (_trianglesContainerProperty.objectReferenceValue == null && !_paintManager.HasTrianglesData)
					{
						_useNeighborsVerticesForRaycastsProperty.boolValue = false;
						serializedObject.ApplyModifiedProperties();
					}
					MarkAsDirty();
				}
			}
			EditorGUILayout.EndFadeGroup();
		}

		private void DrawButtonsBlock()
		{
			if (EditorGUILayout.BeginFadeGroup(Convert.ToSingle(_isMeshObject)))
			{
				EditorGUI.BeginDisabledGroup(!_paintManager.HasTrianglesData);
				if (GUILayout.Button(new GUIContent("Save Triangles Data to Asset", PaintManagerHelper.CloneMaterialTooltip), GUILayout.ExpandWidth(true)))
				{
					var path = EditorUtility.SaveFilePanelInProject("Save Triangles Data to TrianglesContainer", "TrianglesContainer", "asset", "TrianglesContainer asset saving");
					if (!string.IsNullOrEmpty(path))
					{
						var asset = CreateInstance<TrianglesContainer>();
						AssetDatabase.CreateAsset(asset, path);
						asset.Data = _paintManager.GetTriangles();
						EditorUtility.SetDirty(asset);
						_paintManager.ClearTrianglesData();
						serializedObject.Update();
						_trianglesContainerProperty.objectReferenceValue = asset;
						MarkAsDirty();
					}
				}
				EditorGUI.EndDisabledGroup();
			}
			EditorGUILayout.EndFadeGroup();
						
			var disablePlaying = Application.isPlaying;
			EditorGUI.BeginDisabledGroup(!disablePlaying);
			if (GUILayout.Button(new GUIContent("Initialize", PaintManagerHelper.SaveToFileTooltip), GUILayout.ExpandWidth(true)))
			{
				_paintManager.Init();
			}
			EditorGUI.EndDisabledGroup();
			
			GUILayout.BeginHorizontal();
			EditorGUI.BeginDisabledGroup(disablePlaying);
			if (GUILayout.Button(new GUIContent("Clone Material", PaintManagerHelper.CloneMaterialTooltip), GUILayout.ExpandWidth(true)))
			{
				var clonedMaterial = PaintManagerHelper.CloneMaterial(_paintMaterialProperty.objectReferenceValue as Material);
				if (clonedMaterial != null)
				{
					_paintMaterialProperty.objectReferenceValue = clonedMaterial;
				}
			}
			if (GUILayout.Button(new GUIContent("Clone Texture", PaintManagerHelper.CloneTextureTooltip), GUILayout.ExpandWidth(true)))
			{
				PaintManagerHelper.CloneTexture(_paintMaterialProperty.objectReferenceValue as Material, _shaderTextureNameProperty.stringValue);
			}
			EditorGUI.EndDisabledGroup();
			GUILayout.EndHorizontal();
				
			GUILayout.BeginHorizontal();
			var disableUndo = !(Application.isPlaying && _paintObject != null && _paintObject.TextureKeeper.CanUndo());
			EditorGUI.BeginDisabledGroup(disableUndo);
			if (GUILayout.Button(new GUIContent("Undo", PaintManagerHelper.UndoTooltip), GUILayout.ExpandWidth(true)))
			{
				_paintObject.TextureKeeper.Undo();
				_paintManager.Render();
			}
			EditorGUI.EndDisabledGroup();
			var disableRedo = !(Application.isPlaying && _paintObject != null && _paintObject.TextureKeeper.CanRedo());
			EditorGUI.BeginDisabledGroup(disableRedo);
			if (GUILayout.Button(new GUIContent("Redo", PaintManagerHelper.RedoTooltip), GUILayout.ExpandWidth(true)))
			{
				_paintObject.TextureKeeper.Redo();
				_paintManager.Render();
			}
			EditorGUI.EndDisabledGroup();
			GUILayout.EndHorizontal();
			
			GUILayout.BeginHorizontal();
			var disableBake = !(Application.isPlaying && _isMeshObject);
			EditorGUI.BeginDisabledGroup(disableBake);
			if (GUILayout.Button(new GUIContent("Bake", PaintManagerHelper.BakeTooltip), GUILayout.ExpandWidth(true)))
			{
				PaintManagerHelper.Bake(_paintManager.Material.SourceTexture, _paintManager.Bake);
			}
			EditorGUI.EndDisabledGroup();
				
			EditorGUI.BeginDisabledGroup(!Application.isPlaying);
			if (GUILayout.Button(new GUIContent("Save", PaintManagerHelper.SaveToFileTooltip), GUILayout.ExpandWidth(true)))
			{
				PaintManagerHelper.SaveToFile(_paintManager);
			}
			EditorGUI.EndDisabledGroup();
			GUILayout.EndHorizontal();
		}

		private void MarkAsDirty()
		{
			if (!Application.isPlaying)
			{
				EditorUtility.SetDirty(_paintManager);
				EditorSceneManager.MarkSceneDirty(_paintManager.gameObject.scene);
			}
		}
	}
}