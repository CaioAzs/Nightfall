using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Rendering.Universal;
using System.Collections;

public class GameManager : MonoBehaviour
{
    // Singleton
    public static GameManager Instance { get; private set; }
    
    [Header("Referências de Scripts")]
    public PlayerHealth playerHealth;
    public BossHealth bossHealth;
    public Lanterna lanternaScript;
    
    [Header("UI")]
    public Image playerHealthBar;
    public Image bossHealthBar;
    public Image bateriaBar;
    
    [Header("Configurações")]
    public float playerMaxHealth = 4f; // Valor máximo de saúde do player
    public float lanternaMinOuter = 1f; // Valor mínimo da lanterna
    public float lanternaMaxOuter = 4f; // Valor máximo da lanterna
    public int bossMaxHealth = 12; // Valor máximo de saúde do boss
    [SerializeField] private bool resetarBateriaNoCarregamento = true; // Opção para resetar a bateria ao carregar uma nova cena

    [Header("Chaves de Elevador")]
    public int chavesElevadorColetadas = 0;
    
    // Variável para persistência entre cenas
    [HideInInspector] public float playerSavedHealth;
    
    // Flag para controlar inicialização
    private bool isInitialized = false;
    
    private void Awake()
    {
        // Implementação do padrão Singleton
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            
            // Inicializar valores padrão
            playerSavedHealth = playerMaxHealth; // Inicializar com valor máximo por segurança
            
            // Registrar para o evento de mudança de cena
            SceneManager.sceneLoaded += OnSceneLoaded;
            
            // Registrar para o evento de reset do player
            PlayerHealth.OnPlayerDeath += OnPlayerDeathOrReset;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    // Novo método para escutar eventos de morte ou reset do player
    private void OnPlayerDeathOrReset(bool isDead)
    {
        // Quando o player é resetado (isDead = false), atualizamos nossas referências
        if (!isDead)
        {
            Debug.Log("GameManager: Player foi resetado, atualizando referências");
            
            // Verificar se estamos em uma tela de reset completo
            if (SceneManager.GetActiveScene().name.Contains("Lose") || 
                SceneManager.GetActiveScene().name.Contains("Win") ||
                SceneManager.GetActiveScene().name.Contains("Menu"))
            {
                // Restaurar vida cheia para o próximo carregamento
                playerSavedHealth = playerMaxHealth;
                Debug.Log("GameManager: Reset completo, vida restaurada para máximo");
                
                // Resetar a bateria da lanterna também
                ResetarBateriaLanterna();
            }
            
            StartCoroutine(AtualizarReferenciasAposDelay(0.2f));
        }
    }
    
    private IEnumerator AtualizarReferenciasAposDelay(float delay)
    {
        // Pequeno delay para garantir que tudo foi inicializado
        yield return new WaitForSeconds(delay);
        
        // Encontrar referências novamente
        EncontrarReferencias();
        
        // Atualizar barras de vida
        AtualizarBarras();
        
        Debug.Log("GameManager: Referências atualizadas após reset do player");
    }
    
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Usar um coroutine com delay para garantir que todos os objetos estejam carregados
        StartCoroutine(EncontrarReferenciasAposCarregamento());
    }
    
    private IEnumerator EncontrarReferenciasAposCarregamento()
    {
        // Esperar dois frames para garantir que objetos estejam instanciados
        yield return null;
        yield return null;
        
        // Buscar referências na nova cena
        EncontrarReferencias();
        
        // Verificar se estamos em uma cena de jogo ou menu
        string sceneName = SceneManager.GetActiveScene().name;
        bool isGameplayScene = !(sceneName.Contains("Menu") || 
                                 sceneName.Contains("Win") || 
                                 sceneName.Contains("Lose") ||
                                 sceneName.Contains("Dialogue"));
        
        // Apenas aplicar valores salvos em cenas de gameplay
        if (isGameplayScene)
        {
            // Aplicar valores do player
            AplicarValoresSalvosPlayer();
        }
        else
        {
            // Em cenas não-gameplay, podemos restaurar valores para o máximo
            playerSavedHealth = playerMaxHealth;
            AtualizarBarras();
        }
        
        Debug.Log("GameManager: Referências recarregadas após troca de cena para: " + sceneName);
    }
    
