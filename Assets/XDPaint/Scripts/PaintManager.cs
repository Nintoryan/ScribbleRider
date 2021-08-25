using UnityEngine;
using XDPaint.Controllers;
using XDPaint.Controllers.InputData;
using XDPaint.Controllers.InputData.Base;
using XDPaint.Core;
using XDPaint.Core.Materials;
using XDPaint.Core.PaintObject;
using XDPaint.Core.PaintObject.Base;
using XDPaint.Tools;
using XDPaint.Tools.Raycast;

namespace XDPaint
{
    [DisallowMultipleComponent]
    public class PaintManager : MonoBehaviour
    {
        #region Properties and variables
        public GameObject ObjectForPainting;
        public Paint Material = new Paint();
        public bool ShouldOverrideCamera;
        public BasePaintObject PaintObject { get; private set; }

        [SerializeField] private Camera overrideCamera;
        public Camera Camera
        {
            private get { return ShouldOverrideCamera || overrideCamera == null ? Camera.main : overrideCamera; }
            set
            {
                overrideCamera = value;
                if (InputController.Instance != null)
                {
                    InputController.Instance.Camera = overrideCamera;
                }
                if (RaycastController.Instance != null)
                {
                    RaycastController.Instance.Camera = overrideCamera;
                }
                if (_initialized)
                {
                    PaintObject.Camera = overrideCamera;
                }
            }
        }
        
        public bool CopySourceTextureToPaintTexture = true;

        [SerializeField] private bool useSourceTextureAsBackground = true;
        public bool UseSourceTextureAsBackground
        {
            get { return useSourceTextureAsBackground; }
            set
            {
                useSourceTextureAsBackground = value;
                if (!Application.isPlaying)
                {
                    if (!useSourceTextureAsBackground)
                    {
                        ClearTrianglesNeighborsData();
                    }
                }
                if (_initialized)
                {
                    PaintObject.UseSourceTextureAsBackground = useSourceTextureAsBackground;
                }
            }
        }

        [SerializeField] private bool useNeighborsVerticesForRaycasts;
        public bool UseNeighborsVerticesForRaycasts
        {
            get { return useNeighborsVerticesForRaycasts; }
            set
            {
                useNeighborsVerticesForRaycasts = value;
                if (!Application.isPlaying)
                {
                    if (!useNeighborsVerticesForRaycasts)
                    {
                        ClearTrianglesNeighborsData();
                    }
                }
                if (_initialized)
                {
                    PaintObject.UseNeighborsVertices = useNeighborsVerticesForRaycasts;
                }
            }
        }

        public bool HasTrianglesData
        {
            get { return triangles != null && triangles.Length > 0; }
        }

        public bool Initialized
        {
            get { return _initialized; }
        }

        [SerializeField] private TrianglesContainer trianglesContainer;
        [SerializeField] private Triangle[] triangles;
        private CanvasGraphicRaycaster _canvasGraphicRaycaster;
        private IRenderTextureHelper _renderTextureHelper;
        private IRenderComponentsHelper _renderComponentsHelper;
        private InputDataBase _input;
        private bool _initialized;
        #endregion

        private void Start()
        {
            if (!_initialized)
            {
                Init();
            }
        }

        private void Update()
        {
            if (_initialized && (PaintObject.IsPainting || PaintController.Instance.Preview))
            {
                Render();
            }
        }
        
        private void OnDestroy()
        {
            Destroy();
        }

