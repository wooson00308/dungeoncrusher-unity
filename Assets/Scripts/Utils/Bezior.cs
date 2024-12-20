using UnityEngine;

public static class Bezior
{
    public static Vector2 BeziorMove(Vector2[] points, float lerpTime)
    {
        for (int i = points.Length - 1; i >= 0; i--)
        {
            for (int j = 0; j < i; j++)
            {
                points[j] = Vector2.MoveTowards(points[j], points[j + 1], lerpTime * Time.deltaTime);
            }
        }

        return points[0];
    }
}