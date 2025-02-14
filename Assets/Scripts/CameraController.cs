using System.Threading;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour
{
    public Transform CharacterFollowerTransform;
    public Transform CharacterTransform;
    public Transform CameraCenterTargetTransform; // A point slightly to the side of the character
    public float ZoomLevel = 1.0f;
    public Vector2 CameraMovingSpeed = new Vector2(0.0f, 0.0f);
    public Vector2 Sensitivity = new Vector2(1.0f, 1.0f);
    public float CameraSmoothDeceleration = 0.1f;
    public float CameraSmootMaxValue = 12.0f;
    public bool UseSmoothCamera = true;
    float rotationY;
    public bool cameraLockedToTransform = false;
    public Transform lockedTransformToLook;
    public LayerMask lockableTargetMask;
    public Transform TargetEnemy;
    void Start()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    void LateUpdate()
    {
        transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, -2.5f / ZoomLevel);
        CharacterFollowerTransform.position = CharacterTransform.position;



        if (Input.GetMouseButton(1))
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;

            if (UseSmoothCamera && !cameraLockedToTransform)
                SmoothCameraMovement();
            else if (!cameraLockedToTransform)
                SimpleCameraMovement();
        }
        else
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }

        if (!Input.GetMouseButton(1) && Input.GetMouseButtonDown(0))
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            Physics.Raycast(transform.position, ray.direction, out hit);
            if(hit.collider != null && hit.transform.gameObject.layer == 6)
            {
                TargetEnemy = hit.transform;
                TargetEnemy.GetComponent<EnemyScript>().AddPlayerTargetingMe(CharacterTransform);
            }
        }

        HandleInputs();

        if (cameraLockedToTransform)
        {
            UpdateLockedCamera();
        }
    }

    private void SmoothCameraMovement()
    {
        if (Input.GetAxis("Mouse X") != 0.0f)
        {
            float accelerationX = Input.GetAxis("Mouse X") * Sensitivity.x * (1.0f + (Mathf.Pow(Mathf.Abs(CameraMovingSpeed.x), Mathf.Abs(Input.GetAxis("Mouse X")) * 0.1f))) * Time.deltaTime;
            CameraMovingSpeed.x = Mathf.Clamp(CameraMovingSpeed.x + accelerationX, -CameraSmootMaxValue * Sensitivity.x, CameraSmootMaxValue * Sensitivity.x);
        }
        else
        {
            CameraMovingSpeed.x += -CameraMovingSpeed.x * CameraSmoothDeceleration * Time.deltaTime;
        }
        if (Input.GetAxis("Mouse Y") != 0.0f)
        {
            float accelerationY = Input.GetAxis("Mouse Y") * -Sensitivity.y * (1.0f + (Mathf.Pow(Mathf.Abs(CameraMovingSpeed.y), Mathf.Abs(Input.GetAxis("Mouse Y")) * 0.1f))) * Time.deltaTime;
            CameraMovingSpeed.y = Mathf.Clamp(CameraMovingSpeed.y + accelerationY, -CameraSmootMaxValue * Sensitivity.y, CameraSmootMaxValue * Sensitivity.y);
        }
        else
        {
            CameraMovingSpeed.y += -CameraMovingSpeed.y * CameraSmoothDeceleration * Time.deltaTime;
        }

        CharacterFollowerTransform.rotation *= Quaternion.Euler(new Vector3(0.0f, CameraMovingSpeed.x, 0.0f));
        Quaternion rotationQuaternion = CameraCenterTargetTransform.localRotation * Quaternion.Euler(new Vector3(CameraMovingSpeed.y, 0.0f, 0.0f));
        if (rotationQuaternion.x * Mathf.Rad2Deg > -35.0f && rotationQuaternion.x * Mathf.Rad2Deg < 35.0f)
        {
            CameraCenterTargetTransform.localRotation = rotationQuaternion;
        }
    }

    private void SimpleCameraMovement()
    {
        if (Input.GetAxis("Mouse X") != 0.0f)
        {
            CharacterFollowerTransform.rotation *= Quaternion.Euler(new Vector3(0.0f, Input.GetAxis("Mouse X") * Sensitivity.x * 200.0f * Time.deltaTime, 0.0f));
        }
        if (Input.GetAxis("Mouse Y") != 0.0f)
        {
            rotationY += Input.GetAxis("Mouse Y") * -Sensitivity.y * 200 * Time.deltaTime;
            rotationY = Mathf.Clamp(rotationY, -45.0f, 45.0f);
            CameraCenterTargetTransform.localRotation = Quaternion.Euler(rotationY, 0.0f, 0.0f);
        }
    }

    void HandleInputs()
    {
        // Zoom
        if (Input.GetAxis("Mouse ScrollWheel") > 0f)
            ZoomLevel += Mathf.Max(10.0f * ZoomLevel * Time.deltaTime, 0.01f);
        if (Input.GetAxis("Mouse ScrollWheel") < 0f)
            ZoomLevel -= Mathf.Max(10.0f * ZoomLevel * Time.deltaTime, 0.01f);

        // Middle mouse button, lock target
        if (Input.GetMouseButtonDown(2) || (cameraLockedToTransform && lockedTransformToLook == null))
        {
            if (cameraLockedToTransform)
            {
                UnlockCamera();
            }
            else
            {
                Transform transformToLock = SearchClosestLockableTarget();
                if (transformToLock)
                    LockAtTransform(transformToLock);
            }
        }
    }

    public Transform averageCube;
    private void UpdateLockedCamera()
    {
        bool closeToTarget = Vector3.Distance(new Vector3(lockedTransformToLook.position.x, CharacterFollowerTransform.position.y, lockedTransformToLook.position.z), CharacterFollowerTransform.position) < 1.0f;
        if (closeToTarget)
        {
            Vector3 average = (CharacterTransform.position + lockedTransformToLook.position + new Vector3(0.0f, 0.45f, 0.0f)) / 2;
            CameraLookAt(average);
            CharacterFollowerTransform.position = average;
            transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, -2.5f / ZoomLevel * 2.0f);
        }
        else
        {
            CameraLookAt(lockedTransformToLook.position + new Vector3(0.0f, 0.9f, 0.0f));
        }
    }

    void CameraLookAt(Vector3 position)
    {
        bool veryCloseToTarget = Vector3.Distance(new Vector3(position.x, CharacterFollowerTransform.position.y, position.z), CharacterFollowerTransform.position) < 0.1f;
        if (!veryCloseToTarget)
        {
            CharacterFollowerTransform.LookAt(position);
            CharacterFollowerTransform.rotation = Quaternion.Euler(new Vector3(0.0f, CharacterFollowerTransform.eulerAngles.y, 0.0f));
            CameraCenterTargetTransform.LookAt(position);
            CameraCenterTargetTransform.localRotation = Quaternion.Euler(new Vector3(CameraCenterTargetTransform.eulerAngles.x, 0.0f, 0.0f));
        }
    }

    void UnlockCamera()
    {
        cameraLockedToTransform = false;
        lockedTransformToLook = null;
    }

    void LockAtTransform(Transform transform)
    {
        cameraLockedToTransform = true;
        lockedTransformToLook = transform;
    }
    Transform SearchClosestLockableTarget()
    {
        Ray ray = new Ray(transform.position, transform.forward);
        RaycastHit hit;

        // Try Sphere Cast
        Physics.SphereCast(ray, 2.0f, out hit, 255.0f, lockableTargetMask.value);
        if (hit.transform)
        {
            return hit.transform;
        }
        else
        {
            // Try Raycast
            Physics.Raycast(ray, out hit, 255.0f, lockableTargetMask.value);
            if (hit.transform)
            {
                return hit.transform;
            }
            else
            {
                // Search for very close targets
                // Not implemented
                return null;
            }
        }
    }
}
