using UnityEngine;

public class Utilities : MonoBehaviour
{

    public static void RotateSprite(Transform toTransform, Vector3 target, float angleOffset)
    {
        Vector3 direction = target - toTransform.position;
        direction.Normalize();
        float angle = Mathf.Atan2(direction.z, direction.x) * Mathf.Rad2Deg + angleOffset;
        toTransform.localRotation = Quaternion.Euler(0, 0, angle);
    }

}