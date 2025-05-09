using UnityEngine;

public class DocumentoColetavel : MonoBehaviour
{
    public Sprite imagemDoDocumento;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PainelDocumentoController painel = FindObjectOfType<PainelDocumentoController>();
            painel.MostrarDocumento(imagemDoDocumento);
            //Aplicar lógica do contador dos documentos aqui
            Destroy(gameObject);
        }
    }
}
