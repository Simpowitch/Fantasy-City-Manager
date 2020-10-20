using UnityEngine;

public static class GridUtilities
{
    const int RADIUSTOBOX = 8;

    public static Vector3[] GetPositionsAround(Vector3 startPosition, float distanceFromCenter, int count)
    {
        Vector3[] offsets = new Vector3[count];

        for (int i = 0; i < count; i++)
        {
            float angle = i * (360f / count);
            Vector3 dir = ApplyRotationToVector(new Vector3(1, 0), angle);
            offsets[i] = startPosition + dir * distanceFromCenter;
        }
        return offsets;
    }

    private static Vector3 ApplyRotationToVector(Vector3 vec, float angle) => Quaternion.Euler(0, 0, angle) * vec;
}
