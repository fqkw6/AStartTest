using UnityEngine;
using System.Collections.Generic;

public class PoolManager : Singleton<PoolManager>
{
    private Dictionary<string, GameObject> _allPrefabs = new Dictionary<string, GameObject>();
    private Dictionary<string, List<GameObject>> _allCacheObjects = new Dictionary<string, List<GameObject>>();
    private GameObject _pool = null;

    private void Awake()
    {
        _pool = new GameObject("Pool");
        _pool.transform.position = Vector3.zero;
    }

    public GameObject GetObject(string prefabPath)
    {
        GameObject goPrefab;
        if (!_allPrefabs.TryGetValue(prefabPath, out goPrefab)) {
            goPrefab = Resources.Load<GameObject>(prefabPath);
            _allPrefabs[prefabPath] = goPrefab;
        }
        
        List<GameObject> lstCaches;
        if (_allCacheObjects.TryGetValue(prefabPath, out lstCaches)) {
            if (lstCaches.Count > 0) {
                GameObject go = lstCaches[0];
                go.SetActive(true);
                lstCaches.RemoveAt(0);
                return go;
            }
        }

        GameObject goNew = Object.Instantiate(goPrefab);
        goNew.transform.parent = _pool.transform;
        return goNew;
    }

    public void ReleaseObject(string prefabPath, GameObject go)
    {
        go.SetActive(false);

        List<GameObject> lstCaches;
        if (!_allCacheObjects.TryGetValue(prefabPath, out lstCaches)) {
            lstCaches = new List<GameObject>();
            _allCacheObjects[prefabPath] = lstCaches;
        }
        lstCaches.Add(go);
    }
}