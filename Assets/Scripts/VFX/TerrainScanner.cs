using UnityEngine;

public class TerrainScanner : MonoBehaviour
{
    [SerializeField] private GameObject _terrainScannerPrefab;
    [Tooltip("持续时间")] [SerializeField] private float _duration = 10f;
    [Tooltip("范围大小")] [SerializeField] private float _size = 500f;

    /// <summary>
    /// 生成地形扫描器
    /// </summary>
    public void SpawnTerrainScanner()
    {
        GameObject _scanner = GameObjectPoolManager.SpawnObject(_terrainScannerPrefab, transform.position, Quaternion.identity);
        ParticleSystem particleSystem = _scanner.GetComponentInChildren<ParticleSystem>();

        if (particleSystem != null)
        {
            var main = particleSystem.main;
            main.startLifetime = _duration;
            main.startSize = _size;
            particleSystem.Play();
        }
        else
            Debug.LogError($"ParticleSystem not found! {gameObject}");

        GameObjectPoolManager.DelyReturnToPoolBySeconds(_scanner, _duration + 1f);
    }

    public void SpawnTerrainScanner(Transform spawnPosition)
    {
        GameObject _scanner = GameObjectPoolManager.SpawnObject(_terrainScannerPrefab, spawnPosition.position, Quaternion.identity);
        ParticleSystem particleSystem = _scanner.GetComponentInChildren<ParticleSystem>();

        if (particleSystem != null)
        {
            var main = particleSystem.main;
            main.startLifetime = _duration;
            main.startSize = _size;
            particleSystem.Play();
        }
        else
            Debug.LogError($"ParticleSystem not found! {gameObject}");

        GameObjectPoolManager.DelyReturnToPoolBySeconds(_scanner, _duration + 1f);
    }
}
