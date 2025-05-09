using UnityEngine;
using UnityEngine.Rendering.Universal;
using System.Collections;

public class Lanterna : MonoBehaviour
{
    public Light2D lanterna;
    public float intervaloTempoSegundos = 5f;
    public float quantidadeDiminuir = 1f;
    public float tamanhoMinimoOuter = 1f;
    public float tamanhoMaximoOuter = 4f; // Valor máximo normal para recarga
    
    [Header("Efeitos de Som")]
    public AudioClip somMonstro; // Som do monstro que toca antes da morte
    public AudioClip somMorteBateria; // Som quando o player morre por falta de bateria
    public float volumeSom = 1.0f; // Volume do som
    public float atrasoEntreRuidoEMorte = 1.5f; // Tempo entre o ruído do monstro e a morte do player
    
    [Header("Iluminação Após Morte")]
    public float tamanhoOuterAposMorte = 2f; // Tamanho da luz após morte
    public float velocidadeIluminacao = 1.0f; // Velocidade de transição
    
    // Variáveis privadas
    private float tempoPassado = 0f;
    private bool morteAcionada = false;
    private bool transicaoLuzAposMorte = false;
    private bool sequenciaMorteEmAndamento = false;
    private float valorRealBateria = 0f; // Armazena o valor real da bateria para a UI
    
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
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                playerHealth = player.GetComponent<PlayerHealth>();
            }
        }
        
        // Inicializa o valor real da bateria
        valorRealBateria = lanterna.pointLightOuterRadius;
    }

    void Update()
    {
        // Se o player estiver morto, não execute a lógica de diminuir a luz
        if (playerHealth != null && playerHealth.dead)
        {
            // Se estiver na transição de luz após a morte
            if (transicaoLuzAposMorte)
            {
                if (lanterna != null)
                {
                    lanterna.pointLightOuterRadius = Mathf.Lerp(
                        lanterna.pointLightOuterRadius, 
                        tamanhoOuterAposMorte, 
                        Time.deltaTime * velocidadeIluminacao
                    );
                    
                    // Se chegou próximo do valor desejado, finaliza e fixa em exatamente 2
                    if (Mathf.Abs(lanterna.pointLightOuterRadius - tamanhoOuterAposMorte) < 0.05f)
                    {
                        lanterna.pointLightOuterRadius = tamanhoOuterAposMorte; // Fixa em exatamente 2
                        transicaoLuzAposMorte = false;
                    }
                }
            }
            return; // Não executa o resto da lógica se o player estiver morto
        }
        
        // Não executa a lógica normal se a sequência de morte já estiver em andamento
        if (sequenciaMorteEmAndamento)
        {
            return;
        }
        
        // Lógica normal da lanterna (só executa se o player estiver vivo)
        tempoPassado += Time.deltaTime;
        
        if (tempoPassado >= intervaloTempoSegundos)
        {
            DiminuirCirculoDeLuz();
            tempoPassado = 0f;
        }
        
        // Verificar se a lanterna atingiu o tamanho mínimo
        if (lanterna != null && lanterna.pointLightOuterRadius <= tamanhoMinimoOuter && !morteAcionada)
        {
            if (playerHealth != null && !playerHealth.dead)
            {
                // Salvar o valor real da bateria antes da sequência de morte
                valorRealBateria = tamanhoMinimoOuter;
                
                // Iniciar sequência de morte
                StartCoroutine(SequenciaMorte());
            }
        }
    }
    
    // Sequência de eventos para a morte do player
    private IEnumerator SequenciaMorte()
    {
        sequenciaMorteEmAndamento = true;
        
        // 1. Reproduzir o som do monstro primeiro
        if (somMonstro != null)
        {
            AudioSource.PlayClipAtPoint(somMonstro, transform.position, volumeSom);
        }
        
        // 2. Aguardar o intervalo definido
        yield return new WaitForSeconds(atrasoEntreRuidoEMorte);
        
        // 3. Reproduzir o som de morte
        if (somMorteBateria != null)
        {
            AudioSource.PlayClipAtPoint(somMorteBateria, transform.position, volumeSom);
        }
        
        // 4. Acionar a morte do player
        if (playerHealth != null && !playerHealth.dead)
        {
            playerHealth.PlayerDead();
            morteAcionada = true;
            
            // 5. Iniciar a transição para luz fixa após morte
            transicaoLuzAposMorte = true;
        }
        
        sequenciaMorteEmAndamento = false;
    }
    
    void DiminuirCirculoDeLuz()
    {
        if (lanterna != null)
        {
            // Diminui o raio externo do círculo de luz
            float novoRaio = lanterna.pointLightOuterRadius - quantidadeDiminuir;
            
            // Garante que não fique menor que o tamanho mínimo
            lanterna.pointLightOuterRadius = Mathf.Max(novoRaio, tamanhoMinimoOuter);
            
            // Atualiza o valor real da bateria para a UI
            valorRealBateria = lanterna.pointLightOuterRadius;
        }
    }
    
    // Método para recarregar a lanterna
    public void RecarregarLanterna(float quantidade)
    {
        // Não permite recarregar se o player estiver morto ou transição em andamento
        if (lanterna != null && !transicaoLuzAposMorte && !sequenciaMorteEmAndamento && (playerHealth == null || !playerHealth.dead))
        {
            // Aumenta o raio até o máximo definido
            lanterna.pointLightOuterRadius = Mathf.Min(lanterna.pointLightOuterRadius + quantidade, tamanhoMaximoOuter);
            
            // Atualiza o valor real da bateria para a UI
            valorRealBateria = lanterna.pointLightOuterRadius;
            
            // Se a lanterna foi recarregada, permite que a morte seja acionada novamente
            if (lanterna.pointLightOuterRadius > tamanhoMinimoOuter)
            {
                morteAcionada = false;
            }
        }
    }
    
    // Método para obter o valor real da bateria para a UI (independente do efeito visual)
    public float GetValorRealBateria()
    {
        return valorRealBateria;
    }
    
    // Método para obter o valor mínimo da bateria
    public float GetValorMinimoBateria()
    {
        return tamanhoMinimoOuter;
    }
    
    // Método para obter o valor máximo da bateria
    public float GetValorMaximoBateria()
    {
        return tamanhoMaximoOuter;
    }
    
    // Método para resetar a lanterna quando o jogo recomeçar
    public void ResetarLanterna()
    {
        if (lanterna != null)
        {
            lanterna.pointLightOuterRadius = tamanhoMaximoOuter;
            valorRealBateria = tamanhoMaximoOuter;
            morteAcionada = false;
            transicaoLuzAposMorte = false;
            sequenciaMorteEmAndamento = false;
            StopAllCoroutines(); // Para qualquer sequência de morte em andamento
        }
    }
}