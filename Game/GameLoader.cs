using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class GameLoader : Singleton<GameLoader>
{
    public UnityEvent OnLoadStart;
    public UnityEvent OnLoadFinish;
    public bool isLoading {get; protected set;}
    public UIAnimator loadingUIAnimator;
    public float loadingProgress {get; protected set;}

    [Header("Minimum Time")] 
    public float startDelay = 1f;
    public float finishDelay = 1f;
    
    public string currentScene => SceneManager.GetActiveScene().name;
    
    public virtual void Load(string scene)
    {
        StartCoroutine(LoadRoutine(scene));
    }

    protected virtual IEnumerator LoadRoutine(string scene)
    {
        OnLoadStart?.Invoke();
        isLoading = true;
        loadingUIAnimator.SetActive(true);
        loadingUIAnimator.Show();
        
        yield return new WaitForSeconds(startDelay);
        
        var operation = SceneManager.LoadSceneAsync(scene);
        loadingProgress = 0;

        while (!operation.isDone)
        {
            loadingProgress = operation.progress;
            yield return null;
        }

        loadingProgress = 1;
        
        yield return new WaitForSeconds(finishDelay);

        isLoading = false;
        loadingUIAnimator.Hide();
        OnLoadFinish?.Invoke();
    }
}