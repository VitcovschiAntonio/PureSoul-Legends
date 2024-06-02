using UnityEngine;

public class PlayerLook : MonoBehaviour
{

   
    [SerializeField] public float _sensitivity = 2f;
    [SerializeField] public float _yRotationLimit = 88f;
    [SerializeField] private float _rotationX = 0f;
    [SerializeField] private Transform _cameraHolder;
    void Start()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }
    void LateUpdate()
    {
        Look();
    }

    void Look()
    {
         // Get mouse input for rotation
        float mouseX = Input.GetAxis("Mouse X") * _sensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * _sensitivity;

        // Rotate the player GameObject horizontally
        transform.Rotate(Vector3.up * mouseX);

        // Rotate the camera vertically (limited)
        _rotationX -= mouseY;
        _rotationX = Mathf.Clamp(_rotationX, -_yRotationLimit, _yRotationLimit);
        _cameraHolder.transform.localRotation = Quaternion.Euler(_rotationX, 0f, 0f);
    }
}
