using UnityEngine;

public class CameraSway : MonoBehaviour
{
    public float swayAmount = 0.1f;
    public float swaySpeed = 0.5f;
    public float rotationAmount = 1f;

    private Vector3 startPos;
    private Quaternion startRot;

    void Start()
    {
        startPos = transform.localPosition;
        startRot = transform.localRotation;
    }

    void Update()
    {
        float x = Mathf.PerlinNoise(Time.time * swaySpeed, 0f) - 0.5f;
        float y = Mathf.PerlinNoise(0f, Time.time * swaySpeed) - 0.5f;

        Vector3 offset = new Vector3(x, y, 0f) * swayAmount;
        transform.localPosition = startPos + offset;

        float rotX = y * rotationAmount;
        float rotZ = -x * rotationAmount;

        transform.localRotation = Quaternion.Euler(
            startRot.eulerAngles.x + rotX,
            startRot.eulerAngles.y,
            startRot.eulerAngles.z + rotZ
        );
    }
}