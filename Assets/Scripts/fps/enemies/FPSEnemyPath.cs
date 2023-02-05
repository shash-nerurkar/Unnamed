using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class FPSEnemyPath : MonoBehaviour
{
    public List<Transform> waypoints;
    [SerializeField] private bool alwaysDrawPath;
    [SerializeField] private bool drawAsLoop;
    [SerializeField] private bool drawNumbers;
    public Color debugColour = Color.white;

#if UNITY_EDITOR
    public void OnDrawGizmos()
    {
        if (alwaysDrawPath)
        {
            DrawPath();
        }
    }
    public void DrawPath()
    {
        for (int i = 0; i < waypoints.Count; i++)
        {
            GUIStyle labelStyle = new()
            {
                fontSize = 30,
                normal = new() {
                    textColor = debugColour,
                }
            };

            if (drawNumbers) Handles.Label(waypoints[i].position, i.ToString(), labelStyle);
            
            if (i >= 1)
            {
                Gizmos.color = debugColour;
                Gizmos.DrawLine(waypoints[i - 1].position, waypoints[i].position);

                if (drawAsLoop)
                    Gizmos.DrawLine(waypoints[^1].position, waypoints[0].position);

            }
        }
    }
    public void OnDrawGizmosSelected()
    {
        if (alwaysDrawPath)
            return;
        else
            DrawPath();
    }
#endif
}
