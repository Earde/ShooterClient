using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClientSend : MonoBehaviour
{
    private static void SendTCPData(Packet packet)
    {
        packet.WriteLength();
        Client.instance.tcp.SendData(packet);
    }

    private static void SendUDPData(Packet packet)
    {
        packet.WriteLength();
        Client.instance.udp.SendData(packet);
    }

    #region Packets
    /// <summary>
    /// Send WelcomeReceived to server (player name)
    /// </summary>
    /// <param name="name"></param>
    public static void WelcomeReceived(string name)
    {
        using (Packet packet = new Packet((int)ClientPackets.welcomeReceived))
        {
            packet.Write(Client.instance.myId);
            packet.Write(name);

            SendTCPData(packet);
        }
    }

    /// <summary>
    /// Send TimeSync to server (id of server TimeSyncPacket)
    /// Send client game time to server for RTT and client/server time mismatch
    /// </summary>
    /// <param name="packetId"></param>
    public static void TimeSync(int packetId)
    {
        using (Packet packet = new Packet((int)ClientPackets.timeSync))
        {
            packet.Write(packetId);
            packet.Write(Time.time);
            SendUDPData(packet);
        }
    }

    /// <summary>
    /// Send Movement to server (keyboard inputs, rotation, Client game time)
    /// </summary>
    /// <param name="_inputs"></param>
    /// <param name="rotation"></param>
    /// <param name="time"></param>
    public static void PlayerMovement(bool[] _inputs, Quaternion rotation, float time)
    {
        using (Packet _packet = new Packet((int)ClientPackets.playerMovement))
        {
            _packet.Write(time);
            _packet.Write(_inputs.Length);
            foreach (bool _input in _inputs)
            {
                _packet.Write(_input);
            }
            _packet.Write(rotation);

            SendUDPData(_packet);
        }
    }

    /// <summary>
    /// Send ChangeGun to server (client game time, index of gun)
    /// </summary>
    /// <param name="time"></param>
    /// <param name="gunId"></param>
    public static void PlayerChangeGun(float time, int gunId)
    {
        using (Packet packet = new Packet((int)ClientPackets.playerChangeGun))
        {
            packet.Write(time);
            packet.Write(gunId);

            SendTCPData(packet);
        }
    }

    /// <summary>
    /// Send Shoot to server (camera direction, client game time, enemy interpolation delay)
    /// </summary>
    /// <param name="facing"></param>
    /// <param name="time"></param>
    /// <param name="enemyDelay"></param>
    public static void PlayerShoot(Vector3 facing, float time, float enemyDelay)
    {
        using (Packet packet = new Packet((int)ClientPackets.playerShoot))
        {
            packet.Write(facing);
            packet.Write(time);
            packet.Write(enemyDelay);

            SendUDPData(packet);
        }
    }

    /// <summary>
    /// Send ThrowItem to server (camera direction)
    /// </summary>
    /// <param name="facing"></param>
    public static void PlayerThrowItem(Vector3 facing)
    {
        using (Packet packet = new Packet((int)ClientPackets.playerThrowItem))
        {
            packet.Write(facing);

            SendTCPData(packet);
        }
    }
    #endregion
}
