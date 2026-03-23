using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;
using Cysharp.Threading.Tasks;
using System;

public class ObjectPool : MonoSingleton<ObjectPool>
{
    private Dictionary<GameObject, ObjectPool<GameObject>> poolDictionary = new Dictionary<GameObject, ObjectPool<GameObject>>();

    [Header("Settings")]
    [SerializeField] private bool collectionCheck = true;
    [SerializeField] private int defaultCapacity = 10;
    [SerializeField] private int maxPoolSize = 100;

    public GameObject GetObject(GameObject prefab, Vector3 position, Quaternion rotation)
    {
        if (!poolDictionary.ContainsKey(prefab))
        {
            ObjectPool<GameObject> newPool = CreateNewPool(prefab);
            poolDictionary[prefab] = newPool;
        }

        GameObject instance = poolDictionary[prefab].Get();

        // Detach from pool parent so Rigidbody physics works correctly
        instance.transform.SetParent(null);
        instance.transform.SetPositionAndRotation(position, rotation);

        // Sync Rigidbody internal position with transform position
        // Without this, the physics engine keeps the RB at its old position
        if (instance.TryGetComponent<Rigidbody>(out var rb))
        {
            rb.position = position;
            rb.rotation = rotation;
        }

        return instance;
    }

    public GameObject GetObject(GameObject prefab, Transform target)
    {
        return GetObject(prefab, target.position, target.rotation);
    }

    public GameObject GetObject(GameObject prefab, Vector3 position)
    {
        return GetObject(prefab, position, Quaternion.identity);
    }

    public void ReturnObject(GameObject instance, float delay = 0)
    {
        if (instance == null || instance.activeSelf == false)
            return;

        if (delay > 0)
        {
            DelayReturn(instance, delay).Forget();
        }
        else
        {
            if (instance.TryGetComponent<PooledObject>(out var pooledObj))
            {
                pooledObj.Release();
            }
            else
            {
                Debug.LogWarning($"[ObjectPool] {instance.name} missing PooledObject component. Destroying.");
                Destroy(instance);
            }
        }
    }

    private async UniTaskVoid DelayReturn(GameObject instance, float delay)
    {
        await UniTask.Delay(TimeSpan.FromSeconds(delay), cancellationToken: this.GetCancellationTokenOnDestroy());

        if (instance != null)
            ReturnObject(instance);
    }

    private ObjectPool<GameObject> CreateNewPool(GameObject prefab)
    {
        ObjectPool<GameObject> newlyCreatedPool = null;

        newlyCreatedPool = new ObjectPool<GameObject>(
            createFunc: () =>
            {
                // Create under pool parent (will be detached on Get)
                GameObject newObj = Instantiate(prefab, transform);
                newObj.SetActive(false);
                PooledObject pooledScript = newObj.TryGetComponent<PooledObject>(out var p) ? p : newObj.AddComponent<PooledObject>();
                pooledScript.SetPool(newlyCreatedPool);
                return newObj;
            },
            actionOnGet: (obj) =>
            {
                obj.SetActive(true);
            },
            actionOnRelease: (obj) =>
            {
                // Reset Rigidbody state before pooling
                if (obj.TryGetComponent<Rigidbody>(out var rb))
                {
                    rb.linearVelocity = Vector3.zero;
                    rb.angularVelocity = Vector3.zero;
                    rb.isKinematic = false;
                }

                // Reset TrailRenderer to prevent ghost trails
                if (obj.TryGetComponent<TrailRenderer>(out var trail))
                {
                    trail.Clear();
                }

                obj.SetActive(false);

                // Re-parent back to pool for hierarchy organization
                obj.transform.SetParent(Instance.transform);
            },
            actionOnDestroy: (obj) =>
            {
                Destroy(obj);
            },
            collectionCheck: collectionCheck,
            defaultCapacity: defaultCapacity,
            maxSize: maxPoolSize
        );

        return newlyCreatedPool;
    }
}