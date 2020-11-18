using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public static Dictionary<int, PlayerController> players = new Dictionary<int, PlayerController>();
    public static Dictionary<int, ItemSpawner> itemSpawners = new Dictionary<int, ItemSpawner>();
    public static Dictionary<int, ProjectileController> projectiles = new Dictionary<int, ProjectileController>();

    public GameObject localPlayerPrefab;
    public GameObject playerPrefab;
    public GameObject itemSpawnerPrefab;
    public GameObject projectilePrefab;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Debug.Log("Instance already exists, destroying object.");
            Destroy(this);
        }
    }

    public void SpawnPlayer(int id, string username, Vector3 position, Quaternion rotation)
    {
        GameObject player;
        if (id == Client.instance.myId)
        {
            Camera.main.gameObject.SetActive(false);
            player = Instantiate(localPlayerPrefab, position, rotation);
            IngameMenuManager.instance.SetLocalPlayer(player.GetComponent<LocalPlayerController>());
        } 
        else
        {
            player = Instantiate(playerPrefab, position, rotation);
        }
        Debug.Log($"Spawning player {id}.");
        player.GetComponent<PlayerController>().Initialize(id, username);
        players.Add(id, player.GetComponent<PlayerController>());
    }

    public void CreateItemSpawner(int spawnerId, Vector3 position, bool hasItem)
    {
        GameObject spawner = Instantiate(itemSpawnerPrefab, position, itemSpawnerPrefab.transform.rotation);
        spawner.GetComponent<ItemSpawner>().Initialize(spawnerId, hasItem);
        itemSpawners.Add(spawnerId, spawner.GetComponent<ItemSpawner>());
    }

    public void SpawnProjectile(int id, Vector3 position)
    {
        GameObject projectile = Instantiate(projectilePrefab, position, Quaternion.identity);
        projectile.GetComponent<ProjectileController>().Initialize(id);
        projectiles.Add(id, projectile.GetComponent<ProjectileController>());
    }
}
