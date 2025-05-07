using UnityEngine;
using UnityEngine.UI;

public class Healthbar : MonoBehaviour
{
    // Mais simples: pegue uma referência direta ao GameObject do Player
    [SerializeField] private GameObject playerObject;
    [SerializeField] private Image totalhealthBar;
    [SerializeField] private Image currenthealthBar;
    [SerializeField] private float maxHealth = 4f;
    
    private PlayerHealth playerHealth;

    private void Start()
    {
        // Verificação de segurança para as imagens
        if (totalhealthBar == null || currenthealthBar == null)
        {
            Debug.LogError("Referências de Image não atribuídas no Healthbar. Atribua-as no Inspector.");
            enabled = false;
            return;
        }
        
        // Inicializa a barra de saúde total
        totalhealthBar.fillAmount = 1f;
        
        // Obtém o objeto do player se não atribuído
        if (playerObject == null)
        {
            playerObject = GameObject.FindGameObjectWithTag("Player");
            
            if (playerObject == null)
            {
                Debug.LogError("Objeto do player não encontrado! Atribua-o manualmente ou adicione a tag 'Player'.");
                enabled = false;
                return;
            }
        }
        
        // Obtém o componente PlayerHealth
        playerHealth = playerObject.GetComponent<PlayerHealth>();
        
        // Tenta nos filhos se não encontrar no objeto principal
        if (playerHealth == null)
        {
            playerHealth = playerObject.GetComponentInChildren<PlayerHealth>();
        }
        
        // Verificação final
        if (playerHealth == null)
        {
            Debug.LogError("Componente PlayerHealth não encontrado no player! Verifique se o componente foi adicionado.");
            enabled = false;
        }
    }
    
    private void Update()
    {
        if (playerHealth != null)
        {
            UpdateHealthBar();
        }
    }

    private void UpdateHealthBar()
    {
        currenthealthBar.fillAmount = playerHealth.currentHealth / maxHealth;
    }
}