using System.Collections;
using System.Collections.Generic;
using TMPro.EditorUtilities;
using UnityEngine;

public class Path
{
    public readonly Vector3[] lookPoints;
    public readonly Line[] turnBoundries;
    public readonly int finishLineIndex;

    public Path(Vector3[] waypoints, Vector3 startPos, float turnDst)
    {
        lookPoints = waypoints;
        turnBoundries = new Line[lookPoints.Length];
        finishLineIndex = turnBoundries.Length - 1;

        Vector2 PreviousPoint = V3Tov2(startPos);

        for (int i = 0; i < lookPoints.Length; i++)
        {
            Vector2 currentPoint = V3Tov2(lookPoints[i]);
            Vector2 dirToCurrentPoint = (currentPoint - PreviousPoint).normalized;
            Vector2 turnBoundaryPoint = (i == finishLineIndex) ? currentPoint : currentPoint - dirToCurrentPoint * turnDst;
            turnBoundries[i] = new Line(turnBoundaryPoint, PreviousPoint - dirToCurrentPoint * turnDst);
            PreviousPoint = turnBoundaryPoint;
        }
    }

    Vector2 V3Tov2(Vector3 v3)
    {
        return new Vector2(v3.x, v3.z);
    }

    public void DrawWithGizmos()
    {
        Gizmos.color = Color.black;
        foreach (Vector3 p in lookPoints)
        {
            Gizmos.DrawCube(p + Vector3.up, Vector3.one);
        }
        
        Gizmos.color = Color.white;
        foreach (Line l in turnBoundries)
        {
            l.DrawWithGizmos(10);
        }
    }
}
