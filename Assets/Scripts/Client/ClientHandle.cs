using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;

public class ClientHandle : MonoBehaviour
{
    public static void Welcome(Packet packet)
    {
        string msg = packet.ReadString();
        int myId = packet.ReadInt();

        Debug.Log($"Message from server: {msg}");
        Client.instance.myId = myId;
        Client.instance.SetWelcomeReceived();
        Debug.Log($"My ID: {myId}.");

        Client.instance.udp.Connect(((IPEndPoint)Client.instance.tcp.socket.Client.LocalEndPoint).Port);
    }

    public static void SyncTime(Packet packet)
    {
        int packetId = packet.ReadInt();
        ClientSend.TimeSync(packetId);
    }

    public static void SpawnPlayer(Packet packet)
    {
        int id = packet.ReadInt();
        string username = packet.ReadString();
        Vector3 position = packet.ReadVector3();
        Quaternion rotation = packet.ReadQuaternion();

        GameManager.instance.SpawnPlayer(id, username, position, rotation);
    }

    public static void PlayerPosition(Packet packet)
    {
        int id = packet.ReadInt();
        Vector3 _position = packet.ReadVector3();
        Quaternion _rotation = packet.ReadQuaternion();
        float _yVelocity = packet.ReadFloat();
        float _time = packet.ReadFloat();
        if (GameManager.players.ContainsKey(id))
        {
            GameManager.players[id].SetLastAcceptedPosition(new PlayerState { _position = _position, _rotation = _rotation, _time = _time, _yVelocity = _yVelocity });
        }
    }

    public static void PlayerDisconnected(Packet packet)
    {
        int id = packet.ReadInt();
        if (GameManager.players.ContainsKey(id))
        {
            Destroy(GameManager.players[id].gameObject);
            GameManager.players.Remove(id);
        }
    }

    public static void PlayerHealth(Packet packet)
    {
        int id = packet.ReadInt();
        float health = packet.ReadFloat();
        if (GameManager.players.ContainsKey(id))
        {
            GameManager.players[id].SetHealth(health);
        }
    }

    public static void PlayerRespawned(Packet packet)
    {
        int id = packet.ReadInt();
        if (GameManager.players.ContainsKey(id))
        {
            GameManager.players[id].Respawn();
        }
    }

    public static void PlayerHitmark(Packet packet)
    {
        int id = packet.ReadInt();
        if (GameManager.players.ContainsKey(id))
        {
            GameManager.players[id].Hitmark();
        }
    }

    public static void CreateItemSpawner(Packet packet)
    {
        int spawnerId = packet.ReadInt();
        Vector3 spawnerPosition = packet.ReadVector3();
        bool hasItem = packet.ReadBool();

        GameManager.instance.CreateItemSpawner(spawnerId, spawnerPosition, hasItem);
    }

    public static void ItemSpawned(Packet packet)
    {
        int spawnerId = packet.ReadInt();
        if (GameManager.itemSpawners.ContainsKey(spawnerId))
        {
            GameManager.itemSpawners[spawnerId].ItemSpawned();
        }
    }

    public static void ItemPickedUp(Packet packet)
    {
        int spawnerId = packet.ReadInt();
        int byPlayer = packet.ReadInt();

        if (GameManager.itemSpawners.ContainsKey(spawnerId) && 
            GameManager.players.ContainsKey(byPlayer))
        {
            GameManager.itemSpawners[spawnerId].ItemPickedUp();
            GameManager.players[byPlayer].itemCount++;
        }
    }

    public static void SpawnProjectile(Packet packet)
    {
        int projectileId = packet.ReadInt();
        Vector3 position = packet.ReadVector3();
        int thrownByPlayer = packet.ReadInt();

        if (GameManager.players.ContainsKey(thrownByPlayer))
        {
            GameManager.instance.SpawnProjectile(projectileId, position);
            GameManager.players[thrownByPlayer].itemCount--;
        }
    }

    public static void ProjectilePosition(Packet packet)
    {
        int projectileId = packet.ReadInt();
        Vector3 _position = packet.ReadVector3();
        if (GameManager.projectiles.ContainsKey(projectileId))
        {
            GameManager.projectiles[projectileId].AddPlayerState(new PlayerState { _position = _position, _time = Time.time, _yVelocity = 0 });
        }
    }

    public static void ProjectileExploded(Packet packet)
    {
        int projectileId = packet.ReadInt();
        Vector3 position = packet.ReadVector3();
        if (GameManager.projectiles.ContainsKey(projectileId))
        {
            GameManager.projectiles[projectileId].Explode(position);
        }
    }
}
