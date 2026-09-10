using UnityEngine;
using UnityEngine.InputSystem;

   
public class jj : MonoBehaviour
{
    public InputActionReference moveActionReference; // mismo tipo que en el rebind

    InputAction moveAction;

    void OnEnable()
    {
        moveAction = moveActionReference.action; // la MISMA instancia que usa el rebind
        moveAction.Enable();
        moveAction.performed += OnMove;
    }

    void OnDisable()
    {
        moveAction.performed -= OnMove;
        moveAction.Disable();
    }

    void Update()
    {
        Debug.Log("Enabled: " + moveAction.enabled + " | Valor: " + moveAction.ReadValue<Vector2>());
    }

    void OnMove(InputAction.CallbackContext context)
    {
        Vector2 direccion = context.ReadValue<Vector2>();
        if (direccion != Vector2.zero)
            Debug.Log("el pepe2");
    }
}
