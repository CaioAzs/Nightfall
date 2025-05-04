using UnityEngine;
using UnityEngine.UI;

public class Healthbar : MonoBehaviour
{
    [SerializeField] private Health playerHealth;
    [SerializeField] private Image totalhealthBar;
    [SerializeField] private Image currenthealthBar;
    [SerializeField] private float maxHealth = 4f;

    private void Start()
    {
        totalhealthBar.fillAmount = 1f;
        UpdateHealthBar();
    }
    private void Update()
    {
        UpdateHealthBar();
    }

    private void UpdateHealthBar()
    {
        currenthealthBar.fillAmount = playerHealth.currentHealth / maxHealth;
        
    }

}