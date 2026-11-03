using TMPro;
using UnityEngine;

public class MostrarVelocidad : MonoBehaviour
{
    [SerializeField] private Rigidbody auto;

    private TextMeshProUGUI texto;
    private Vector3 posicionAnterior;
    private float velocidadKmh;

    private void Awake()
    {
        texto = GetComponent<TextMeshProUGUI>();

        if (auto != null)
        {
            posicionAnterior = auto.position;
        }
    }

    private void FixedUpdate()
    {
        if (auto == null) return;

        Vector3 posicionActual = auto.position;
        Vector3 desplazamiento = posicionActual - posicionAnterior;

        // Ignora el movimiento vertical.
        desplazamiento.y = 0f;

        velocidadKmh =
            desplazamiento.magnitude / Time.fixedDeltaTime * 3.6f;

        posicionAnterior = posicionActual;
    }

    private void Update()
    {
        if (texto == null) return;

        texto.text = $"Velocidad: {velocidadKmh:0} km/h";
    }
}