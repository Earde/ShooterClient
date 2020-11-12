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
    public static void WelcomeReceived()
    {
        using (Packet packet = new Packet((int)ClientPackets.welcomeReceived))
        {
            packet.Write(Client.instance.myId);
            packet.Write("FIX THIS"); //MainMenuManager.instance.usernameField.text

            SendTCPData(packet);
        }
    }

    public static void TimeSync(int packetId)
    {
        using (Packet packet = new Packet((int)ClientPackets.timeSync))
        {
            packet.Write(packetId);
            packet.Write(Time.time);
            SendUDPData(packet);
        }
    }

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

    public static void PlayerShoot(Vector3 facing, float time, float enemyTime)
    {
        using (Packet packet = new Packet((int)ClientPackets.playerShoot))
        {
            packet.Write(facing);
            packet.Write(time);
            packet.Write(enemyTime);

            SendTCPData(packet);
        }
    }

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
