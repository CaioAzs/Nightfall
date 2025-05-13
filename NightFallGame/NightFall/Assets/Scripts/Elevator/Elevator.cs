using UnityEngine;

public class Elevator : MonoBehaviour
{
    private SceneController sceneController;

    private void Start()
    {
        // Encontra o SceneController na cena
        sceneController = FindObjectOfType<SceneController>();

        if (sceneController == null)
        {
            Debug.LogError("SceneController não encontrado na cena!");
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && sceneController != null)
        {
            int keyCount = GameManager.Instance.chavesElevadorColetadas; // ou outro nome se você usar diferente

            if (keyCount >= 1)
            {
                sceneController.LoadCena2();
            }
            else
            {
                Debug.Log("Você precisa de uma chave para usar o elevador.");
            }
        }
    }
}
