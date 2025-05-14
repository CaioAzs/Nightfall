using UnityEngine;

public class Elevator : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            int keyCount = GameManager.Instance.chavesElevadorColetadas; // ou outro nome se você usar diferente

            if (keyCount >= 1)
            {
                SceneController.instance.LoadElevador1_2();
            }
            else
            {
                Debug.Log("Você precisa de uma chave para usar o elevador.");
            }
        }
    }
}