    private void Start()
    {
        if (!isInitialized)
        {
            EncontrarReferencias();
            isInitialized = true;
            
            // Se não houver valor salvo de saúde do player, inicializar com o valor máximo
            if (playerSavedHealth <= 0)
            {
                playerSavedHealth = playerMaxHealth;
            }
            
            // Atualizar barras
            AtualizarBarras();
        }
    }
    
    private void Update()
    {
        // Atualizar todas as barras que estiverem disponíveis
        AtualizarBarras();
        
        // Salvar a saúde atual do player para persistência
        if (playerHealth != null && playerHealth.currentHealth > 0)
        {
            playerSavedHealth = playerHealth.currentHealth;
        }
    }
    
    private void EncontrarReferencias()
    {
        // Buscar todas as referências disponíveis na cena atual com mais opções de fallback
        
        // Player (sempre buscar)
        if (playerHealth == null)
        {
            playerHealth = FindObjectOfType<PlayerHealth>();
            
            // Fallback para procurar por tag se o tipo não for suficiente
            if (playerHealth == null)
            {
                GameObject playerObj = GameObject.FindWithTag("Player");
                if (playerObj != null)
                {
                    playerHealth = playerObj.GetComponent<PlayerHealth>();
                    Debug.Log("GameManager: Player encontrado por tag");
                }
            }
        }
        
        if (playerHealthBar == null)
        {
            // Tentar por tag
            var healthbarObj = GameObject.FindWithTag("PlayerHealthBar");
            if (healthbarObj != null)
            {
                playerHealthBar = healthbarObj.GetComponent<Image>();
                Debug.Log("GameManager: Barra de vida do player encontrada por tag");
            }
            else
            {
                // Fallback para tentar encontrar por nome
                var hbObj = GameObject.Find("PlayerHealthBar");
                if (hbObj != null)
                {
                    playerHealthBar = hbObj.GetComponent<Image>();
                    Debug.Log("GameManager: Barra de vida do player encontrada por nome");
                }
                
                // Busca mais ampla por qualquer objeto com nome que contenha "HealthBar" e "Player"
                if (playerHealthBar == null)
                {
                    var images = FindObjectsOfType<Image>();
                    foreach (var img in images)
                    {
                        if (img.name.Contains("HealthBar") && (img.name.Contains("Player") || img.transform.parent.name.Contains("Player")))
                        {
                            playerHealthBar = img;
                            Debug.Log("GameManager: Barra de vida do player encontrada por busca ampla: " + img.name);
                            break;
                        }
                    }
                }
            }
        }
        
        // Lanterna
        if (lanternaScript == null)
        {
            lanternaScript = FindObjectOfType<Lanterna>();
            
            // Fallback por tag
            if (lanternaScript == null)
            {
                GameObject lanternaObj = GameObject.FindWithTag("Lanterna");
                if (lanternaObj != null)
                {
                    lanternaScript = lanternaObj.GetComponent<Lanterna>();
                    Debug.Log("GameManager: Lanterna encontrada por tag");
                }
            }
        }
        
        if (bateriaBar == null)
        {
            // Tentar por tag
            var bateriaUI = GameObject.FindWithTag("BateriaUI");
            if (bateriaUI != null)
            {
                bateriaBar = bateriaUI.GetComponent<Image>();
                Debug.Log("GameManager: Barra de bateria encontrada por tag");
            }
            else
            {
                // Fallback para tentar encontrar por nome
                var bateriaObj = GameObject.Find("BateriaUI");
                if (bateriaObj != null)
                {
                    bateriaBar = bateriaObj.GetComponent<Image>();
                    Debug.Log("GameManager: Barra de bateria encontrada por nome");
                }
                
                // Busca ampla por qualquer barra com nome relacionado a bateria
                if (bateriaBar == null)
                {
                    var images = FindObjectsOfType<Image>();
                    foreach (var img in images)
                    {
                        if (img.name.Contains("Bateria") || img.name.Contains("Battery"))
                        {
                            bateriaBar = img;
                            Debug.Log("GameManager: Barra de bateria encontrada por busca ampla: " + img.name);
                            break;
                        }
                    }
                }
            }
        }
        
        // Boss
        if (bossHealth == null)
        {
            bossHealth = FindObjectOfType<BossHealth>();
            
            // Fallback por tag
            if (bossHealth == null)
            {
                GameObject bossObj = GameObject.FindWithTag("Boss");
                if (bossObj != null)
                {
                    bossHealth = bossObj.GetComponent<BossHealth>();
                    Debug.Log("GameManager: Boss encontrado por tag");
                }
            }
        }
        
        if (bossHealthBar == null)
        {
            // Tentar por tag
            var bossHealthbarObj = GameObject.FindWithTag("BossHealthBar");
            if (bossHealthbarObj != null)
            {
                bossHealthBar = bossHealthbarObj.GetComponent<Image>();
                Debug.Log("GameManager: Barra de vida do boss encontrada por tag");
            }
            else
            {
                // Fallback para tentar encontrar por nome
                var bhObj = GameObject.Find("BossHealthBar");
                if (bhObj != null)
                {
                    bossHealthBar = bhObj.GetComponent<Image>();
                    Debug.Log("GameManager: Barra de vida do boss encontrada por nome");
                }
            }
        }
        
        // Ativar ou desativar a barra de vida do boss baseado na presença de um boss na cena
        SetBossHealthBarActive(bossHealth != null);
        
        // Log para confirmar quais referências foram encontradas
        string logMessage = "GameManager: Referências encontradas: ";
        logMessage += playerHealth != null ? "Player ✓, " : "Player ✗, ";
        logMessage += playerHealthBar != null ? "PlayerBar ✓, " : "PlayerBar ✗, ";
        logMessage += lanternaScript != null ? "Lanterna ✓, " : "Lanterna ✗, ";
        logMessage += bateriaBar != null ? "BateriaBar ✓, " : "BateriaBar ✗, ";
        logMessage += bossHealth != null ? "Boss ✓, " : "Boss ✗, ";
        logMessage += bossHealthBar != null ? "BossBar ✓" : "BossBar ✗";
        
        Debug.Log(logMessage);
    }
    
