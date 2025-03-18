using UnityEngine;
using UnityGameObjectPool;

public class TerrainScanner : MonoBehaviour
{
    [SerializeField] private IPoolableParticleSystem _terrainScannerPrefab;
    [Tooltip("持续时间")][SerializeField] private float duration = 10f;
    [Tooltip("范围大小")][SerializeField] private float size = 500f;

    /// <summary>
    /// 生成地形扫描器
    /// </summary>
    public void SpawnTerrainScanner()
    {
        var scanner = GameObjectPoolManager.GetItem<IPoolableParticleSystem>(_terrainScannerPrefab, transform.position, Quaternion.identity);
        ParticleSystem particle = scanner.particle;

        // 设置粒子系统参数时, 获取的 module 只是一个接口, 其始终对应着其 ParticleSystem, 而无需赋值回去
        var main = particle.main;
        main.startLifetime = duration;
        main.startSize = size;
        particle.Play();

        TimerManager.CreateTimeOut(duration + 0.1f, () => GameObjectPoolManager.RecycleItem(scanner));
    }

    public void SpawnTerrainScanner(Transform spawnPosition)
    {
        var scanner = GameObjectPoolManager.GetItem<IPoolableParticleSystem>(_terrainScannerPrefab, spawnPosition.position, spawnPosition.rotation);
        ParticleSystem particleSystem = scanner.particle;

        var main = particleSystem.main;
        main.startLifetime = duration;
        main.startSize = size;
        particleSystem.Play();

        TimerManager.CreateTimeOut(duration + 0.1f, () => GameObjectPoolManager.RecycleItem(scanner));
    }
}
