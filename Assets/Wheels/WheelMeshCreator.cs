using System.Collections.Generic;
using UnityEngine;

public class WheelMeshCreator : MonoBehaviour
{
    [SerializeField] private GameObject Cube;
    [SerializeField] private Car _car;
    

    public void CreateWheel(List<Vector3> points)
    {
        if(points.Count<2) return;
        var parent = new GameObject();
        var Center = Vector3.zero;
        var Cubes = new List<GameObject>();
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
            var newCube = Instantiate(Cube);
            newCube.transform.position = (currentPoint + point) / 2f;
            newCube.transform.localScale= new Vector3(newCube.transform.localScale.x,
                newCube.transform.localScale.y,
                Vector3.Distance(currentPoint,point));
            newCube.transform.LookAt(point);
            Cubes.Add(newCube);
            Center += newCube.transform.position;
            currentPoint = point;
        }

        parent.transform.position = Center/Cubes.Count;
        foreach (var cube in Cubes)
        {
            cube.transform.SetParent(parent.transform);
        }

        parent.AddComponent<Rigidbody>();
        _car.SetWheels(parent,Vector3.Distance(min,max));
    }
}
