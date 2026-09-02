using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TransitionManager : MonoBehaviour
{
    public static TransitionManager Instance {get; private set;}

    [Header("Scene Transition References")]
    [SerializeField] private Transform sceneTransitionContainer;
    [SerializeField] private List<Transition> transitions = new();
    
    public  bool isTrasitioning { get; private set;}
    
    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
        }

        Instance = this;
        
        transitions.Clear();
        transitions = sceneTransitionContainer.GetComponentsInChildren<Transition>(true).ToList();
        
        DontDestroyOnLoad(gameObject);
    }

    private IEnumerator LoadSceneAsync(string sceneName, string transitionName)
    {
        Transition transition = transitions.First(t => t.name == transitionName);

        AsyncOperation scene = SceneManager.LoadSceneAsync(sceneName);
        scene.allowSceneActivation = false;

        yield return transition.TransitionIn();
        
        do
        {
            //progressBar.value = scene.progress;
            yield return null;
        } while (scene.progress < 0.9f);

        yield return new WaitForSeconds(1f);

        scene.allowSceneActivation = true;

        yield return null;

        yield return transition.TransitionOut();
    }

    private IEnumerator LoadEnvironmentAsync(string environmentName, string transitionName)
    {
        Transition transition = transitions.First(t => t.name == transitionName);
        
        yield return transition.TransitionIn();
        
        float progress = 0f;
        do
        {
            progress += Time.deltaTime;
            //progressBar.value = scene.progress;
            yield return null;
        } while (progress < 0.9f);

        yield return new WaitForSeconds(1f);
        
        Debug.Log($"[{name} (TransitionDialogueEnvironment)] Transition Environment: {environmentName} Transition: {transitionName}");
        GameEvents.OnChangeEnvironment.Invoke(environmentName);

        isTrasitioning = false;
        
        DialogueManager.Instance.ContinueDialogue();
        
        yield return transition.TransitionOut();
        
        if (DialogueManager.Instance.IsAllDoneDialogue())
            GameEvents.OnRequestOpenPanel.Invoke(PanelType.EndStory);
        
    }
    
    public void TransitionScene(string sceneName, string transitionName)
    {
        StartCoroutine(LoadSceneAsync(sceneName, transitionName));
    }

    public void TransitionDialogueEnvironment(string environmentName, string transitionName)
    {
        isTrasitioning = true;
        StartCoroutine(LoadEnvironmentAsync(environmentName, transitionName));
    }
}
