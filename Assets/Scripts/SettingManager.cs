using Assets.Scripts;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SettingManager : MonoBehaviour
{
    [SerializeField] private TMP_InputField ipAddress;
    [SerializeField] private TMP_InputField port;
    [SerializeField] private TMP_InputField oscAddress;

    private void Start()
    {
        if (PlayerPrefs.HasKey("ipAddress"))
        {
            ipAddress.text = PlayerPrefs.GetString("ipAddress");
        }
        
        if (PlayerPrefs.HasKey("port"))
        {
            port.text = PlayerPrefs.GetString("port");
        }
        
        if (PlayerPrefs.HasKey("oscAddress"))
        {
            oscAddress.text = PlayerPrefs.GetString("oscAddress");
        }
    }

    public void OnClick()
    {
        var cameraSignAppManager = CameraSignAppManager.Instance;
        cameraSignAppManager.oscConnection.host = ipAddress.text;
        cameraSignAppManager.oscConnection.port = int.Parse(port.text);
        cameraSignAppManager.oscAddress = oscAddress.text;
        
        
        SceneManager.LoadScene("Paint");
    }
}

