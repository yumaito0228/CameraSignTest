using System.Collections;
using System.Collections.Generic;
using OscJack;
using UnityEngine;

public class OscReceiver : MonoBehaviour
{
    [SerializeField] int port = 8000;
    
    // Start is called before the first frame update
    private void Start()
    {
        var server = new OscServer(port); 
        server.MessageDispatcher.AddCallback(
            "/OscJack/position",
            (string address, OscDataHandle data) =>
            {Debug.Log($"({data.GetElementAsFloat(0)}, {data.GetElementAsFloat(1)})");
            });
    }

    public void OnReceive(Vector2 position)
    {
        Debug.Log(position);
    }
}
