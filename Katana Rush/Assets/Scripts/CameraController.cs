using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform target;

    [Header("Offset")]
    public Vector3 offset = new Vector3(0f, 2f, -5f);

    [Header("Rotation")]
    public float mouseSensitivity = 3f;

    [Tooltip("왼쪽 회전 제한")]
    public float minYaw = -90f;

    [Tooltip("오른쪽 회전 제한")]
    public float maxYaw = 90f;

    private float yaw;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        yaw = 0f;
    }

    void LateUpdate()
    {
        if (target == null) return;

        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        yaw += mouseX;

        yaw = Mathf.Clamp(yaw, minYaw, maxYaw);

        transform.position = target.position;

        transform.rotation = Quaternion.Euler(0f, yaw, 0f);

        Camera.main.transform.position = transform.position + transform.rotation * offset;

        Camera.main.transform.LookAt(target.position + Vector3.up * 1.5f);
    }
}
