using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace com.braineeeedevs.mouseflight.game
{
public class SpawnManager : MonoBehaviour
{
    public  Controller playerControls;
    public List<Transform> spawnPoints = new List<Transform>();
    public Dictionary<string, ObjectPooler> fleetPool = new Dictionary<string, ObjectPooler> ();
    protected Coroutine playerRespawnDelay;
    void Start()
    {
        GameObject [] taggedSpawns = GameObject.FindGameObjectsWithTag("SpawnPoint");
        foreach(GameObject go in taggedSpawns)
        {
            spawnPoints.Add(go.transform);
        }
        GameObject [] pools = GameObject.FindGameObjectsWithTag("SpawnPoint");
        foreach(GameObject pool in pools)
        {
            fleetPool.Add(pool.name, pool.GetComponent<ObjectPooler>());
        }
    }
    public void StartRespawn(StarShip player)
    {
        if(playerRespawnDelay == null)
        {
            playerRespawnDelay = StartCoroutine(Respawn(player));
        }
    } 

    protected IEnumerator Respawn(StarShip ship)
    {
        Transform targetSpawn = spawnPoints[Random.Range(0,spawnPoints.Count)];

        playerControls.ResetCamera(targetSpawn);

        yield return new WaitForSeconds(5f);
        Debug.Assert(fleetPool.Count > 0, "Fleet pooler is empty.");
        Debug.Assert(spawnPoints.Count > 0, "Spawnpoints list is empty.");
        fleetPool[ship.gameObject.name].GetObject(ship, targetSpawn);
        playerRespawnDelay = null;
    }
}

}