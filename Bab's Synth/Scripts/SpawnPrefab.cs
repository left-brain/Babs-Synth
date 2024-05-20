using UnityEngine;

public class SpawnPrefab : MonoBehaviour
{
    public GameObject prefabToSpawn;
    public Transform spawnParent;

    public void Spawn()
    {
        if (prefabToSpawn != null && spawnParent != null)
        {
            Instantiate(prefabToSpawn, spawnParent.position, spawnParent.rotation, spawnParent);
        }
        else
        {
            Debug.LogError("Prefab to spawn or spawn parent is not assigned!");
        }
    }
}
