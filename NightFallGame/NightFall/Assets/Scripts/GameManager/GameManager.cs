using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Rendering.Universal; // Para o Light2D

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
    
    // Variável para persistência entre cenas
    [HideInInspector] public float playerSavedHealth;
    
    private void Awake()
    {
        // Implementação do padrão Singleton
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            
            // Inicializar valores padrão
            playerSavedHealth = 0; // Será definido no Start
            
            // Registrar para o evento de mudança de cena
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Buscar referências na nova cena
        EncontrarReferencias();
        
        // Aplicar valores do player
        AplicarValoresSalvosPlayer();
    }
    
    private void Start()
    {
        EncontrarReferencias();
        
        // Se não houver valor salvo de saúde do player, inicializar com o valor atual
        if (playerSavedHealth <= 0 && playerHealth != null)
        {
            playerSavedHealth = playerHealth.currentHealth;
        }
        
        // Atualizar barras
        AtualizarBarras();
    }
    
    private void Update()
    {
        // Atualizar todas as barras que estiverem disponíveis
        AtualizarBarras();
        
        // Salvar a saúde atual do player para persistência
        if (playerHealth != null)
        {
            playerSavedHealth = playerHealth.currentHealth;
        }
    }
    
    private void EncontrarReferencias()
    {
        // Buscar todas as referências disponíveis na cena atual
        
        // Player (sempre buscar)
        if (playerHealth == null)
            playerHealth = FindObjectOfType<PlayerHealth>();
            
        if (playerHealthBar == null)
        {
            var healthbarObj = GameObject.FindWithTag("PlayerHealthBar");
            if (healthbarObj != null)
                playerHealthBar = healthbarObj.GetComponent<Image>();
        }
        
        // Lanterna
        if (lanternaScript == null)
            lanternaScript = FindObjectOfType<Lanterna>();
            
        if (bateriaBar == null)
        {
            var bateriaUI = GameObject.FindWithTag("BateriaUI");
            if (bateriaUI != null)
                bateriaBar = bateriaUI.GetComponent<Image>();
        }
        
        // Boss
        if (bossHealth == null)
            bossHealth = FindObjectOfType<BossHealth>();
            
        if (bossHealthBar == null)
        {
            var bossHealthbarObj = GameObject.FindWithTag("BossHealthBar");
            if (bossHealthbarObj != null)
                bossHealthBar = bossHealthbarObj.GetComponent<Image>();
        }
    }
    
    private void AplicarValoresSalvosPlayer()
    {
        // Como não podemos definir currentHealth diretamente, usamos ResetPlayer e TakeDamage
        if (playerHealth != null && playerSavedHealth > 0)
        {
            // Primeiro, resetamos para a saúde máxima
            playerHealth.ResetPlayer();
            
            // Em seguida, calculamos quanto dano deve ser aplicado para chegar no valor salvo
            float danoNecessario = playerMaxHealth - playerSavedHealth;
            if (danoNecessario > 0)
            {
                playerHealth.TakeDamage(danoNecessario);
            }
        }
    }
    
    // Atualiza todas as barras disponíveis
    public void AtualizarBarras()
    {
        // Atualizar barras que estiverem disponíveis
        if (playerHealth != null && playerHealthBar != null)
            AtualizarBarraVidaPlayer();
            
        if (bossHealth != null && bossHealthBar != null)
            AtualizarBarraVidaBoss();
            
        if (lanternaScript != null && bateriaBar != null)
            AtualizarBarraBateria();
    }
    
    private void AtualizarBarraVidaPlayer()
    {
        if (playerHealthBar != null && playerHealth != null)
        {
            // Apenas lemos o currentHealth, não tentamos definir
            playerHealthBar.fillAmount = playerHealth.currentHealth / playerMaxHealth;
        }
    }
    
    private void AtualizarBarraVidaBoss()
    {
        if (bossHealthBar != null && bossHealth != null)
        {
            bossHealthBar.fillAmount = (float)bossHealth.health / bossMaxHealth;
        }
    }
    
    public void AtualizarBarraBateria()
    {
        if (bateriaBar != null && lanternaScript != null)
        {
            // Usa o valor real da bateria, não o visual
            float min = lanternaScript.GetValorMinimoBateria();
            float max = lanternaScript.GetValorMaximoBateria();
            float atual = lanternaScript.GetValorRealBateria(); // Usa o valor real em vez do visual
            
            float percentual = (atual - min) / (max - min);
            bateriaBar.fillAmount = percentual;
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
        }
    }
    
    private void OnDestroy()
    {
        // Cancelar inscrição no evento ao destruir o objeto
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}