using System;
using UnityEditor;

namespace Editor
{
    public static class ProjectPath
    {
        public enum EditType
        { 
            BGV, 
            Camera, 
            Lighting, 
            Master, 
            Motion, 
            VFX, 
            Recording
        }

        public static string GetScenePath(string partName, EditType type)
        {
            switch (type)
            {
                case EditType.BGV:
                case EditType.Camera:
                case EditType.Lighting:
                case EditType.Master:
                case EditType.Motion:
                case EditType.VFX:
                    return $"Assets/{partName}/Scenes/{partName}_{Enum.GetName(typeof(EditType), type)}_Edit.unity";
                case EditType.Recording:
                    return $"Assets/{partName}/Scenes/{partName}_{Enum.GetName(typeof(EditType), type)}.unity";
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
        }

        public static string GetPrefabPath(string partName, EditType type)
        {
            return $"Assets/{partName}/Prefabs/{partName}_{Enum.GetName(typeof(EditType), type)}.prefab";
        }
        
        public static string GetTimelinePath(string partName, EditType type)
        {
            return $"Assets/{partName}/Timelines/{partName}_{Enum.GetName(typeof(EditType), type)}.playable";
        }

        public static string GetFolderPath(string partName)
        {
            return $"Assets/{partName}";
        }
    }
}