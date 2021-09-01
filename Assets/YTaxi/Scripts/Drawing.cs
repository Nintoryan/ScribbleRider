using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI.Extensions;
using YTaxi.CustomUserInput;

namespace YTaxi.Drawing
{
    public class Drawing : MonoBehaviour
    {
        [SerializeField] private float newPointDistance;
        [SerializeField] private UILineRenderer _uiLineRenderer;
        [SerializeField] private HoldAndDrag _holdAndDrag;
        [SerializeField] private List<Vector3> brokenLinePoints = new List<Vector3>();
        [SerializeField] private List<Vector2> uiLinePoints = new List<Vector2>();
        [SerializeField] private WheelMeshCreator _wheelMeshCreator;
        
        [SerializeField] private RectTransform m_CanvasTransform;
    
        private void Start()
        {
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
                    brokenLinePoints.Add(_holdAndDrag.CurrentPoint);
                    uiLinePoints.Add(_holdAndDrag.CurrentPoint);
                    _uiLineRenderer.Points = uiLinePoints.ToArray();
                }
            };
            _holdAndDrag.Stopped += () =>
            {
                _wheelMeshCreator.CreateWheel(brokenLinePoints);
                brokenLinePoints.Clear();
                uiLinePoints.Clear();
                _uiLineRenderer.Points = new Vector2[0];
                _uiLineRenderer.gameObject.SetActive(false);
                _uiLineRenderer.gameObject.SetActive(true);
            };
        }
    
        private void OnDrawGizmos()
        {
            Gizmos.matrix = m_CanvasTransform.localToWorldMatrix;
            var arrayOfPoints = brokenLinePoints.ToArray();
            for (int i = 1; i < brokenLinePoints.Count; i++)
            {
                Gizmos.DrawLine(arrayOfPoints[i-1], arrayOfPoints[i]);
            }
        }
    }
}

