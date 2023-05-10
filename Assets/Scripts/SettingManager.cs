using Assets.Scripts;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SettingManager : MonoBehaviour
{
    [SerializeField] private TMP_InputField ipaddress;
    [SerializeField] private TMP_InputField port;
    
    public void OnClick()
    {
        var oscConnection = CameraSignAppManager.Instance.oscConnection;
        oscConnection.host = ipaddress.text;
        oscConnection.port = int.Parse(port.text);
        
        SceneManager.LoadScene("Paint");
    }
}
