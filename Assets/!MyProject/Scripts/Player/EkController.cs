using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{

    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private Rigidbody2D RigidBody;
    private EkControllerSystem input;

    private Vector2 moveInput;

    private void Awake()
    {
        input = new EkControllerSystem();
    }

    private void OnEnable()
    {
        input.Enable();
        input.EkMovementSystem.EkMovement.performed += OnMove;
        input.EkMovementSystem.EkMovement.canceled += OnMove;
    }

    private void OnDisable()
    {
        input.EkMovementSystem.EkMovement.performed -= OnMove;
        input.EkMovementSystem.EkMovement.canceled -= OnMove;
        input.EkMovementSystem.Disable();
    }

    private void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
    }

    private void FixedUpdate()
    {
        RigidBody.linearVelocity = new Vector2(moveInput.x * moveSpeed, moveInput.y * moveSpeed);
    }
}