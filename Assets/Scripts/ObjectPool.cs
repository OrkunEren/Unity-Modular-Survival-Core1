using UnityEngine;
using System.Collections.Generic;

public class ObjectPool : MonoBehaviour
{
    public static ObjectPool instance;

    [SerializeField] private GameObject pooledObjectPrefab;
    [SerializeField] private Transform parentTransform;
    [SerializeField] private int objectCount = 20;

    [SerializeField] List<GameObject> pooledObjects;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
                return;
        }

        instance = this;
    }

    private void Start()
    {
        pooledObjects = new List<GameObject>();

        for (int i = 0; i < objectCount; i++)
        {
            CreateGameObject();
        }

    }

    private GameObject CreateGameObject()
    {
        GameObject go = Instantiate(pooledObjectPrefab, parentTransform);
        go.SetActive(false);
        pooledObjects.Add(go);
        return go;
    }

    public GameObject GetPooledObject()
    {
        for (int i = 0; i < pooledObjects.Count; i++)
        {
            if (!pooledObjects[i].activeInHierarchy)
                return pooledObjects[i];
        }
        return CreateGameObject();
        
    }




}
