using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class ControlAutoJugador : MonoBehaviour
{
    [SerializeField] private float velocidadMaximaKmh = 90f;
    [SerializeField] private float velocidadMaximaReversaKmh = 30f;
    [SerializeField] private float aceleracionKmhPorSegundo = 30f;
    [SerializeField] private float aceleracionReversaKmhPorSegundo = 30f;
    [SerializeField] private float desaceleracionKmhPorSegundo = 15f;
    [SerializeField] private float frenadoKmhPorSegundo = 45f;
    [SerializeField] private float frenoManoKmhPorSegundo = 90f;
    [SerializeField] private float velocidadGiro = 90f;

    private Rigidbody rb;
    private float aceleracion;
    private float direccion;
    private bool frenoNormal;
    private bool frenoMano;

    public float VelocidadActualKmh { get; private set; }
    public bool EstaQuemandoLlantas { get; private set; }

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

        aceleracion = 0f;
        direccion = 0f;
        frenoNormal = false;
        frenoMano = false;

        if (teclado == null) return;

        if (teclado.wKey.isPressed || teclado.upArrowKey.isPressed)
            aceleracion = 1f;

        if (teclado.sKey.isPressed || teclado.downArrowKey.isPressed)
            frenoNormal = true;

        if (teclado.spaceKey.isPressed)
            frenoMano = true;

        if (teclado.dKey.isPressed || teclado.rightArrowKey.isPressed)
            direccion += 1f;

        if (teclado.aKey.isPressed || teclado.leftArrowKey.isPressed)
            direccion -= 1f;
    }

    private void FixedUpdate()
    {
        float deltaTiempo = Time.fixedDeltaTime;

        bool quemandoLlantas =
            frenoMano &&
            aceleracion > 0f &&
            Mathf.Abs(VelocidadActualKmh) <= 1f;

        if (quemandoLlantas)
        {
            VelocidadActualKmh = 0f;
        }
        else if (frenoMano)
        {
            VelocidadActualKmh = Mathf.MoveTowards(
                VelocidadActualKmh,
                0f,
                frenoManoKmhPorSegundo * deltaTiempo
            );
        }
        else if (frenoNormal)
        {
            if (VelocidadActualKmh > 0f)
            {
                VelocidadActualKmh = Mathf.MoveTowards(
                    VelocidadActualKmh,
                    0f,
                    frenadoKmhPorSegundo * deltaTiempo
                );
            }
            else
            {
                VelocidadActualKmh = Mathf.MoveTowards(
                    VelocidadActualKmh,
                    -velocidadMaximaReversaKmh,
                    aceleracionReversaKmhPorSegundo * deltaTiempo
                );
            }
        }
        else if (aceleracion > 0f)
        {
            if (VelocidadActualKmh < 0f)
            {
                VelocidadActualKmh = Mathf.MoveTowards(
                    VelocidadActualKmh,
                    0f,
                    frenadoKmhPorSegundo * deltaTiempo
                );
            }
            else
            {
                VelocidadActualKmh = Mathf.MoveTowards(
                    VelocidadActualKmh,
                    velocidadMaximaKmh,
                    aceleracionKmhPorSegundo * deltaTiempo
                );
            }
        }
        else
        {
            VelocidadActualKmh = Mathf.MoveTowards(
                VelocidadActualKmh,
                0f,
                desaceleracionKmhPorSegundo * deltaTiempo
            );
        }

        EstaQuemandoLlantas = quemandoLlantas;

        float velocidadMetrosSegundo = VelocidadActualKmh / 3.6f;

        rb.MovePosition(
            rb.position +
            transform.forward * velocidadMetrosSegundo * deltaTiempo
        );

        if (Mathf.Abs(VelocidadActualKmh) > 0.01f)
        {
            float giro = direccion * velocidadGiro *
                         Mathf.Sign(VelocidadActualKmh);

            rb.MoveRotation(
                rb.rotation *
                Quaternion.Euler(0f, giro * deltaTiempo, 0f)
            );
        }
    }

    public void ReiniciarVelocidadPorImpacto()
    {
        aceleracion = 0f;
        direccion = 0f;
        frenoNormal = false;
        frenoMano = false;
        VelocidadActualKmh = 0f;
        EstaQuemandoLlantas = false;

        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }
    }
}