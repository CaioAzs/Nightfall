using UnityEngine;
using UnityEngine.UI;

public class BossHealthbar : MonoBehaviour
{
    [SerializeField] private Image totalHealthBar;    // Barra de fundo (opcional)
    [SerializeField] private Image currentHealthBar;  // Barra de vida atual
    
    [Header("Referencias")]
    [SerializeField] private BossHealth bossHealth;
    [SerializeField] private float maxHealth = 12f;   // Valor máximo de vida do boss
    
    private void Awake()
    {
        // Tenta encontrar o boss automaticamente se não for atribuído
        if (bossHealth == null)
        {
            GameObject boss = GameObject.FindGameObjectWithTag("Boss");
            if (boss != null)
            {
                bossHealth = boss.GetComponent<BossHealth>();
            }
        }
    }
    
    private void Start()
    {
        // Inicializa a barra de vida
        if (totalHealthBar != null)
        {
            totalHealthBar.fillAmount = 1.0f;
        }
        
        if (currentHealthBar != null)
        {
            currentHealthBar.fillAmount = 1.0f;
        }
        
        if (bossHealth != null)
        {
            // Atualiza o valor máximo de vida com base no boss
            maxHealth = bossHealth.health;
        }
    }
    
    private void Update()
    {
        if (bossHealth != null && currentHealthBar != null)
        {
            // Atualiza o preenchimento da barra com base na vida atual do boss
            float healthPercent = (float)bossHealth.health / maxHealth;
            currentHealthBar.fillAmount = healthPercent;
        }
    }
    
    // Método para ativar/desativar a barra de vida do boss
    public void SetActive(bool active)
    {
        gameObject.SetActive(active);
    }
}