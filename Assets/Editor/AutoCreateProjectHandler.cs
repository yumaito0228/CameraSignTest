using System;
using System.IO;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using EditType = Editor.ProjectPath.EditType;
using Object = UnityEngine.Object;

namespace Editor
{
    public static class AutoCreateProjectHandler
    {
        public static void Process()
        {
            var assetsPath = Application.dataPath;
            var partListData = File.ReadAllText(assetsPath + "/PartList.txt");
            var partList = partListData.Split(" ");

            foreach (var partName in partList)
            {
                if (AssetDatabase.IsValidFolder(ProjectPath.GetFolderPath(partName)))
                    continue;
                
                // 収録したOKパートごとのフォルダを作成する。PartList.txtに収録したパート名は格納
                // Partごとに「Motions」「Prefabs」「Scenes」「Timelines」フォルダを追加
                CreatePartFolder(partName);

                // Timelinesフォルダにprefixに「パート名_」をつけたTimelineAssetを追加。
                CreateTimelineAssets(partName);
                
                var objectName = $"{partName}_Master";
                var masterObject = new GameObject(objectName);
                var playableDirector = masterObject.AddComponent<PlayableDirector>();
                playableDirector.playOnAwake = false;
                playableDirector.playableAsset =
                    AssetDatabase.LoadAssetAtPath<PlayableAsset>(ProjectPath.GetTimelinePath(partName, EditType.Master));
                PrefabUtility.SaveAsPrefabAsset(masterObject, ProjectPath.GetPrefabPath(partName, EditType.Master));

                // Scenesフォルダにprefixに「パート名_」をつけたSceneAssetを追加。
                CreateSceneAssets(partName);
                
                // MasterのTimelineにAudioTrackとControlTrackを追加。
                SetMasterTimeline(partName);
            }
        }

        private static void CreatePartFolder(string partName)
        {
            var path = $"{Application.dataPath}/{partName}";
            if (Directory.Exists(path))
                return;

            AssetDatabase.CreateFolder("Assets", partName);

            var subDirectories = new[] { "Motion", "Prefabs", "Scenes", "Timelines" };
            foreach (var subDirectory in subDirectories)
            {
                AssetDatabase.CreateFolder($"Assets/{partName}", subDirectory);
            }
        }

        private static void CreateTimelineAssets(string partName)
        {
            foreach (EditType type in Enum.GetValues(typeof(EditType)))
            {
                var timelineAsset = ScriptableObject.CreateInstance<TimelineAsset>();
                AssetDatabase.CreateAsset(timelineAsset, ProjectPath.GetTimelinePath(partName, type));
            }
        }

        public static void SetMasterTimeline(string partName)
        {
            EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
            
            var masterObject = PrefabUtility.InstantiatePrefab(
                AssetDatabase.LoadAssetAtPath<GameObject>(ProjectPath.GetPrefabPath(partName, EditType.Master))).GameObject();
            var masterTimeline = AssetDatabase.LoadAssetAtPath<TimelineAsset>(
                ProjectPath.GetTimelinePath(partName, EditType.Master));

            masterTimeline.CreateTrack<AudioTrack>();
            
            // ControlTrackを作成して、各トラックに子になっているTimelineを割り当てる。
            foreach (var type in Enum.GetValues(typeof(EditType)))
            {
                if (type is EditType.Master or EditType.Recording)
                    continue;

                var trackName = Enum.GetName(typeof(EditType), type);
                var controlTrack = masterTimeline.CreateTrack<ControlTrack>(null, trackName);
                var clip = controlTrack.CreateDefaultClip();
                clip.displayName = $"{partName}_{trackName}";

                var serializedObject = new SerializedObject(clip.asset, masterObject.GetComponent<PlayableDirector>());
                var serializedProperty = serializedObject.FindProperty("sourceGameObject");
                serializedProperty.exposedReferenceValue = masterObject
                    .GetComponentsInChildren<PlayableDirector>()
                    .Select(director => director.gameObject)
                    .Single(obj => trackName != null && obj.name.Contains(trackName));
                serializedObject.ApplyModifiedProperties();
            }
        }

        public static void SetRecordingTimeline(string partName)
        {
            EditorSceneManager.OpenScene(ProjectPath.GetScenePath(partName, EditType.Recording));

            var masterObject = GameObject.Find($"{partName}_Master");
            var recordingObject = GameObject.Find($"{partName}_Recording");
            var recordingTimeline = AssetDatabase.LoadAssetAtPath<TimelineAsset>(
                ProjectPath.GetTimelinePath(partName, EditType.Recording));

            var masterTrack = recordingTimeline.CreateTrack<ControlTrack>(null, "Master");
            var clip = masterTrack.CreateDefaultClip();
            clip.displayName = $"{partName}_Master";
            var serializedObject = new SerializedObject(clip.asset, recordingObject.GetComponent<PlayableDirector>());
            var serializedProperty = serializedObject.FindProperty("sourceGameObject");
            serializedProperty.exposedReferenceValue = masterObject;
            serializedObject.ApplyModifiedProperties();
        }

        private static void CreateSceneAssets(string partName)
        {
            foreach (EditType type in Enum.GetValues(typeof(EditType)))
            {
                if (type == EditType.Master)
                    continue;
                
                var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
                var masterPath = ProjectPath.GetPrefabPath(partName, EditType.Master);

                // 親になるMasterObjectをインスタンス化してシーンに配置。
                var masterObject = PrefabUtility.InstantiatePrefab(
                        AssetDatabase.LoadAssetAtPath<GameObject>(masterPath)).GameObject();
                
                // 子に配置するオブジェクトを作成
                var objectName = $"{partName}_{type}";
                var prefabObject = new GameObject(objectName);
                var playableDirector = prefabObject.AddComponent<PlayableDirector>();
                playableDirector.playOnAwake = false;
                playableDirector.playableAsset =
                    AssetDatabase.LoadAssetAtPath<PlayableAsset>(ProjectPath.GetTimelinePath(partName, type));

                if (type != EditType.Recording)
                {
                    // 編集用のPrefabを子のタイムラインごとに作成。
                    PrefabUtility.SaveAsPrefabAssetAndConnect(prefabObject, ProjectPath.GetPrefabPath(partName, type),
                        InteractionMode.AutomatedAction);
                    // Masterを親にしてPrefabに反映
                    prefabObject.transform.SetParent(masterObject.transform);
                    PrefabUtility.ApplyPrefabInstance(masterObject, InteractionMode.AutomatedAction);
                    // Edit用のPrefabをロード（Masterの子にしたオブジェクトはdisableに変更）
                    PrefabUtility.InstantiatePrefab(
                        AssetDatabase.LoadAssetAtPath<GameObject>(ProjectPath.GetPrefabPath(partName, type))).GameObject();
                    prefabObject.SetActive(false);
                }
                
                // 子のタイムラインごとにシーンの作成。
                EditorSceneManager.SaveScene(scene, ProjectPath.GetScenePath(partName, type));
            }
        }
    }
}