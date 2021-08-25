using System.Collections.Generic;
using CustomUserInput;
using UnityEngine;
using XDPaint;

public class Drawing : MonoBehaviour
{
    [SerializeField] private float newPointDistance;
    [SerializeField] private float brokenLineSimplifyingDistance;
    [SerializeField] private HoldAndDrag _holdAndDrag;
    [SerializeField] private PaintManager _paintManager;
    [SerializeField] private List<Vector3> brokenLinePoints = new List<Vector3>();
    [SerializeField] private WheelMeshCreator _wheelMeshCreator;
    
    [SerializeField] private RectTransform m_CanvasTransform;

    private void Start()
    {
        _holdAndDrag.Started += () =>
        {
            brokenLinePoints.Add(_holdAndDrag.CurrentPoint);
        };
        _holdAndDrag.Dragged += () =>
        {
            if (Vector3.Distance(_holdAndDrag.CurrentPoint, brokenLinePoints[brokenLinePoints.Count-1]) > newPointDistance)
            {
                brokenLinePoints.Add(_holdAndDrag.CurrentPoint);
            }
        };
        _holdAndDrag.Stopped += () =>
        {
            Debug.Log($"Amount of Points:{brokenLinePoints.Count}");
            _wheelMeshCreator.CreateWheel(brokenLinePoints);
            brokenLinePoints.Clear();
            _paintManager.PaintObject.ClearTexture();
            _paintManager.Render();
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