        public void Init()
        {
            if (ObjectForPainting == null)
            {
                Debug.LogError("ObjectForPainting is null!");
                return;
            }

            RestoreSourceMaterialTexture();

            if (_renderComponentsHelper == null)
            {
                _renderComponentsHelper = new RenderComponentsHelper();
            }
            ObjectComponentType componentType;
            _renderComponentsHelper.Init(ObjectForPainting, out componentType);
            if (componentType == ObjectComponentType.Unknown)
            {
                Debug.LogError("Unknown component type!");
                return;
            }

            if (ControllersContainer.Instance == null)
            {
                var containerGameObject = new GameObject(Settings.Instance.ContainerGameObjectName);
                containerGameObject.AddComponent<ControllersContainer>();
            }

            if (_renderComponentsHelper.IsMesh())
            {
                var paintComponent = _renderComponentsHelper.PaintComponent;
                var renderComponent = _renderComponentsHelper.RendererComponent;
                var mesh = _renderComponentsHelper.GetMesh();
                if (trianglesContainer != null)
                {
                    triangles = trianglesContainer.Data;
                }
                if (triangles == null || triangles.Length == 0)
                {
                    if (mesh != null)
                    {
                        Debug.LogWarning("PaintManager does not have triangles data! Getting it may take a while.");
                        triangles = TrianglesData.GetData(mesh, useNeighborsVerticesForRaycasts);
                    }
                    else
                    {
                        Debug.LogError("Mesh is null!");
                        return;
                    }
                }
                RaycastController.Instance.InitObject(Camera, paintComponent, renderComponent, triangles);
            }
            Material.Init(_renderComponentsHelper);
            InitRenderTexture();
            InitPaintObject();
            InputController.Instance.Camera = Camera;
            PaintController.Instance.Init(this);
            SubscribeInputEvents(componentType);
            Render();
            _initialized = true;
        }

        public void Destroy()
        {
            if (_initialized)
            {
                //restore source material and texture
                RestoreSourceMaterialTexture();
                //destroy created material
                Material.Destroy();
                //free RenderTextures
                _renderTextureHelper.ReleaseTextures();
                //destroy raycast data
                if (_renderComponentsHelper.IsMesh())
                {
                    var renderComponent = _renderComponentsHelper.RendererComponent;
                    RaycastController.Instance.DestroyMeshData(renderComponent);
                }
                //unsubscribe input events
                UnsubscribeInputEvents();
                _input.OnDestroy();
                //free undo/redo RenderTextures and meshes
                PaintObject.Destroy();
                _initialized = false;
            }
        }
        
        public void Render()
        {
            if (_initialized)
            {
                PaintObject.OnRender();
                PaintObject.RenderCombined();
            }
        }
        
        public void FillTrianglesData(bool fillNeighbors = true)
        {
            if (_renderComponentsHelper == null)
            {
                _renderComponentsHelper = new RenderComponentsHelper();
            }
            ObjectComponentType componentType;
            _renderComponentsHelper.Init(ObjectForPainting, out componentType);
            if (componentType == ObjectComponentType.Unknown)
            {
                return;
            }
            if (_renderComponentsHelper.IsMesh())
            {
                var mesh = _renderComponentsHelper.GetMesh();
                if (mesh != null)
                {
                    triangles = TrianglesData.GetData(mesh, fillNeighbors);
                    if (fillNeighbors)
                    {
                        Debug.Log("Added triangles with neighbors data. Triangles count: " + triangles.Length);
                    }
                    else
                    {
                        Debug.Log("Added triangles data. Triangles count: " + triangles.Length);
                    }
                }
                else
                {
                    Debug.LogError("Mesh is null!");
                }
            }
        }
        
        public void ClearTrianglesData()
        {
            triangles = null;
        }

        public void ClearTrianglesNeighborsData()
        {
            if (triangles != null)
            {
                foreach (var triangle in triangles)
                {
                    triangle.N.Clear();
                }
            }
        }
                
        public Triangle[] GetTriangles()
        {
            return triangles;
        }

        public void SetTriangles(Triangle[] trianglesData)
        {
            triangles = trianglesData;
        }

        public void SetTriangles(TrianglesContainer trianglesContainerData)
        {
            trianglesContainer = trianglesContainerData;
            triangles = trianglesContainer.Data;
        }

        public RenderTexture GetRenderTextureLine()
        {
            return _renderTextureHelper.PaintLine;
        }

        public RenderTexture GetPaintTexture()
        {
            return _renderTextureHelper.PaintTexture;
        }

        public RenderTexture GetResultRenderTexture()
        {
            if (!_initialized)
                return null;
            return _renderTextureHelper.CombinedTexture;
        }

        /// <summary>
        /// Returns result texture
        /// </summary>
        /// <returns></returns>
        public Texture2D GetResultTexture()
        {
            var material = Material.Material;
            var sourceTexture = material.mainTexture;
            var previousRenderTexture = RenderTexture.active;
            var texture2D = new Texture2D(sourceTexture.width, sourceTexture.height, TextureFormat.ARGB32, false);
            RenderTexture.active = GetResultRenderTexture();
            texture2D.ReadPixels(new Rect(0, 0, texture2D.width, texture2D.height), 0, 0, false);
            texture2D.Apply();
            RenderTexture.active = previousRenderTexture;
            return texture2D;
        }
        
