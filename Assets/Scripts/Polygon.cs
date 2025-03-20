using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Polygon : MonoBehaviour
{
    PolygonCollider2D polygon;
    Mesh mesh;
    private void Awake()
    {
        polygon = this.gameObject.GetComponent<PolygonCollider2D>();
    }

    public Mesh PolygonCreate(Vector2[] points)
    {
        
        polygon.SetPath(0, points);
        mesh = polygon.CreateMesh(true, false);
        return mesh;
    }
}
