using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI.Extensions;
using YTaxi.CustomUserInput;

namespace YTaxi.Drawing
{
    public class Drawing : MonoBehaviour
    {
        [SerializeField] private float newPointDistance;
        [SerializeField] private RectTransform _field;
        [SerializeField] private float defaultLineThickness = 20f;
        [SerializeField] private UILineRenderer _uiLineRenderer;
        [SerializeField] private HoldAndDrag _holdAndDrag;
        [SerializeField] private List<Vector3> brokenLinePoints = new List<Vector3>();
        [SerializeField] private List<Vector2> uiLinePoints = new List<Vector2>();
        [SerializeField] private WheelMeshCreator _wheelMeshCreator;
        
        [SerializeField] private RectTransform m_CanvasTransform;
        private Rect CheckingRect;
    
        private void Start()
        {
            newPointDistance = 20f / (900f / Screen.width);
            _uiLineRenderer.lineThickness = defaultLineThickness / (900f / Screen.width);
            CheckingRect = _field.rect;
            CheckingRect.center = new Vector2(Screen.width * 0.5f, Screen.height * _field.anchorMax.y *0.5f);

            _holdAndDrag.Started += () =>
            {
                brokenLinePoints.Add(_holdAndDrag.CurrentPoint);
                uiLinePoints.Add(_holdAndDrag.CurrentPoint);
                _uiLineRenderer.Points = uiLinePoints.ToArray();
            };
            _holdAndDrag.Dragged += () =>
            {
                if (Vector3.Distance(_holdAndDrag.CurrentPoint, brokenLinePoints[brokenLinePoints.Count-1]) > newPointDistance)
                {
                    if (!CheckingRect.Contains(_holdAndDrag.CurrentPoint)) return;
                    brokenLinePoints.Add(_holdAndDrag.CurrentPoint);
                    uiLinePoints.Add(_holdAndDrag.CurrentPoint);
                    _uiLineRenderer.Points = uiLinePoints.ToArray();
                }
            };
            _holdAndDrag.Stopped += () =>
            {
                ResizePoints();
                _wheelMeshCreator.CreateWheel(brokenLinePoints);
                brokenLinePoints.Clear();
                uiLinePoints.Clear();
                _uiLineRenderer.Points = new Vector2[0];
                _uiLineRenderer.gameObject.SetActive(false);
                _uiLineRenderer.gameObject.SetActive(true);
            };
        }
        #if UNITY_EDITOR
        private void OnValidate()
        {
            if (_wheelMeshCreator == null)
            {
                _wheelMeshCreator = FindObjectOfType<WheelMeshCreator>();
            }
        }
#endif

        private void ResizePoints()
        {
            var resizingCoef = Screen.width;
            for (int i = 0; i < brokenLinePoints.Count;i++)
            {
                brokenLinePoints[i] *= 500f / resizingCoef;
            }
        }
        private void OnDrawGizmos()
        {
            Gizmos.matrix = _field.localToWorldMatrix;
            var arrayOfPoints = brokenLinePoints.ToArray();
            for (int i = 1; i < brokenLinePoints.Count; i++)
            {
                Gizmos.DrawLine(arrayOfPoints[i-1], arrayOfPoints[i]);
            }
        }
    }
}

