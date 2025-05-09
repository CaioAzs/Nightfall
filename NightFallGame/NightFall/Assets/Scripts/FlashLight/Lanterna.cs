using UnityEngine;
using UnityEngine.Rendering.Universal;

public class Lanterna : MonoBehaviour
{
    public Light2D lanterna;
    public float intervaloTempoSegundos = 5f;
    public float quantidadeDiminuir = 1f;
    public float tamanhoMinimoOuter = 1f;
    public float tamanhoMaximoOuter = 4f; // Valor máximo para recarga
    public float tamanhoMax = 10f;
    
    private float tempoPassado = 0f;
    private bool morteAcionada = false; // Flag para evitar acionar a morte múltiplas vezes
    
    // Referência ao script PlayerHealth
    public PlayerHealth playerHealth;

    void Start()
    {
        // Auto-referência se não for atribuído
        if (lanterna == null)
        {
            lanterna = GetComponent<Light2D>();
        }
        
        // Tenta encontrar o PlayerHealth se não for atribuído
        if (playerHealth == null)
        {
            // Tenta encontrar no mesmo GameObject
            playerHealth = GetComponent<PlayerHealth>();
            
            // Se não encontrar, tenta encontrar no GameObject pai (se este for filho do player)
            if (playerHealth == null && transform.parent != null)
            {
                playerHealth = transform.parent.GetComponent<PlayerHealth>();
            }
            
            // Se ainda não encontrou, tenta encontrar pelo tag "Player"
            if (playerHealth == null)
            {
                GameObject player = GameObject.FindGameObjectWithTag("Player");
                if (player != null)
                {
                    playerHealth = player.GetComponent<PlayerHealth>();
                }
            }
        }
    }

    void Update()
    {
        tempoPassado += Time.deltaTime;
        
        if (tempoPassado >= intervaloTempoSegundos)
        {
            DiminuirCirculoDeLuz();
            tempoPassado = 0f;
        }
        
        // Verifica se a lanterna atingiu o tamanho mínimo e o player ainda não morreu
        if (lanterna != null && lanterna.pointLightOuterRadius <= tamanhoMinimoOuter && !morteAcionada)
        {
            // Aciona a morte do player
            if (playerHealth != null)
            {
                playerHealth.PlayerDead();
                morteAcionada = true; // Evita acionar a morte múltiplas vezes
                AumentarCirculoDeLuz();
            }
            else
            {
                Debug.LogError("Referência ao PlayerHealth não encontrada! Não foi possível acionar a morte do player.");
            }
        }
    }
    
    void DiminuirCirculoDeLuz()
    {
        if (lanterna != null)
        {
            // Diminui o raio externo do círculo de luz
            float novoRaio = lanterna.pointLightOuterRadius - quantidadeDiminuir;
            
            // Garante que não fique menor que o tamanho mínimo
            lanterna.pointLightOuterRadius = Mathf.Max(novoRaio, tamanhoMinimoOuter);
        }
    }

    void AumentarCirculoDeLuz(){
        if(lanterna != null){
            float novoRaio = lanterna.pointLightOuterRadius * tamanhoMax;
            lanterna.pointLightOuterRadius = Mathf.Max(novoRaio, tamanhoMax);
        }
    }
    
    // Método para recarregar a lanterna (caso precise)
    public void RecarregarLanterna(float quantidade)
    {
        if (lanterna != null)
        {
            // Aumenta o raio até o máximo definido
            lanterna.pointLightOuterRadius = Mathf.Min(lanterna.pointLightOuterRadius + quantidade, tamanhoMaximoOuter);
            
            // Se a lanterna foi recarregada, permite que a morte seja acionada novamente se necessário
            if (lanterna.pointLightOuterRadius > tamanhoMinimoOuter)
            {
                morteAcionada = false;
            }
        }
    }
}