        /// <summary>
        /// Bakes into Material source texture
        /// </summary>
        public void Bake()
        {
            PaintObject.FinishPainting();
            Render();
            var prevRenderTexture = RenderTexture.active;
            var renderTexture = GetResultRenderTexture();
            RenderTexture.active = renderTexture;
            if (Material.SourceTexture != null)
            {
                var texture = Material.SourceTexture as Texture2D;
                if (texture != null)
                {
                    texture.ReadPixels(new Rect(0, 0, texture.width, texture.height), 0, 0);
                    texture.Apply();
                }
            }
            RenderTexture.active = prevRenderTexture;
        }

        /// <summary>
        /// Restore source material and texture
        /// </summary>
        private void RestoreSourceMaterialTexture()
        {
            if (_initialized && Material.SourceMaterial != null)
            {
                if (Material.SourceMaterial.GetTexture(Material.ShaderTextureName) == null)
                {
                    Material.SourceMaterial.SetTexture(Material.ShaderTextureName, Material.SourceTexture);
                }
                _renderComponentsHelper.SetSourceMaterial(Material.SourceMaterial, Material.Index);
            }
        }

        private void InitPaintObject()
        {
            if (PaintObject != null)
            {
                UnsubscribeInputEvents();
                PaintObject.Destroy();
            }
            if (_renderComponentsHelper.ComponentType == ObjectComponentType.RawImage)
            {
                PaintObject = new CanvasRendererPaint();
            }
            else if (_renderComponentsHelper.ComponentType == ObjectComponentType.SpriteRenderer)
            {
                PaintObject = new SpriteRendererPaint();
            }
            else
            {
                PaintObject = new MeshRendererPaint();
            }
            PaintObject.Init(Camera, ObjectForPainting.transform, Material, _renderTextureHelper, CopySourceTextureToPaintTexture);
            PaintObject.UseNeighborsVertices = useNeighborsVerticesForRaycasts;
            PaintObject.UseSourceTextureAsBackground = useSourceTextureAsBackground;
        }

        private void InitRenderTexture()
        {
            if (_renderTextureHelper == null)
            {
                _renderTextureHelper = new RenderTextureHelper();
            }
            _renderTextureHelper.Init(Material.SourceTexture.width, Material.SourceTexture.height);
            if (Material.SourceTexture != null)
            {
                Graphics.Blit(Material.SourceTexture, _renderTextureHelper.CombinedTexture);
            }
            Material.SetObjectMaterialTexture(_renderTextureHelper.CombinedTexture);
            Material.SetPaintTexture(_renderTextureHelper.PaintTexture);
        }

        #region Setup input events
        private void SubscribeInputEvents(ObjectComponentType componentType)
        {
            if (_input != null)
            {
                UnsubscribeInputEvents();
                _input.OnDestroy();
            }
            _input = new InputDataResolver().Resolve(componentType);
            _input.Init(this, Camera);
            UpdatePreviewInput();
            InputController.Instance.OnUpdate -= _input.OnUpdate;
            InputController.Instance.OnUpdate += _input.OnUpdate;
            InputController.Instance.OnMouseDown -= _input.OnDown;
            InputController.Instance.OnMouseDown += _input.OnDown;
            InputController.Instance.OnMouseButton -= _input.OnPress;
            InputController.Instance.OnMouseButton += _input.OnPress;
            InputController.Instance.OnMouseUp -= _input.OnUp;
            InputController.Instance.OnMouseUp += _input.OnUp;
        }

        private void UnsubscribeInputEvents()
        {
            InputController.Instance.OnUpdate -= _input.OnUpdate;
            InputController.Instance.OnMouseHover -= _input.OnHover;
            InputController.Instance.OnMouseDown -= _input.OnDown;
            InputController.Instance.OnMouseButton -= _input.OnPress;
            InputController.Instance.OnMouseUp -= _input.OnUp;
        }

        public void UpdatePreviewInput()
        {
            if (PaintController.Instance.Preview)
            {
                InputController.Instance.OnMouseHover -= _input.OnHover;
                InputController.Instance.OnMouseHover += _input.OnHover;
            }
            else
            {
                InputController.Instance.OnMouseHover -= _input.OnHover;
            }
        }

        #endregion
    }
}