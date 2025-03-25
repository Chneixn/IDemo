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
        [SerializeField] private bool isLog;
        public readonly SceneGroupManager Manager = new();

        private async void Start()
        {
            if (loadingCanvas == null)
            {
                var handle = Addressables.LoadAssetAsync<GameObject>("Loading Canvas");
                handle.Completed += (h) =>
                {
                    loadingCanvas = Instantiate(h.Result).GetComponent<LoadingCanvas>();
                    h.Release();
                };
                await UniTask.WaitUntil(() => handle.IsDone);
            }

            if (fadeCanvas == null)
            {
                var handle = Addressables.LoadAssetAsync<GameObject>("Fade Canvas");
                handle.Completed += (h) =>
                {
                    fadeCanvas = Instantiate(h.Result).GetComponent<FadeCanvas>();
                    h.Release();
                };
                await UniTask.WaitUntil(() => handle.IsDone);
            }
            // fire and forget 在同步方法中调用异步方法的一种方式
            LoadSceneGroup(0).Forget();
        }

        public async UniTaskVoid LoadSceneGroup(int index, bool unloadUnusedAssets = true)
        {
            // 索引检查
            if (index < 0 || index >= sceneGroups.Length)
            {
                if (isLog) Debug.LogError("Invalid scene group index" + index);
                return;
            }

            // 启用渐变和加载画布
            await EnableFadeCanvas(true);
            EnableLoadingCanvas();

            // 加载
            float forceWaitTimer = Time.time;
            var progress = new LoadingProgress(p =>
            {
                loadingCanvas.SetTargetPercent(p);
                loadingProgress = p;
            });
            await Manager.LoadScenes(sceneGroups[index], progress, unloadUnusedAssets);

            // 等待加载画布完成
            await UniTask.WaitUntil(() => loadingCanvas.IsDone);
            if (Time.time - forceWaitTimer < forceWaitTime)
                await UniTask.WaitForSeconds(forceWaitTime - (Time.time - forceWaitTimer));

            // 启用渐出画布, 关闭加载画布
            EnableLoadingCanvas(false);
            EnableFadeCanvas(false).Forget();
        }

        private async UniTask EnableFadeCanvas(bool enable = true)
        {
            if (isLog) Debug.Log("开始淡入");
            fadeCanvas.gameObject.SetActive(true);
            if (enable) fadeCanvas.FadeIn(fadeDuration);
            else fadeCanvas.FadeOut(fadeDuration);
            await UniTask.WaitUntil(() => fadeCanvas.IsDone);
        }

        private void EnableLoadingCanvas(bool enable = true)
        {
            if (isLog)
                if (enable) Debug.Log("开始加载动画");
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