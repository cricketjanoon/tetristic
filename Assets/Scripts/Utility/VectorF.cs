using UnityEngine;

public static class VectorF
{
    public static Vector2 Round(Vector2 vect2)
    {
        return new Vector2(Mathf.Round(vect2.x), Mathf.Round(vect2.y));
    }

    public static Vector3 Round(Vector3 vect3)
    {
        return new Vector3(Mathf.Round(vect3.x), Mathf.Round(vect3.y), Mathf.Round(vect3.z));
    }
}