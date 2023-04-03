using System.Collections;
using System.Collections.Generic;
using OscJack;
using UnityEngine;

public class CameraSignManager : MonoBehaviour
{
    [SerializeField] string ipAddress = "127.0.0.1";
    [SerializeField] int port = 8080;
    OscClient client;

    private void OnEnable()
    {
        client = new OscClient(ipAddress, port);
    }

    private void OnDisable()
    {
        client.Dispose();
    }


    public void SendSignPosition(Vector2 vector2)
    {
        client.Send("/OscJack/position", vector2.x, vector2.y);
    }
}
