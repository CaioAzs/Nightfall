using UnityEngine;

public class ChangeSortingOrder : MonoBehaviour
{
    public SpriteRenderer playerRenderer;
    public SpriteRenderer galonRenderer;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerRenderer.sortingOrder = galonRenderer.sortingOrder + 1; // Player fica na frente
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerRenderer.sortingOrder = galonRenderer.sortingOrder - 1; // Player volta atrás
        }
    }
}
