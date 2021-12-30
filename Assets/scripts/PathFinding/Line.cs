using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct Line
{
    private const float verticalLineGradient = 1e5f;
    
    private float graidient;
    private float y_intercept;
    private Vector2 pointOnLine_1;
    private Vector2 pointOnLine_2;

    private float graidientPerpendicular;

    private bool approachSide;

    public Line(Vector2 pointOnLine, Vector2 pointPerpendicularOnLine)
    {
        float dx = pointOnLine.x - pointPerpendicularOnLine.x;
        float dy = pointOnLine.y - pointPerpendicularOnLine.y;

        if (dy == 0) {
            graidientPerpendicular = verticalLineGradient;
        }else {
            graidientPerpendicular = dy / dx;
        }

        if (graidientPerpendicular == 0) {
            graidient = verticalLineGradient;
        }else {
            graidient = -1 / graidientPerpendicular;    
        }

        y_intercept = pointOnLine.y - graidient * pointOnLine.x;

        pointOnLine_1 = pointOnLine;
        pointOnLine_2 = pointOnLine_1 + new Vector2(1, graidient);
        approachSide = false;
        approachSide = GetSide(pointPerpendicularOnLine);
    }

    bool GetSide(Vector2 p)
    {
        return (p.x - pointOnLine_1.x) * (pointOnLine_2.y - pointOnLine_1.y) >
               (p.y - pointOnLine_1.y) * (pointOnLine_2.x - pointOnLine_1.x);
    }

    public bool HasCrossedLine(Vector2 p)
    {
        return GetSide(p) != approachSide;
    }

    public void DrawWithGizmos(float length)
    {
        Vector3 LineDir = new Vector3(1, 0, graidient).normalized;
        Vector3 lineCenter = new Vector3(pointOnLine_1.x, 0, pointOnLine_1.y) + Vector3.up;
        Gizmos.DrawLine(lineCenter - LineDir * length / 2f, lineCenter + LineDir * length/ 2f);
    }
}
