using System.Collections;
using UnityEngine;

public class ImpactoObstaculo : MonoBehaviour
{
    [SerializeField] private float umbralGolpeFuerte = 5f;
    [SerializeField] private float distanciaAntesObstaculo = 5f;
    [SerializeField] private float tiempoFrenado = 1f;

    private Rigidbody rb;
    private bool procesandoImpacto;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Solo reacciona a objetos con el tag "Obstaculo".
        if (!collision.collider.CompareTag("Obstaculo")) return;
        if (procesandoImpacto) return;

        float fuerzaImpacto = collision.relativeVelocity.magnitude;

        if (fuerzaImpacto >= umbralGolpeFuerte)
        {
            // Regresa al centro, unos metros antes del obstáculo.
            Vector3 destino = new Vector3(
                0f,
                transform.position.y,
                collision.transform.position.z - distanciaAntesObstaculo
            );

            transform.position = destino;
            DetenerMovimiento();
        }
        else
        {
            StartCoroutine(FrenarUnMomento());
        }
    }

    private void DetenerMovimiento()
    {
        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }
    }

    private IEnumerator FrenarUnMomento()
    {
        procesandoImpacto = true;

        // Desactiva temporalmente los otros scripts del auto.
        MonoBehaviour[] scripts = GetComponents<MonoBehaviour>();

        foreach (MonoBehaviour script in scripts)
        {
            if (script != this)
            {
                script.enabled = false;
            }
        }

        DetenerMovimiento();
        yield return new WaitForSeconds(tiempoFrenado);

        foreach (MonoBehaviour script in scripts)
        {
            if (script != this)
            {
                script.enabled = true;
            }
        }

        procesandoImpacto = false;
    }
}