    private void AplicarValoresSalvosPlayer()
    {
        // Como não podemos definir currentHealth diretamente, usamos ResetPlayer e TakeDamage
        if (playerHealth != null)
        {
            Debug.Log("GameManager: Aplicando saúde salva: " + playerSavedHealth + " / " + playerMaxHealth);
            
            // Para reset completo, definir saúde salva como máxima
            if (SceneManager.GetActiveScene().name.Contains("Lose") || 
                SceneManager.GetActiveScene().name.Contains("Win") ||
                SceneManager.GetActiveScene().name.Contains("Menu"))
            {
                // Se estamos voltando de uma tela de derrota/vitória ou menu, restaurar vida cheia
                playerSavedHealth = playerMaxHealth;
                Debug.Log("GameManager: Reset completo detectado, restaurando saúde máxima");
            }
            else if (playerSavedHealth <= 0)
            {
                // Se não temos um valor salvo ou é inválido, usar o valor padrão
                playerSavedHealth = playerMaxHealth;
            }
                
            // Primeiro, resetamos para a saúde máxima
            playerHealth.ResetPlayer();
            
            // Se não queremos saúde máxima, aplicamos dano para ajustar
            if (playerSavedHealth < playerMaxHealth)
            {
                // Em seguida, calculamos quanto dano deve ser aplicado para chegar no valor salvo
                // Usamos um coroutine com delay para garantir que o reset foi concluído
                StartCoroutine(AplicarDanoComDelay(playerMaxHealth - playerSavedHealth));
            }
            else
            {
                // Se estamos com saúde máxima, forçar atualização das barras
                AtualizarBarras();
            }
            
            // Restaurar a bateria da lanterna, se a opção estiver ativada
            if (resetarBateriaNoCarregamento)
            {
                ResetarBateriaLanterna();
            }
        }
    }
    
