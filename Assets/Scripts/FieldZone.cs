using UnityEngine;

[System.Serializable]
public class FieldZone
{
    public Vector3 center;
    public float width = 20f;
    public float depth = 20f;

    public bool IsInside(Vector3 position)
    {
        return Mathf.Abs(position.x - center.x) <= width / 2 &&
               Mathf.Abs(position.z - center.z) <= depth / 2;
    }

    public Vector3 ClampToZone(Vector3 position)
    {
        float x = Mathf.Clamp(position.x, center.x - width / 2, center.x + width / 2);
        float z = Mathf.Clamp(position.z, center.z - depth / 2, center.z + depth / 2);
        return new Vector3(x, position.y, z);
    }
}
