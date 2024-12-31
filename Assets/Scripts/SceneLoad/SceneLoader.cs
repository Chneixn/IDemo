using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.InputSystem;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.SceneManagement;

public enum SceneType
{
    Loaction, Menu
}

public class SceneLoader : MonoBehaviour
{
    [Header("场景渐变")]
    [SerializeField] private FadeEventSO fadeEventSO;
    [SerializeField] private float _fadeOutDuration;
    [SerializeField] private float _fadeInDuration;

    [Header("场景加载")]
    [SerializeField] private float _loadWaitTime;
    [SerializeField] private LoadingPanel loadingPanel;

    [Header("事件监听")]
    public SceneLoadEventSO loadEventSO;
    public GameSceneSO firstLoadScene;

    private GameSceneSO currentScene;
    private GameSceneSO sceneToLoad;
    private Vector3 positionToGo;
    private bool fadeScreen = false;
    private bool isLoading = false;

    private void Awake()
    {
        //firstLoadScene.sceneReference.LoadSceneAsync(LoadSceneMode.Additive);
        ////渐入
        //currentScene = firstLoadScene;
    }

    private void OnEnable()
    {
        loadEventSO.LoadRequestEvent += OnLoadRequestEvent;
    }

    private void OnDisable()
    {
        loadEventSO.LoadRequestEvent -= OnLoadRequestEvent;
    }

    private void OnLoadRequestEvent(GameSceneSO locationToLoad, Vector3 posTogo, bool fadeScreen)
    {
        if (isLoading) return;
        isLoading = true;

        sceneToLoad = locationToLoad;
        positionToGo = posTogo;
        this.fadeScreen = fadeScreen;

        StartCoroutine(UnLoadPreviousScene());
    }

    private IEnumerator UnLoadPreviousScene()
    {
        if (fadeScreen)
        {

        }

        yield return new WaitForSeconds(_fadeInDuration);

        currentScene.sceneReference.UnLoadScene();
        StartCoroutine(LoadScene());
        yield return null;
    }

    private IEnumerator LoadScene()
    {
        // 异步加载场景(如果场景资源没有下载，会自动下载)
        float startTime = Time.time;
        var loadingOption = Addressables.LoadSceneAsync(sceneToLoad.sceneReference, LoadSceneMode.Additive);

        //检测加载状态，当加载失败报错
        if (loadingOption.Status == AsyncOperationStatus.Failed)
        {
            Debug.LogError("场景加载异常: " + loadingOption.OperationException.ToString());
            yield break;
        }

        while (!loadingOption.IsDone)
        {
            // 进度（0~1）
            float percentComplete = loadingOption.GetDownloadStatus().Percent;
            loadingPanel.percentCompleted = percentComplete;

            if (percentComplete >= 0.99f)
            {
                //_slider.value = 1;
                Debug.Log("加载完成");
                if (sceneToLoad.mat_skyBox != null)
                {
                    RenderSettings.skybox = sceneToLoad.mat_skyBox;
                    DynamicGI.UpdateEnvironment();
                }

                //手动延长加载时间，防止加载界面展示时间过短
                if (Time.time - startTime >= _loadWaitTime)
                {
                    if (Keyboard.current.anyKey.wasPressedThisFrame)
                    {
                        //调用场景加载完成后方法
                        loadingPanel.gameObject.SetActive(false);
                        if (fadeScreen)
                        {
                            //TODO 渐出
                        }
                        isLoading = false;
                    }
                }
            }
        }

        yield break;
    }
}
