using UnityEngine;
using UnityEngine.InputSystem;

public class CamaraJugador : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private Transform jugador;
    [SerializeField] private Transform puntoCamaraCapo;

    [Header("Vista trasera")]
    [SerializeField] private Vector3 desplazamientoTrasero = new Vector3(0f, 4f, -7f);
    [SerializeField] private Vector3 puntoMiraTrasero = new Vector3(0f, 1.5f, 5f);

    [Header("Suavizado")]
    [SerializeField] private float suavizado = 8f;

    private bool vistaCapo;
    private Vector3 velocidadSuavizado;

    private void LateUpdate()
    {
        if (jugador == null) return;

        if (Keyboard.current != null &&
            Keyboard.current.cKey.wasPressedThisFrame)
        {
            vistaCapo = !vistaCapo;
        }

        Vector3 posicionObjetivo;
        Vector3 puntoMira;

        if (vistaCapo && puntoCamaraCapo != null)
        {
            transform.position = puntoCamaraCapo.position;
            transform.rotation = puntoCamaraCapo.rotation;
            return;
        }
        else
        {
            posicionObjetivo =
                jugador.position +
                jugador.rotation * desplazamientoTrasero;

            puntoMira =
                jugador.position +
                jugador.rotation * puntoMiraTrasero;
        }

        transform.position = Vector3.SmoothDamp(
            transform.position,
            posicionObjetivo,
            ref velocidadSuavizado,
            1f / suavizado
        );

        transform.rotation = Quaternion.LookRotation(
            puntoMira - transform.position
        );
    }
}