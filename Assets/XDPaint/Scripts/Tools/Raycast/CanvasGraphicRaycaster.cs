using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;

namespace XDPaint.Tools.Raycast
{
    public class CanvasGraphicRaycaster : MonoBehaviour
    {
        private GraphicRaycaster _raycaster;
        private EventSystem _eventSystem;
        private PointerEventData _pointerEventData;

        void Start()
        {
            _raycaster = GetComponent<GraphicRaycaster>();
            _eventSystem = GetComponent<EventSystem>();
        }

        public List<RaycastResult> GetRaycasts(Vector2 position)
        {
            if (_raycaster == null)
                return null;
            _pointerEventData = new PointerEventData(_eventSystem) {position = position};
            var results = new List<RaycastResult>();
            _raycaster.Raycast(_pointerEventData, results);
            return results;
        }
    }
}