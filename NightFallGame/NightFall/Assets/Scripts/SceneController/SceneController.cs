using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class SceneController : MonoBehaviour
{
    public static SceneController instance;
    [SerializeField] Animator transitionAnim;
    
    [Header("Configurações")]
    [SerializeField] private float fadeOutTime = 1f;  // Tempo para fade out (antes de carregar a cena)
    [SerializeField] private float fadeInTime = 1f;   // Tempo para fade in (após carregar a cena)
    [SerializeField] private float pauseTime = 0.5f;  // Tempo adicional de espera na tela preta

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            
            // Tentar encontrar o animator no início
            if (transitionAnim == null)
            {
                Transform child = transform.Find("SceneTransition");
                if (child != null)
                {
                    transitionAnim = child.GetComponent<Animator>();
                }
            }
        }
        else
        {
            Destroy(gameObject); // Isso evita duplicações ao voltar pro menu
        }
    }

    IEnumerator LoadLevel(int sceneIndex)
    {
        // Primeiro, garantir que temos o animator
        FindAnimator();
        
        if (transitionAnim != null)
        {
            // 1. Iniciar a animação de fade out (escurecer a tela)
            transitionAnim.SetTrigger("End");
            Debug.Log("Iniciando fade out para cena: " + sceneIndex);
            
            // 2. Aguardar o tempo de fade out
            yield return new WaitForSeconds(fadeOutTime);
            
            // 3. Pausa opcional na tela preta para uma transição mais dramática
            yield return new WaitForSeconds(pauseTime);
        }
        else
        {
            Debug.LogWarning("Animator de transição não encontrado! Transição será instantânea.");
        }
        
        // 4. Iniciar o carregamento da cena
        Debug.Log("Carregando cena: " + sceneIndex);
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneIndex);
        
        // 5. Aguardar o carregamento completo
        while (!asyncLoad.isDone)
        {
            // Você pode adicionar lógica de barra de progresso aqui se quiser
            float progress = Mathf.Clamp01(asyncLoad.progress / 0.9f);
            Debug.Log("Progresso de carregamento: " + (progress * 100) + "%");
            yield return null;
        }
        
        // 6. Aguardar um frame adicional para garantir que tudo está inicializado
        yield return new WaitForEndOfFrame();
        
        // 7. Encontrar o animator novamente (pode ter sido perdido na troca de cena)
        FindAnimator();
        
        if (transitionAnim != null)
        {
            // 8. Iniciar a animação de fade in (clarear a tela)
            Debug.Log("Iniciando fade in na cena: " + sceneIndex);
            transitionAnim.SetTrigger("Start");
            
            // 9. Aguardar o término da animação de fade in
            yield return new WaitForSeconds(fadeInTime);
        }
        
        Debug.Log("Transição para a cena " + sceneIndex + " concluída");
    }
    
    private void FindAnimator()
    {
        if (transitionAnim == null)
        {
            // Primeiro verifica se é um filho direto
            Transform child = transform.Find("SceneTransition");
            if (child != null)
            {
                transitionAnim = child.GetComponent<Animator>();
                return;
            }
            
            // Se não for filho, busca na cena
            GameObject transObj = GameObject.Find("SceneTransition");
            if (transObj != null)
            {
                transitionAnim = transObj.GetComponent<Animator>();
                Debug.Log("Animator de transição encontrado na cena");
            }
            else
            {
                Debug.LogWarning("SceneTransition não encontrado! Verifique se o objeto existe na cena.");
            }
        }
    }
    // Métodos para carregar cenas específicas
    // Todos usando a mesma lógica de transição

    public void LoadMenu(){
        StartCoroutine(LoadLevel(0));
    }
    public void LoadHistoryScene()
    {
        StartCoroutine(LoadLevel(1));
    }

    public void LoadLevel1()
    {
        StartCoroutine(LoadLevel(2));
    }
    
    public void LoadElevador1_2()
    {
        StartCoroutine(LoadLevel(3));
    }

    public void LoadLevel2()
    {
        StartCoroutine(LoadLevel(4));
    }
    
    public void LoadElevador2_3()
    {
        StartCoroutine(LoadLevel(5));
    }

    public void LoadLevel3()
    {
        StartCoroutine(LoadLevel(6));
    }

    public void LoadDialogueWinGame()
    {
        StartCoroutine(LoadLevel(7));
    }

    public void LoadDialogueLoseGame()
    {
        MusicaAmbiente musicaAmbiente = FindObjectOfType<MusicaAmbiente>();
        if (musicaAmbiente != null)
        {
            musicaAmbiente.RetomarMusica();
        }
        StartCoroutine(LoadLevel(8));
    }

    public void LoadYouWin(){
        StartCoroutine(LoadLevel(9));
    }

    public void LoadYouLose(){
        StartCoroutine(LoadLevel(10));
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}