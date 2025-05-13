using UnityEngine;

public class DocumentoColetavel : MonoBehaviour
{
    public Sprite imagemDoDocumento;
    public float brilhoVelocidade = 2f; // Velocidade da pulsação de brilho
    public float brilhoMin = 0.5f;      // Intensidade mínima do brilho
    public float brilhoMax = 1.2f;      // Intensidade máxima do brilho
    public float distanciaMinima = 2f;  // Distância máxima para poder abrir

    private Transform player;
    private SpriteRenderer spriteRenderer;
    private Color corOriginal;
    private bool brilhoAtivo = true; // Flag para controlar o brilho

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        spriteRenderer = GetComponent<SpriteRenderer>();
        corOriginal = spriteRenderer.color;
    }

    private void Update()
    {
        if (brilhoAtivo)
        {
            // Efeito de brilho pulsante
            float intensidade = Mathf.Lerp(brilhoMin, brilhoMax, Mathf.PingPong(Time.time * brilhoVelocidade, 1));
            spriteRenderer.color = corOriginal * intensidade;
        }
    }

    private void OnMouseDown()
    {
        if (PainelDocumentoController.documentoAberto) return;

        if (Vector2.Distance(transform.position, player.position) <= distanciaMinima)
        {
            // Desativa o brilho e restaura a cor original
            brilhoAtivo = false;
            spriteRenderer.color = corOriginal;

            PainelDocumentoController painel = FindObjectOfType<PainelDocumentoController>();
            painel.MostrarDocumento(imagemDoDocumento);
        }
    }
}
