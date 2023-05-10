using System;
using System.Collections;
using System.Collections.Generic;
using OscJack;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

public class OscReceiver : MonoBehaviour
{
    [SerializeField] int port = 8000;
    [SerializeField] private Camera targetCamera;
    [SerializeField] private GameObject targetObject;
    [SerializeField] private LineDrawer lineDrawer;
    [SerializeField] private float distance;
    
    // public UnityEvent<Vector3> OnReceivedPosition = new UnityEvent<Vector3>();

    public bool IsReceivedStart = false;
    private float lastReceivedTime = 0.0f;
    [SerializeField] private float waitTime = 0.1f;

    private OscServer server;
    
    // Start is called before the first frame update
    private void Start()
    {
        server = new OscServer(port); 
        server.MessageDispatcher.AddCallback(
            "/OscJack/position",
            (string address, OscDataHandle data) =>
            {Debug.Log($"({data.GetElementAsFloat(0)}, {data.GetElementAsFloat(1)})");
            });
    }

    private void OnDisable()
    {
        server.Dispose();
        server = null;
    }

    private void Update()
    {
        if (IsReceivedStart && lastReceivedTime - Time.time > waitTime)
        {
            IsReceivedStart = false;
        }
    }

    public void OnReceive(Vector2 position)
    {
        var pos = targetCamera.ViewportToScreenPoint(position);
        pos.z = distance;
        pos = targetCamera.ScreenToWorldPoint(pos);
        targetObject.transform.position = pos;
        lineDrawer.Draw(pos);
        // Debug.Log(position);
        // var screen = targetCamera.ViewportToScreenPoint(position);
        // Debug.Log(screen);
        // screen.z = 10;
        // var world = targetCamera.ScreenToWorldPoint(screen);
        // Debug.Log(world);
    } 
}
