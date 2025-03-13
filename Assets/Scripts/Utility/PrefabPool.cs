using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Utility class for creating PrefabPools, a data structure which holds copies of a GameObject.
/// Rather than Instantiating and Destroying copies repeatedly, we keep a pool of inactive
/// objects and reactivate them when they're needed.
/// 
/// INITIALIZATION MUST HAPPEN ELSEWHERE.
/// </summary>
public class PrefabPool
{
    // Public Variables ===========================================================================

    // Read-only accessor for _numActive.
    public int NumActive => _numActive;
    // The list storing our pool of objects.
    public readonly List<GameObject> Pool = new();

    // Misc Internal Variables ====================================================================

    // The prefab spawned when we need to add another object.
    private GameObject _prefab;
    // The transform that we spawn all objects under.
    private Transform _spawnParent;
    // The number of objects in our pool currently active.
    private int _numActive;

    // Initializers ===============================================================================

    public PrefabPool(GameObject prefab, Transform spawnParent, int initialCapacity=0)
    {
        _prefab = prefab;
        _spawnParent = spawnParent;

        // Fill in the pool to the intial capacity.
        for (int i=0; i<initialCapacity; i++) {
            GameObject obj = GameObject.Instantiate(_prefab, _spawnParent);
            obj.SetActive(false);
            Pool.Add(obj);
        }
    }

    // Interface Methods ==========================================================================

    public GameObject Request()
    {
        // Requests an inactive object from our pool, or makes one if it
        // doesn't exist.
        // ================

        // Find the first inactive object in our pool.
        GameObject obj = Pool.Find(x => !x.activeInHierarchy);
        
        if (obj != null) {
            // If it exists, set it active.
            obj.SetActive(true);
        } else {
            // If none are inactive, make a new one and return it.
            obj = GameObject.Instantiate(_prefab, _spawnParent);
            Pool.Add(obj);
        }

        _numActive++;
        return obj;
    }

    public void Deactivate(GameObject obj)
    {
        // Deactivates an object in our pool.
        // ================

        if (!Pool.Contains(obj)) {
            Debug.LogError("PrefabPool Error: Deactivate failed. Object was not in pool.\n"
                          +"Try using object.SetActive(false)", obj);
        } else {
            obj.SetActive(false);
        }

        _numActive--;
    }

    public void DeactivateAll()
    {
        // Deactivates all objects in our pool.
        // ================

        foreach (GameObject obj in Pool) {
            obj.SetActive(false);
        }

        _numActive = 0;
    }
}