using System;
using System.Threading.Tasks;
using UnityEngine;

namespace System.SceneManagement
{
    public class SceneLoader : MonoBehaviour
    {
        [SerializeField] private LoadingCanvas loadingPanel;
        [SerializeField] private Canvas loadingCanvas;
        [SerializeField] private FadeCanvas fadeCanvas;
        [SerializeField] private float fadeDuration = 0.5f;
        [SerializeField] private SceneGroup[] sceneGroups;

        private float loadingProgress;
        bool isLoading;

        public readonly SceneGroupManager manager = new();

        async void Start()
        {
            await LoadSceneGroup(0);
        }

        /// <summary>
        /// 加载新场景组
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        private async Task LoadSceneGroup(int index)
        {
            if (index < 0 || index >= sceneGroups.Length)
            {
                Debug.LogError("Invalid scene group index" + index);
                return;
            }
            fadeCanvas.FadeIn(fadeDuration);
            await Task.Delay((int)(fadeDuration * 1000));
            EnableLoadingCanvas();
            LoadingProgress progress = new();
            progress.Progressed += p =>
            {
                loadingProgress = p;
                loadingPanel.SetTargetPercent(loadingProgress);
            };

            await manager.LoadScenes(sceneGroups[index], progress);
            EnableLoadingCanvas(false);
            fadeCanvas.FadeOut(fadeDuration);
        }

        /// <summary>
        /// 启用/禁用加载画布
        /// </summary>
        /// <param name="enable"></param>
        private void EnableLoadingCanvas(bool enable = true)
        {
            isLoading = enable;
            loadingCanvas.gameObject.SetActive(enable);
            fadeCanvas.gameObject.SetActive(enable);
        }
    }

    public class LoadingProgress : IProgress<float>
    {
        public event Action<float> Progressed;

        const float ratio = 1f;
        public void Report(float value)
        {
            Progressed?.Invoke(value / ratio);
        }
    }
}