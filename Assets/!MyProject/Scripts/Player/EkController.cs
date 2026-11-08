using UnityEngine;
using UnityEngine.InputSystem;

public class EkController : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private Rigidbody2D RigidBody;
    [SerializeField] private Animator _animator;
    private EkControllerSystem input;

    private Vector2 moveInput;
    private bool _movementLocked;
    private bool _dead;

    private void Awake()
    {
        input = new EkControllerSystem();
    }

    private void Update()
    {
        _animator.SetFloat("Vertical", moveInput.y);
        _animator.SetFloat("Horizontal", moveInput.x);
        _animator.SetFloat("Speed", moveInput.sqrMagnitude);
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
        input.Disable();
    }

    private void OnDestroy()
    {
        input.Dispose();
        input = null;
    }

    private void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
    }

    private void FixedUpdate()
    {
        RigidBody.linearVelocity = (_dead || _movementLocked) ? Vector2.zero : moveInput.normalized * moveSpeed;
    }

    public void SetMovementLocked(bool locked)
    {
        _movementLocked = locked;
        if (locked) RigidBody.linearVelocity = Vector2.zero;
    }

    public void SetDead()
    {
        _dead = true;
        _movementLocked = true;
        RigidBody.linearVelocity = Vector2.zero;
    }
}