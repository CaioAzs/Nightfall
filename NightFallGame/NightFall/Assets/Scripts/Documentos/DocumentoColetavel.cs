using UnityEngine;

public class DocumentoColetavel : MonoBehaviour
{
    public Sprite imagemDoDocumento;
    public float pulsarVelocidade = 2f; // Velocidade da pulsação
    public float escalaMaxima = 1.1f;   // Escala máxima
    public float escalaMinima = 0.9f;   // Escala mínima
    public float distanciaMinima = 2f; // Distância máxima para poder abrir
    private Transform player;
    private Vector3 escalaOriginal;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        escalaOriginal = transform.localScale;
    }

    private void Update()
    {
        // Cria o efeito de pulsar usando seno
        float escala = Mathf.Lerp(escalaMinima, escalaMaxima, (Mathf.Sin(Time.time * pulsarVelocidade) + 1f) / 2f);
        transform.localScale = escalaOriginal * escala;
    }

    private void OnMouseDown()
    {
        if (PainelDocumentoController.documentoAberto) return;

        // Verifica se o player está perto o suficiente
        if (Vector2.Distance(transform.position, player.position) <= distanciaMinima)
        {
            PainelDocumentoController painel = FindObjectOfType<PainelDocumentoController>();
            painel.MostrarDocumento(imagemDoDocumento);
        }
    }
}