    // Adicione este novo método para resetar a bateria da lanterna
    private void ResetarBateriaLanterna()
    {
        if (lanternaScript != null)
        {
            bool bateriaRecarregada = false;
            
            // Verificar se o método para recarregar a bateria existe
            var recarregarMethod = lanternaScript.GetType().GetMethod("RecarregarCompletamente");
            if (recarregarMethod != null)
            {
                // Chamar o método para recarregar completamente
                recarregarMethod.Invoke(lanternaScript, null);
                Debug.Log("GameManager: Bateria da lanterna recarregada via RecarregarCompletamente");
                bateriaRecarregada = true;
            }
            else
            {
                // Tentar outros métodos comuns que possam existir
                var metodos = new string[] { "RecarregarBateria", "ResetarBateria", "EncheBateria", "ResetarLanterna" };
                
                foreach (var nomeMetodo in metodos)
                {
                    var metodo = lanternaScript.GetType().GetMethod(nomeMetodo);
                    if (metodo != null)
                    {
                        metodo.Invoke(lanternaScript, null);
                        Debug.Log($"GameManager: Bateria recarregada usando método {nomeMetodo}");
                        bateriaRecarregada = true;
                        break;
                    }
                }
            }
            
            // Se nenhum método funcionou, tentar adicionar um método de recarga diretamente
            if (!bateriaRecarregada)
            {
                Debug.Log("GameManager: Tentando recarregar bateria manualmente");
                try
                {
                    // Tentar definir o valor diretamente pelo SetValorRealBateria
                    var setValueMethod = lanternaScript.GetType().GetMethod("SetValorRealBateria");
                    var getMaxMethod = lanternaScript.GetType().GetMethod("GetValorMaximoBateria");
                    
                    if (setValueMethod != null && getMaxMethod != null)
                    {
                        float maxValue = (float)getMaxMethod.Invoke(lanternaScript, null);
                        setValueMethod.Invoke(lanternaScript, new object[] { maxValue });
                        Debug.Log("GameManager: Bateria recarregada manualmente para: " + maxValue);
                        bateriaRecarregada = true;
                    }
                }
                catch (System.Exception e)
                {
                    Debug.LogWarning("GameManager: Erro ao tentar recarregar bateria: " + e.Message);
                }
            }
            
            // Atualizar a barra de bateria de qualquer forma
            AtualizarBarraBateria();
        }
    }
    
    private IEnumerator AplicarDanoComDelay(float dano)
    {
        // Pequeno delay para garantir que tudo foi inicializado após o reset
        yield return new WaitForSeconds(0.2f);
        
        if (playerHealth != null && dano > 0)
        {
            Debug.Log("GameManager: Aplicando dano para ajustar saúde: " + dano);
            playerHealth.TakeDamage(dano);
            
            // Forçar atualização das barras após aplicar dano
            yield return new WaitForSeconds(0.1f);
            AtualizarBarras();
        }
    }
    
    // Atualiza todas as barras disponíveis
    public void AtualizarBarras()
    {
        // Verificar se temos pelo menos uma referência válida
        bool algumaBarraAtualizada = false;
        
        // Atualizar barras que estiverem disponíveis
        if (playerHealth != null && playerHealthBar != null)
        {
            AtualizarBarraVidaPlayer();
            algumaBarraAtualizada = true;
        }
            
        if (bossHealth != null && bossHealthBar != null)
        {
            AtualizarBarraVidaBoss();
            algumaBarraAtualizada = true;
        }
            
        if (lanternaScript != null && bateriaBar != null)
        {
            AtualizarBarraBateria();
            algumaBarraAtualizada = true;
        }
        
        // Se não conseguimos atualizar nenhuma barra, tentar encontrar referências novamente
        if (!algumaBarraAtualizada)
        {
            // Mas não fazer isso a cada frame para evitar impacto de desempenho
            if (Time.frameCount % 60 == 0) // A cada 60 frames (aproximadamente 1 segundo a 60 FPS)
            {
                Debug.LogWarning("GameManager: Nenhuma barra atualizada, tentando encontrar referências novamente");
                EncontrarReferencias();
            }
        }
    }
    
