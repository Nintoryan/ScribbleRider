using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
 
namespace YTaxi.CustomUserInput
{
    public class HoldAndDrag : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler, IPointerClickHandler
    {
        [SerializeField] private UnityEvent _clicked;
        [SerializeField] private UnityEvent _started;
        [SerializeField] private UnityEvent _dragged;
        [SerializeField] private UnityEvent _stopped;
        
        private Vector2 _startPoint;
        private Vector2 _currentPoint;
 
        public event UnityAction Clicked
        {
            add => _clicked.AddListener(value);
            remove => _clicked.RemoveListener(value);
        }
        public event UnityAction Started
        {
            add => _started.AddListener(value);
            remove => _started.RemoveListener(value);
        }
        public event UnityAction Dragged
        {
            add => _dragged.AddListener(value);
            remove => _dragged.RemoveListener(value);
        }

        public event UnityAction<PointerEventData> DraggedData;
        public event UnityAction Stopped
        {
            add => _stopped.AddListener(value);
            remove => _stopped.RemoveListener(value);
        }
        public Vector2 StartPoint => _startPoint;
        public Vector2 CurrentPoint => _currentPoint;
        public Vector2 Delta { get; private set; }
 
        private bool IsDragged => _startPoint.magnitude > 0;
 
        public void OnPointerDown(PointerEventData eventData)
        {
            _startPoint = eventData.position;
            _currentPoint = eventData.position;
            _started?.Invoke();
        }
 
        public void OnDrag(PointerEventData eventData)
        {
            _currentPoint = eventData.position;
            Delta = eventData.delta;
            _dragged?.Invoke();
            DraggedData?.Invoke(eventData);
        }
 
        public void OnPointerUp(PointerEventData eventData)
        {
            _currentPoint = Vector2.zero;
            _startPoint = Vector2.zero;
 
            _stopped?.Invoke();
        }
 
        public void OnPointerClick(PointerEventData eventData)
        {
            if (IsDragged == false)
            {
                _clicked?.Invoke();
            }
        }
    }
}