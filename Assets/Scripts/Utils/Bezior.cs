using UnityEngine;

public static class Bezior
{
    public static Vector2 BeziorMove(Vector2[] points, float lerpTime)
    {
        Vector2[] tempPoints = (Vector2[])points.Clone();

        for (int i = tempPoints.Length - 1; i > 0; i--)
        {
            for (int j = 0; j < i; j++)
            {
                tempPoints[j] = Vector2.Lerp(tempPoints[j], tempPoints[j + 1], lerpTime);
            }
        }

        return tempPoints[0];
    }
}