using UnityEngine;

public class ItemBateria : MonoBehaviour
{
    public float quantidadeRecarga = 3f; // Quanto recarregar da bateria

    public float intensidadeMin = 0.5f;
    public float intensidadeMax = 1.5f;
    public float velocidade = 2f;

    private SpriteRenderer spriteRenderer;
    private Color corOriginal;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            corOriginal = spriteRenderer.color;
        }
    }

    void Update()
    {
        if (spriteRenderer != null)
        {
            float intensidade = Mathf.Lerp(intensidadeMin, intensidadeMax, Mathf.PingPong(Time.time * velocidade, 1));
            spriteRenderer.color = corOriginal * intensidade;
        }
    }
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        // Verificar se colidiu com o player
        if (other.CompareTag("Player"))
        {
            // Encontrar o script da lanterna
            Lanterna lanterna = FindObjectOfType<Lanterna>();
            
            if (lanterna != null)
            {
                // Recarregar a lanterna diretamente no script dela
                lanterna.RecarregarLanterna(quantidadeRecarga);
                
                // Destruir a bateria
                Destroy(gameObject);
            }
        }
    }
}
