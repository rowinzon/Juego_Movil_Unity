using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;

public class ImpactoObstaculo : MonoBehaviour
{
    [Header("Umbrales")]
    [SerializeField] private float velocidadObstaculoFuerteKmh = 60f;
    [SerializeField] private float umbralCompetidorFuerteMs = 10f;

    [Header("Golpe contra obstáculo")]
    [SerializeField] private float distanciaAntesObstaculo = 5f;
    [SerializeField] private float tiempoFrenadoGolpeSuave = 1f;
    [SerializeField] private float tiempoRecuperacionGolpeFuerte = 2.5f;
    [SerializeField] private float intervaloParpadeo = 0.15f;

    private Rigidbody rb;
    private VidaJugador vidaJugador;
    private ControlAutoJugador controlJugador;
    private Renderer[] renderizadores;
    private bool procesandoImpacto;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        vidaJugador = GetComponent<VidaJugador>();
        controlJugador = GetComponent<ControlAutoJugador>();
        renderizadores = GetComponentsInChildren<Renderer>(true);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (procesandoImpacto) return;

        bool esObstaculo = collision.collider.CompareTag("Obstaculo");
        bool esCompetidor = collision.collider.CompareTag("Competidor");

        if (!esObstaculo && !esCompetidor) return;

        if (vidaJugador == null)
        {
            Debug.LogError("Falta el componente VidaJugador en el auto.", this);
            return;
        }

        if (esObstaculo)
            ProcesarGolpeObstaculo(collision);
        else
            ProcesarGolpeCompetidor(collision);
    }

    private void ProcesarGolpeObstaculo(Collision collision)
    {
        float velocidadKmh = controlJugador != null
            ? Mathf.Abs(controlJugador.VelocidadActualKmh)
            : 0f;

        bool golpeFuerte = velocidadKmh > velocidadObstaculoFuerteKmh;

        if (golpeFuerte)
        {
            vidaJugador.RecibirDanio(100);

            Vector3 destino = new Vector3(
                0f,
                transform.position.y,
                collision.transform.position.z - distanciaAntesObstaculo
            );

            transform.position = destino;
            StartCoroutine(RecuperacionGolpeFuerte());
        }
        else
        {
            vidaJugador.RecibirDanio(20);
            StartCoroutine(FrenarUnMomento(tiempoFrenadoGolpeSuave));
        }
    }

    private void ProcesarGolpeCompetidor(Collision collision)
    {
        float fuerzaImpacto = collision.relativeVelocity.magnitude;
        bool golpeFuerte = fuerzaImpacto >= umbralCompetidorFuerteMs;
        int danio = golpeFuerte ? 70 : 10;

        vidaJugador.RecibirDanio(danio);
        StartCoroutine(FrenarUnMomento(tiempoFrenadoGolpeSuave));
    }

    private IEnumerator RecuperacionGolpeFuerte()
    {
        procesandoImpacto = true;

        BloquearControlYDetenerAuto();

        float tiempoTranscurrido = 0f;
        bool visible = true;

        while (tiempoTranscurrido < tiempoRecuperacionGolpeFuerte)
        {
            visible = !visible;
            CambiarVisibilidad(visible);

            yield return new WaitForSeconds(intervaloParpadeo);
            tiempoTranscurrido += intervaloParpadeo;
        }

        CambiarVisibilidad(true);
        DesbloquearControl();
        procesandoImpacto = false;
    }

    private IEnumerator FrenarUnMomento(float duracion)
    {
        procesandoImpacto = true;

        BloquearControlYDetenerAuto();
        yield return new WaitForSeconds(duracion);

        DesbloquearControl();
        procesandoImpacto = false;
    }

    private void BloquearControlYDetenerAuto()
    {
        if (controlJugador != null)
        {
            controlJugador.ReiniciarVelocidadPorImpacto();
            controlJugador.enabled = false;
        }

        DetenerMovimiento();
    }

    private void DesbloquearControl()
    {
        if (controlJugador != null)
            controlJugador.enabled = true;
    }

    private void DetenerMovimiento()
    {
        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }
    }

    private void CambiarVisibilidad(bool visible)
    {
        foreach (Renderer renderizador in renderizadores)
        {
            if (renderizador != null)
                renderizador.enabled = visible;
        }
    }
}