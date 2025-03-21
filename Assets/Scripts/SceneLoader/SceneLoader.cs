using System;
using System.Collections;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityUtils;

namespace System.SceneManagement
{
    public class SceneLoader : PersistentSingleton<SceneLoader>
    {
        [SerializeField] private LoadingCanvas loadingCanvas;
        [SerializeField] private FadeCanvas fadeCanvas;
        [SerializeField] private float fadeDuration = 0.5f;
        [SerializeField] private float forceWaitTime = 1f;
        [SerializeField] private SceneGroup[] sceneGroups;

        [SerializeField] private float loadingProgress;
        public readonly SceneGroupManager Manager = new();

        private void Start()
        {
            if (loadingCanvas == null){
                Addressables.LoadAssetAsync<GameObject>("Loading Canvas").Completed+=(handle) =>
                {
                    loadingCanvas = Instantiate(handle.Result).GetComponent<LoadingCanvas>();
                    handle.Release();
                };
            }

            if (fadeCanvas == null){
                Addressables.LoadAssetAsync<GameObject>("Fade Canvas").Completed+=(handle) =>
                {
                    fadeCanvas = Instantiate(handle.Result).GetComponent<FadeCanvas>();
                    handle.Release();
                };
            }
            // fire and forget 在同步方法中调用异步方法的一种方式
            LoadSceneGroup(0).Forget();
        }

        public async UniTaskVoid LoadSceneGroup(int index, bool unloadUnusedAssets = true)
        {
            if (index < 0 || index >= sceneGroups.Length)
            {
                Debug.LogError("Invalid scene group index" + index);
                return;
            }
            EnableFadeCanvas();
            await UniTask.WaitUntil(() => fadeCanvas.IsDone);
            EnableLoadingCanvas();
            float forceWaitTimer = Time.time;
            var progress = new LoadingProgress(p =>
            {
                loadingCanvas.SetTargetPercent(p);
                loadingProgress = p;
            });
            await Manager.LoadScenes(sceneGroups[index], progress, unloadUnusedAssets);
            await UniTask.WaitUntil(() => loadingCanvas.IsDone);
            if (Time.time - forceWaitTimer < forceWaitTime)
                await UniTask.WaitForSeconds(forceWaitTime - Time.time - forceWaitTimer);
            EnableLoadingCanvas(false);
            DisableFadeCanvas();
        }

        private void EnableFadeCanvas()
        {
            Debug.Log("开始淡入");
            fadeCanvas.gameObject.SetActive(true);
            fadeCanvas.FadeIn(fadeDuration);
        }

        private void DisableFadeCanvas()
        {
            Debug.Log("开始淡出");
            fadeCanvas.gameObject.SetActive(true);
            fadeCanvas.FadeOut(fadeDuration);
        }

        private void EnableLoadingCanvas(bool enable = true)
        {
            if (enable)
                Debug.Log("开始加载动画");
            else Debug.Log("结束加载动画");
            loadingCanvas.gameObject.SetActiveSafe(enable);
        }


#if UNITY_EDITOR
        [ContextMenu("加载第二个场景组")]
        public void LoadNextSceneGroup()
        {
            LoadSceneGroup(1).Forget();
        }
    }
#endif

    public class LoadingProgress : IProgress<float>
    {
        public LoadingProgress(Action<float> report)
        {
            Progressed += report;
        }

        public Action<float> Progressed;

        const float ratio = 1f;
        public void Report(float value)
        {
            Progressed.Invoke(value / ratio);
        }
    }
}