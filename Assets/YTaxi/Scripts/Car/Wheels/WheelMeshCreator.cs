using System.Collections.Generic;
using UnityEngine;
using YTaxi.Scripts.Shop;

namespace YTaxi.Scripts.Car.Wheels
{
    public class WheelMeshCreator : MonoBehaviour
    {
        [SerializeField] private WheelPart wheelPart;
        [SerializeField] private Car _car;
        [SerializeField] private CarView _carView;
        
    
        private void Start()
        {
            wheelPart = Instantiate(wheelPart);
            wheelPart.transform.SetParent(null);
            wheelPart.SetMaterial(_carView.GetCurrentMaterial());
        }
    
        public void CreateWheel(List<Vector3> points)
        {
            if(points.Count<2) return;
            var NonlinnearCoef = GetNonlinnearCoef(points);
            var amountOfSharpAngles = GetKikiCoef(points);
            var parent = new GameObject();
            var Center = Vector3.zero;
            var wheelParts = new List<WheelPart>();
            var currentPoint = points[0];
            points.Remove(points[0]);
            var min = new Vector3(10000,10000,0);
            var max = new Vector3(-10000,-10000,0);
            foreach (var point in points)
            {
                if (point.x + point.y > max.x + max.y)
                {
                    max = point;
                }
    
                if (point.x + point.y < min.x + min.y)
                {
                    min = point;
                }
                var newPart = Instantiate(wheelPart);
    
                var transform1 = newPart.transform;
                
                transform1.position = (currentPoint + point) / 2f;
                
                var localScale = transform1.localScale;
                
                localScale = new Vector3(localScale.x, localScale.y,Vector3.Distance(currentPoint,point));
                
                newPart.transform.localScale = localScale;
                
                newPart.transform.LookAt(point,Vector3.down);
                var pair = Instantiate(newPart);
                pair.transform.position += new Vector3(0,0,440);
                wheelParts.Add(newPart);
                wheelParts.Add(pair);
    
                Center += newPart.transform.position;
                Center += pair.transform.position;
                currentPoint = point;
            }
            parent.transform.position = Center/wheelParts.Count;
            foreach (var cube in wheelParts)
            {
                cube.transform.SetParent(parent.transform);
            }
            var rb = parent.AddComponent<Rigidbody>();
            rb.interpolation = RigidbodyInterpolation.Extrapolate;
            var Distance = Vector3.Distance(min, max);
            
            var wheel = parent.AddComponent<Wheel>();
            
            wheel.Initialize(NonlinnearCoef,amountOfSharpAngles,Distance);
    
            _car.SetWheels(wheel);
        }
        private float GetNonlinnearCoef(List<Vector3> points)
        {
            float _sumDistance = 0;
            if (points.Count == 2)
            {
                _sumDistance = 0;
            }
            else
            {
                for (int i = 1; i < points.Count-1; i++)
                {
                    _sumDistance += DistancePointLine(points[i], 
                        points[0],
                        points[points.Count - 1]);
                }
                _sumDistance /= points.Count-2;
            }
    
            return _sumDistance;
        }
    
        private int GetKikiCoef(List<Vector3> points)
        {
            int amountOfSharpAngles = 0;
            for (int i = 1; i < points.Count-1; i++)
            {
                var cosY = GetCosOfMiddle(points[i - 1], points[i], points[i + 1]);
    
                if (cosY >= -0.5f)
                {
                    amountOfSharpAngles++;
                }
            }
            return amountOfSharpAngles;
        }
    
        private float GetCosOfMiddle(Vector3 p1, Vector3 p2, Vector3 p3)
        {
            var a = Vector3.Distance(p1, p2);
            var b = Vector3.Distance(p2, p3);
            var c = Vector3.Distance(p1, p3);
            return (a * a + b * b - c * c) / (2 * a * b);
        }
    
        private float DistancePointLine(Vector3 point, Vector3 lineStart, Vector3 lineEnd)
        {
            return Vector3.Magnitude(ProjectPointLine(point, lineStart, lineEnd) - point);
        }
    
        private static Vector3 ProjectPointLine(
            Vector3 point,
            Vector3 lineStart,
            Vector3 lineEnd)
        {
            var rhs = point - lineStart;
            var vector3 = lineEnd - lineStart;
            var magnitude = vector3.magnitude;
            var lhs = vector3;
            if (magnitude > 9.99999997475243E-07)
                lhs /= magnitude;
            var num = Mathf.Clamp(Vector3.Dot(lhs, rhs), 0.0f, magnitude);
            return lineStart + lhs * num;
        }
    }
}