    private void AtualizarBarraVidaPlayer()
    {
        if (playerHealthBar != null && playerHealth != null)
        {
            // Calcular o valor de preenchimento com proteção contra divisão por zero
            float fillValue = (playerMaxHealth > 0) ? 
                (playerHealth.currentHealth / playerMaxHealth) : 0;
                
            // Clampar o valor entre 0 e 1 para segurança
            playerHealthBar.fillAmount = Mathf.Clamp01(fillValue);
            
            // Debug para verificar valor
            if (playerHealthBar.fillAmount <= 0)
                Debug.LogWarning("GameManager: Barra de vida zerada - health atual: " + playerHealth.currentHealth);
            else if (playerHealthBar.fillAmount >= 1)
                Debug.Log("GameManager: Barra de vida cheia - health atual: " + playerHealth.currentHealth);
        }
    }
    
    private void AtualizarBarraVidaBoss()
    {
        if (bossHealthBar != null && bossHealth != null)
        {
            // Calcular o valor de preenchimento com proteção contra divisão por zero
            float fillValue = (bossMaxHealth > 0) ? 
                ((float)bossHealth.health / bossMaxHealth) : 0;
                
            // Clampar o valor entre 0 e 1 para segurança
            bossHealthBar.fillAmount = Mathf.Clamp01(fillValue);
        }
    }
    
    public void AtualizarBarraBateria()
    {
        if (bateriaBar != null && lanternaScript != null)
        {
            try
            {
                // Usa o valor real da bateria, não o visual
                float min = lanternaScript.GetValorMinimoBateria();
                float max = lanternaScript.GetValorMaximoBateria();
                float atual = lanternaScript.GetValorRealBateria();
                
                // Proteção contra divisão por zero
                float percentual = (max > min) ? 
                    ((atual - min) / (max - min)) : 0;
                    
                bateriaBar.fillAmount = Mathf.Clamp01(percentual);
                
                // Debug
                if (bateriaBar.fillAmount >= 0.99f)
                    Debug.Log("GameManager: Barra de bateria cheia: " + percentual);
            }
            catch (System.Exception e)
            {
                Debug.LogError("GameManager: Erro ao atualizar barra de bateria: " + e.Message);
                // Se ocorrer erro, desabilita a referência para forçar nova busca
                lanternaScript = null;
                bateriaBar = null;
            }
        }
    }
    
    // Método para ativar/desativar a barra de vida do boss
    public void SetBossHealthBarActive(bool active)
    {
        if (bossHealthBar != null)
        {
            Transform parent = bossHealthBar.transform.parent;
            if (parent != null)
                parent.gameObject.SetActive(active);
            else
                bossHealthBar.gameObject.SetActive(active);
                
            Debug.Log("GameManager: Barra de vida do boss " + (active ? "ativada" : "desativada"));
        }
    }

    public void AdicionarChaveElevador()
    {
        chavesElevadorColetadas++;
        Debug.Log("GameManager: Chaves de elevador coletadas: " + chavesElevadorColetadas);
    }
    
    // Método público para forçar um reset completo de todos os valores
    public void ResetarCompletamente()
    {
        Debug.Log("GameManager: Executando reset completo");
        
        // Restaurar saúde máxima
        playerSavedHealth = playerMaxHealth;
        
        // Resetar player se existir
        if (playerHealth != null)
        {
            playerHealth.ResetPlayer();
        }
        
        // Resetar bateria da lanterna
        ResetarBateriaLanterna();
        
        // Atualizar todas as barras
        AtualizarBarras();
        
        Debug.Log("GameManager: Reset completo finalizado");
    }
    
    // Função pública para forçar uma busca de referências
    public void ForcarBuscaReferencias()
    {
        Debug.Log("GameManager: Forçando busca de referências");
        EncontrarReferencias();
        AtualizarBarras();
    }
    
    private void OnDestroy()
    {
        // Cancelar inscrição nos eventos
        SceneManager.sceneLoaded -= OnSceneLoaded;
        PlayerHealth.OnPlayerDeath -= OnPlayerDeathOrReset;
    }
}