using UnityEngine;

public class ItemBateria : MonoBehaviour
{
    public float quantidadeRecarga = 3f; // Quanto recarregar da bateria
    public GameObject efeitoColeta; // Efeito visual para coleta
    public AudioClip somColeta; // Som para coleta
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        // Verificar se colidiu com o player
        if (other.CompareTag("Player"))
        {
            // Encontrar o script da lanterna
            Lanterna lanterna = FindObjectOfType<Lanterna>();
            
            if (lanterna != null)
            {
                // Recarregar a lanterna diretamente no script dela
                lanterna.RecarregarLanterna(quantidadeRecarga);
                
                // Efeitos visuais e sonoros
                if (efeitoColeta != null)
                {
                    Instantiate(efeitoColeta, transform.position, Quaternion.identity);
                }
                
                if (somColeta != null)
                {
                    AudioSource.PlayClipAtPoint(somColeta, transform.position);
                }
                
                // Destruir a bateria
                Destroy(gameObject);
            }
        }
    }
}
