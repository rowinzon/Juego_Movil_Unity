using UnityEngine;

public class VidaJugador : MonoBehaviour
{
    private const int VidaInicial = 1000;

    [SerializeField] private int vidaActual = VidaInicial;

    public int VidaActual => vidaActual;
    public bool Derrotado { get; private set; }

    private void Awake()
    {
        vidaActual = VidaInicial;
        Derrotado = false;
    }

    public void RecibirDanio(int cantidad)
    {
        if (Derrotado || cantidad <= 0)
            return;

        vidaActual = Mathf.Max(0, vidaActual - cantidad);
        Debug.Log($"Vida actual: {vidaActual}", this);

        if (vidaActual == 0)
        {
            Derrotado = true;
            Debug.Log("La partida terminó.", this);
        }
    }
}