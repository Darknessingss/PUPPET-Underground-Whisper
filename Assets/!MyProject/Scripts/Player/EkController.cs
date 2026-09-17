using UnityEngine;
using UnityEngine.InputSystem;

public class EkController : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;

    private EkControllerSystem input;
    [SerializeField] private Rigidbody2D RigidBody;
    private Vector2 moveInput;

    private void Awake()
    {
        input = new EkControllerSystem();
    }

    private void OnEnable()
    {
        input.EkMovementSystem.Enable();
    }

    private void OnDisable()
    {
        input.EkMovementSystem.Disable();
    }

    private void Update()
    {
        moveInput = input.EkMovementSystem.EkMovement.ReadValue<Vector2>();
        Debug.Log($"moveInput = {moveInput}");
    }

    private void FixedUpdate()
    {
        RigidBody.linearVelocity = moveInput * moveSpeed;
    }   
}