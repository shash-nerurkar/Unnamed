using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CircleRenderer : MonoBehaviour
{
    public LineRenderer line;

    public void DrawCircle(float radius, Color color, float width) {
        var segments = 360;
        line.startWidth = width;
        line.endWidth = width;

        Gradient gradient = new();

        GradientColorKey[] colorKey = new GradientColorKey[1];
        colorKey[0].color = color;
        colorKey[0].time = 0.0f;
        GradientAlphaKey[] alphaKey = new GradientAlphaKey[1];
        alphaKey[0].alpha = 1.0f;
        alphaKey[0].time = 0.0f;
        gradient.SetKeys(colorKey, alphaKey);
        line.colorGradient = gradient;

        line.positionCount = segments + 1;

        // add extra point to make startpoint and endpoint the same to close the circle
        var pointCount = segments + 1; 
        
        var points = new Vector3[pointCount];
        for (int i = 0; i < pointCount; i++)
        {
            var rad = Mathf.Deg2Rad * (i * 360f / segments);
            points[i] = new Vector3(Mathf.Sin(rad) * radius, Mathf.Cos(rad) * radius, 0);
        }

        line.SetPositions(points);
    }
}
