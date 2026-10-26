using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class ControlAutoJugador : MonoBehaviour
{
    [SerializeField] private float velocidad = 12f;
    [SerializeField] private float velocidadGiro = 90f;

    private Rigidbody rb;
    private float aceleracion;
    private float direccion;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.constraints =
            RigidbodyConstraints.FreezeRotationX |
            RigidbodyConstraints.FreezeRotationZ;
    }

    private void Update()
    {
        Keyboard teclado = Keyboard.current;
        if (teclado == null) return;

        aceleracion = 0f;
        direccion = 0f;

        if (teclado.wKey.isPressed || teclado.upArrowKey.isPressed)
            aceleracion += 1f;
        if (teclado.sKey.isPressed || teclado.downArrowKey.isPressed)
            aceleracion -= 1f;
        if (teclado.dKey.isPressed || teclado.rightArrowKey.isPressed)
            direccion += 1f;
        if (teclado.aKey.isPressed || teclado.leftArrowKey.isPressed)
            direccion -= 1f;
    }

    private void FixedUpdate()
    {
        rb.MovePosition(
            rb.position + transform.forward * aceleracion * velocidad * Time.fixedDeltaTime
        );

        if (Mathf.Abs(aceleracion) > 0.01f)
        {
            float giro = direccion * velocidadGiro * Mathf.Sign(aceleracion);
            rb.MoveRotation(
                rb.rotation * Quaternion.Euler(0f, giro * Time.fixedDeltaTime, 0f)
            );
        }
    }
}