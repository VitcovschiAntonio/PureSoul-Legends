using UnityEngine;

public class WeaponSway : MonoBehaviour
{
    [Header("Sway Settings")]
    [SerializeField] private float _swayAmount = 0.02f;
    [SerializeField] private float _maxSwayAmount = 0.06f;
    [SerializeField] private float _smoothFactor = 4f;

    private Vector3 _initialPosition;

    void Start()
    {
        _initialPosition = transform.localPosition;
    }

    void Update()
    {
        
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");

        float swayX = Mathf.Clamp(-mouseX * _swayAmount, -_maxSwayAmount, _maxSwayAmount);
        float swayY = Mathf.Clamp(-mouseY * _swayAmount, -_maxSwayAmount, _maxSwayAmount);

        // Calculate target position with sway
        Vector3 targetPosition = new Vector3(swayX, swayY, 0) + _initialPosition;

        // Smoothly move towards the target position
        transform.localPosition = Vector3.Lerp(transform.localPosition, targetPosition, Time.deltaTime * _smoothFactor);
    }
}
