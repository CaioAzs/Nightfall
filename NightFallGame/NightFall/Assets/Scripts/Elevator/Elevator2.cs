using UnityEngine;

public class Elevator2 : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            int keyCount = GameManager.Instance.chavesElevadorColetadas; // ou outro nome se você usar diferente

            if (keyCount >= 2)
            {
                SceneController.instance.LoadElevador2_3();
            }
            else
            {
                Debug.Log("Você precisa de uma chave para usar o elevador.");
            }
        }
    }
}
