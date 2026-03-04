using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

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
            poolDictionary[prefab] = CreateNewPool(prefab);
        }

        GameObject instance = poolDictionary[prefab].Get();
        instance.transform.SetPositionAndRotation(position, rotation);

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
        if (delay > 0)
        {
            StartCoroutine(DelayReturn(instance, delay));
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

    private IEnumerator DelayReturn(GameObject instance, float delay)
    {
        yield return new WaitForSeconds(delay);
        ReturnObject(instance);
    }

    private ObjectPool<GameObject> CreateNewPool(GameObject prefab)
    {
        return new ObjectPool<GameObject>(
            createFunc: () =>
            {
                GameObject newObj = Instantiate(prefab, transform);
                PooledObject pooledScript = newObj.AddComponent<PooledObject>();
                pooledScript.SetPool(poolDictionary[prefab]);
                return newObj;
            },
            actionOnGet: (obj) =>
            {
                obj.SetActive(true);
            },
            actionOnRelease: (obj) =>
            {
                obj.SetActive(false);
            },
            actionOnDestroy: (obj) =>
            {
                Destroy(obj);
            },
            collectionCheck: collectionCheck,
            defaultCapacity: defaultCapacity,
            maxSize: maxPoolSize
        );
    }
}