using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UILineRenderer : Graphic
{
    public Vector2 gridSize;
    public List<Vector2> points;

    float unitWidth;
    float unitHeight;
    private Vector2 unitV;
    public Vector2 ChartOffset;
  
    public float thickness = 1f;
    //private float u_thickness;
    
    public bool Show = false;

    public void RefreshChart()
    {
        SetAllDirty();
    }

    protected override void OnPopulateMesh(VertexHelper vh)
    {
        vh.Clear();

        //OffsetV = Vector2.zero; // (-width / 2, height);
        if (Show)
        {
            unitWidth = rectTransform.rect.width / gridSize.x;
            unitHeight = rectTransform.rect.height / gridSize.y;
            unitV = new Vector2(unitWidth, unitHeight);
           // u_thickness = unitHeight * thickness / 2f;

            if (points.Count < 2)
                return;

            int ind = 0;
            for (int i = 0; i < points.Count - 1; i++)
            {
                if (points[i].x >=ChartOffset.x)
                    DrawLine(points[i], points[i + 1], vh, ref ind);
            }  
        }
    }

    void DrawLine(Vector2 point0, Vector2 point1, VertexHelper vh, ref int index0)
    {
        UIVertex vertex = UIVertex.simpleVert;
        vertex.color = color;

        point0 = (point0-ChartOffset)*unitV;
        point1 = (point1-ChartOffset)*unitV;

        Vector2 d = point1 - point0;
        Vector3 shiftV;

        if (d.magnitude < thickness)
        {
            point1 = point0 + new Vector2(0, thickness);
            shiftV = new Vector2(thickness, 0);
        }
        else
            shiftV = Rotate90CW(point1 - point0).normalized * thickness;


        vertex.position = point0;
        vh.AddVert(vertex);
        vertex.position += shiftV;
        vh.AddVert(vertex);

        vertex.position = point1;
        vh.AddVert(vertex);
        vertex.position += shiftV;
        vh.AddVert(vertex);

        vh.AddTriangle(index0 + 0, index0 + 2, index0 + 3);
        vh.AddTriangle(index0 + 1, index0 + 0, index0 + 3);
        index0 += 4;
    }

    Vector3 Rotate90CW(Vector3 aDir)
    {
        return new Vector3(aDir.y, -aDir.x);
    }

}
