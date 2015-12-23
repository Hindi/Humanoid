using UnityEngine;
using System.Collections;

public static class MathUtils
{
    static float EPSILON = 0.0001f;
    public static bool isPointOnLine(Vector2 point, Vector2 A, Vector2 B)
    {
        return (Mathf.Abs(Vector2.Distance(A, point) + Vector2.Distance(B, point) - Vector2.Distance(A, B)) < EPSILON);
    }

    public static float distancePointSegment(Vector2 point, Vector2 A, Vector2 B)
    {
        float a = point.x - A.x;
        float b = point.y - A.y;
        float c = B.x - A.x;
        float d = B.y - A.y;

        float dot = a * c + b * d;
        float len_sq = c * c + d * d;
        float param = -1;
        if (len_sq != 0) //in case of 0 length line
            param = dot / len_sq;

        float xx, yy;

        if (param < 0)
        {
            xx = A.x;
            yy = A.y;
        }
        else if (param > 1)
        {
            xx = B.x;
            yy = B.y;
        }
        else
        {
            xx = A.x + param * c;
            yy = A.y + param * d;
        }

        var dx = point.x - xx;
        var dy = point.y - yy;
        return Mathf.Sqrt(dx * dx + dy * dy);
    }

    public static Vector2 orthogonalProjection(Vector2 point, Vector2 A, Vector2 B)
    {
        float x1 = A.x, y1 = A.y, x2 = B.x, y2 = B.y, x3 = point.x, y3 = point.y;
        float px = x2 - x1, py = y2 - y1, dAB = px * px + py * py;
        float u = ((x3 - x1) * px + (y3 - y1) * py) / dAB;
        float x = x1 + u * px, y = y1 + u * py;
        return new Vector2(x, y);
    }

    public static float triangleOrientation(Vector2 a, Vector2 b, Vector2 c)
    {
        float ax = b.x - a.x;
        float ay = b.y - a.y;
        float bx = c.x - a.x;
        float by = c.y - a.y;
        return bx * ay - ax * by;
    }

    // using barycentric coordinates
    public static bool IsPointInClockwiseTriangle(Vector2 p, Vector2 A, Vector2 B, Vector2 C)
    {
        var s = (A.y * C.x - A.x * C.y + (C.y - A.y) * p.x + (A.x - C.x) * p.y);
        var t = (A.x * B.y - A.y * B.x + (A.y - B.y) * p.x + (B.x - A.x) * p.y);

        if (s <= 0 || t <= 0)
            return false;

        var Area = (-B.y * C.x + A.y * (-B.x + C.x) + A.x * (B.y - C.y) + B.x * C.y);

        return (s + t) < Area;
    }

    public static int getTripletOrientation(Vector2 p, Vector2 q, Vector2 r)
    {
        // See http://www.geeksforgeeks.org/orientation-3-ordered-points/
        // for details of below formula.
        int val = (int)((q.y - p.y) * (r.x - q.x) - (q.x - p.x) * (r.y - q.y));

        if (val == 0) return 0;  // colinear

        return (val > 0) ? 1 : 2; // clock or counterclock wise
    }

    public static bool doSegmentIntersect(Vector2 Ax, Vector2 Ay, Vector2 Bx, Vector2 By)
    {
        // Find the four orientations needed for general and special cases
        int o1 = getTripletOrientation(Ax, Ay, Bx);
        int o2 = getTripletOrientation(Ax, Ay, By);
        int o3 = getTripletOrientation(Bx, By, Ax);
        int o4 = getTripletOrientation(Bx, By, Ay);

        // General case
        if (o1 != o2 && o3 != o4)
            return true;

        // Special cases with colinearity
        if (o1 == 0 && isPointOnLine(Bx, Ax, Ay)) return true;
        if (o2 == 0 && isPointOnLine(By, Ax, Ay)) return true;
        if (o3 == 0 && isPointOnLine(Ax, Bx, By)) return true;
        if (o4 == 0 && isPointOnLine(Ay, Bx, By)) return true;

        return false; // Doesn't fall in any of the above cases
    }
}