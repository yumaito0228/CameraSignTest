using OscJack;
using UnityEngine;

namespace Assets.Scripts
{
    public class CameraSignAppManager: SingletonMonoBehaviour<CameraSignAppManager>
    {
        private OscClient _client;
        public OscConnection oscConnection { get; set; }

        public string oscAddress = "";
        
        protected override bool DontDestroyOnLoad => true;

        protected override void Awake()
        {
            base.Awake();
            oscConnection = ScriptableObject.CreateInstance<OscConnection>();
            oscConnection.type = OscConnectionType.Udp;
        }
    }
}