using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using Eflatun.SceneReference;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;

namespace System.SceneManagement
{
    public class SceneGroupManager
    {
        public Action<string> OnSceneLoaded;
        public Action<string> OnSceneUnloaded;
        public Action OnSceneGroupLoaded;

        private readonly AsyncOperationHandleGroup handleGroup = new(10);

        private SceneGroup ActiveSceneGroup;

        /// <summary>
        /// 加载场景组
        /// </summary>
        /// <param name="group"></param>
        /// <param name="progress"></param>
        /// <param name="reloadDupScenes">是否重新加载存在的场景</param>
        /// <returns></returns>
        public async UniTask LoadScenes(SceneGroup group, IProgress<float> progress, bool reloadDupScenes = false, bool unloadUnusedAssets = true)
        {
            ActiveSceneGroup = group;
            var loadedScenes = new List<string>();

            // 等待卸载场景
            await UnLoadScenes(unloadUnusedAssets);

            // 获取当前加载的所有场景的名字
            int sceneCount = SceneManager.sceneCount;
            for (int i = 0; i < sceneCount; i++)
            {
                loadedScenes.Add(SceneManager.GetSceneAt(i).name);
            }

            int totalScenesToLoad = ActiveSceneGroup.Scenes.Count;
            // 将所有场景加载操作添加进异步操作组来查询整体加载场景进度
            var operationGroup = new AsyncOperationGroup(totalScenesToLoad);

            for (int i = 0; i < totalScenesToLoad; i++)
            {
                var sceneData = ActiveSceneGroup.Scenes[i];
                // 如果不重新加载存在的场景，并且该场景已经加载过了，则跳过
                if (!reloadDupScenes && loadedScenes.Contains(sceneData.Name)) continue;

                if (sceneData.Reference.State == SceneReferenceState.Regular)
                {// 异步加载build中的场景
                    var operation = SceneManager.LoadSceneAsync(sceneData.Reference.Path, LoadSceneMode.Additive);
                    operationGroup.Operations.Add(operation);
                }
                else if (sceneData.Reference.State == SceneReferenceState.Addressable)
                {// 异步加载addressable的场景
                    var sceneHandle = Addressables.LoadSceneAsync(sceneData.Reference.Path, LoadSceneMode.Additive);
                    handleGroup.Handles.Add(sceneHandle);
                }

                OnSceneLoaded?.Invoke(sceneData.Name);
            }

            // 等待所有场景异步加载完成, 并且每隔100ms更新进度
            do
            {
                progress?.Report((operationGroup.Progress + handleGroup.Progress) / 2);
                await UniTask.Delay(100);
            } while (!operationGroup.IsDone || !handleGroup.IsDone);

            progress?.Report(1);

            Scene activeScene = SceneManager.GetSceneByName(ActiveSceneGroup.FindSceneNameByType(SceneType.Activate));

            if (activeScene.IsValid())
            {
                SceneManager.SetActiveScene(activeScene);
            }

            OnSceneGroupLoaded?.Invoke();
        }
        public async UniTask UnLoadScenes(bool unloadUnusedAssets)
        {
            // 记录应当卸载的所有场景名
            List<string> unloadScenes = new();
            var activeScene = SceneManager.GetActiveScene().name;

            int sceneCount = SceneManager.sceneCount;
            for (int i = sceneCount - 1; i > 0; i--)
            {
                var sceneAt = SceneManager.GetSceneAt(i);
                // 如果场景没有加载则跳过
                if (!sceneAt.isLoaded) continue;

                // 如果是当前激活的场景或者是Bootstrapper场景则跳过
                var sceneName = sceneAt.name;
                if (sceneName.Equals(activeScene) || sceneName == "Bootstrapper") continue;

                // 如果是Addressable加载过的场景则跳过
                if (handleGroup.Handles.Any(h => h.IsValid() && h.Result.Scene.name == sceneName)) continue;

                unloadScenes.Add(sceneName);
            }

            // 使用异步操作组来查询卸载场景进度
            var operationGroup = new AsyncOperationGroup(unloadScenes.Count);
            foreach (var scene in unloadScenes)
            {
                var operation = SceneManager.UnloadSceneAsync(scene);
                if (operation == null) continue; // 当返回为空，表明项目中只剩下该场景，无法卸载

                operationGroup.Operations.Add(operation);

                OnSceneUnloaded?.Invoke(scene);
            }

            foreach (var handle in handleGroup.Handles)
            {
                if (handle.IsValid())
                {
                    await Addressables.UnloadSceneAsync(handle);
                }
            }
            handleGroup.Handles.Clear();

            while (!operationGroup.IsDone)
            {
                await UniTask.Delay(100);
            }

            // Optional: 卸载未使用的资源
            if (unloadUnusedAssets)
                await UnityEngine.Resources.UnloadUnusedAssets();
        }
    }

    /// <summary>
    /// Build中场景的异步操作句柄组
    /// </summary>
    public readonly struct AsyncOperationGroup
    {
        public readonly List<AsyncOperation> Operations;

        // 异步操作队列不为零时,返回所有异步操作进度的平均值,否则为1,表示已完成
        public float Progress => Operations.Count == 0 ? 1 : Operations.Average(o => o.progress);
        public bool IsDone => Operations.All(o => o.isDone);

        public AsyncOperationGroup(int initialCapacity)
        {
            Operations = new List<AsyncOperation>(initialCapacity);
        }

    }

    /// <summary>
    /// 基于Addressable的场景异步操作句柄组
    /// </summary>
    public readonly struct AsyncOperationHandleGroup
    {
        public readonly List<AsyncOperationHandle<SceneInstance>> Handles;

        // 异步操作队列不为零时,返回所有异步操作进度的平均值,否则为1,表示已完成
        public float Progress => Handles.Count == 0 ? 1 : Handles.Average(a => a.PercentComplete);
        public bool IsDone => Handles.Count == 0 || Handles.All(a => a.IsDone);

        public AsyncOperationHandleGroup(int initialCapacity)
        {
            Handles = new List<AsyncOperationHandle<SceneInstance>>(initialCapacity);
        }
    }
}

