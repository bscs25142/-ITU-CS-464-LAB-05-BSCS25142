using UnityEngine;
using UnityEngine.InputSystem;

// This script goes on the BLOCKOUT, not the Ball.
// The Ball is left alone - it just has a Rigidbody + gravity and reacts
// to the tilted floor naturally through physics.
[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    [Header("Tilt Settings")]
    [SerializeField] private float tiltSpeed = 40f;     // degrees/sec towards target tilt
    [SerializeField] private float maxTiltAngle = 25f;   // max degrees of tilt

    private Rigidbody rb;
    private float currentTiltX;
    private float currentTiltZ;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.isKinematic = true; // the box is moved by script, never by physics forces
    }

    private void FixedUpdate()
    {
        Vector2 input = ReadInput();

        // A/D or Left/Right -> tilt around Z. W/S or Up/Down -> tilt around X.
        float targetTiltZ = -input.x * maxTiltAngle;
        float targetTiltX = input.y * maxTiltAngle;

        currentTiltX = Mathf.MoveTowards(currentTiltX, targetTiltX, tiltSpeed * Time.fixedDeltaTime);
        currentTiltZ = Mathf.MoveTowards(currentTiltZ, targetTiltZ, tiltSpeed * Time.fixedDeltaTime);

        Quaternion targetRotation = Quaternion.Euler(currentTiltX, 0f, currentTiltZ);
        rb.MoveRotation(targetRotation);
    }

    private Vector2 ReadInput()
    {
        Vector2 input = Vector2.zero;
        var kb = Keyboard.current;
        if (kb == null) return input; // no keyboard device detected this frame

        if (kb.aKey.isPressed || kb.leftArrowKey.isPressed) input.x -= 1f;
        if (kb.dKey.isPressed || kb.rightArrowKey.isPressed) input.x += 1f;
        if (kb.sKey.isPressed || kb.downArrowKey.isPressed) input.y -= 1f;
        if (kb.wKey.isPressed || kb.upArrowKey.isPressed) input.y += 1f;

        return Vector2.ClampMagnitude(input, 1f);
    }
}