using Mono.Cecil;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour
{
    public Transform CharacterTransform;
    public Transform TargetTransform;
    public float ZoomLevel = 1.0f;
    public Vector2 Sensitivity = new Vector2(1.0f, 1.0f);
    void Start()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, -2.5f / ZoomLevel);

        if (Input.GetAxis("Mouse ScrollWheel") > 0f)
        {
            ZoomLevel += Mathf.Max(10.0f * ZoomLevel * Time.deltaTime, 0.01f);
        }
        if (Input.GetAxis("Mouse ScrollWheel") < 0f)
        {
            ZoomLevel -= Mathf.Max(10.0f * ZoomLevel * Time.deltaTime, 0.01f);
        }

        if (Input.GetAxis("Mouse X") != 0.0f)
        {
            CharacterTransform.rotation *= Quaternion.Euler(new Vector3(0.0f, Input.GetAxis("Mouse X") * Sensitivity.x * Time.deltaTime, 0.0f));
        }
        if (Input.GetAxis("Mouse Y") != 0.0f)
        {
            TargetTransform.rotation *= Quaternion.Euler(new Vector3(Input.GetAxis("Mouse Y") * -Sensitivity.y * Time.deltaTime, 0.0f, 0.0f));
        }

    }
}
