using UnityEngine;

public class KeyElevator : MonoBehaviour
{

    [SerializeField] private float rotationSpeed = 90f; // graus por segundo

    private void Update()
    {
        // Rotaciona o item continuamente no eixo Y
        transform.Rotate(0f, rotationSpeed * Time.deltaTime, 0f);
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            GameManager.Instance.AdicionarChaveElevador();
            Destroy(gameObject);
        }
    }
}
