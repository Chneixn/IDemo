using System;
using System.Collections;
using System.Threading.Tasks;
using UnityEngine;
using UnityUtils;

namespace System.SceneManagement
{
    public class SceneLoader : MonoBehaviour
    {
        [SerializeField] private LoadingCanvas loadingCanvas;
        [SerializeField] private FadeCanvas fadeCanvas;
        [SerializeField] private float fadeDuration = 0.5f;
        [SerializeField] private float forceWaitTime = 1f;
        [SerializeField] private SceneGroup[] sceneGroups;

        [SerializeField] private float loadingProgress;
        [SerializeField] bool isLoading = false;

        public readonly SceneGroupManager manager = new();

        async void Start()
        {
            await LoadSceneGroup(0);
        }

        private void Update()
        {
            if (isLoading)
            {
                if (!fadeCanvas.IsDone) return;
                else EnableLoadingCanvas();
                if (!loadingCanvas.IsDone) loadingCanvas.SetTargetPercent(loadingProgress);
                else
                {
                    EnableLoadingCanvas(false);
                    DisableFadeCanvas();
                    isLoading = false;
                }
            }
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
            EnableFadeCanvas();
            isLoading = true;
            LoadingProgress progress = new();
            progress.Progressed += p => loadingProgress = p;

            await manager.LoadScenes(sceneGroups[index], progress);
        }

        private void EnableFadeCanvas()
        {
            fadeCanvas.gameObject.SetActive(true);
            fadeCanvas.FadeIn(fadeDuration);
            PlayerManager.Instance.CharacterControl.ChangeMovementState(PlayerManager.Instance.CharacterControl.freeze);
        }

        private void DisableFadeCanvas()
        {
            fadeCanvas.gameObject.SetActive(true);
            fadeCanvas.FadeOut(fadeDuration);
            PlayerManager.Instance.CharacterControl.ChangeMovementState(PlayerManager.Instance.CharacterControl.idle);
        }

        /// <summary>
        /// 启用/禁用加载画布
        /// </summary>
        /// <param name="enable"></param>
        private void EnableLoadingCanvas(bool enable = true)
        {
            loadingCanvas.gameObject.SetActiveSafe(enable);
        }


#if UNITY_EDITOR
        [ContextMenu("加载第二个场景组")]
        public async Task LoadNextSceneGroup()
        {
            await LoadSceneGroup(1);
        }
    }
#endif

    public class LoadingProgress : IProgress<float>
    {
        public event Action<float> Progressed;

        const float ratio = 1f;
        public void Report(float value)
        {
            Progressed.Invoke(value / ratio);
        }
    }
}