namespace Editor
{
    using UnityEditor;
    using UnityEngine;

    public class TestScriptEditor : EditorWindow
    {
        [MenuItem("Test/Editor extention/Sample", false, 1)]
        private static void ShowWindow()
        {
            var window = GetWindow<TestScriptEditor>();
            window.titleContent = new GUIContent("Sample Window");
        }

        private void OnGUI()
        {
            //ここから追加分
            if (GUILayout.Button("SetMasterTimeline"))
            {
                AutoCreateProjectHandler.SetRecordingTimeline("DOGS");
            }
        }
    }
}