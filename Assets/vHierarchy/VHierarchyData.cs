#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.ShortcutManagement;
using System.Reflection;
using System.Linq;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;
using UnityEditor.SceneManagement;
using static VHierarchy.Libs.VUtils;
using static VHierarchy.Libs.VGUI;


namespace VHierarchy
{
    public class VHierarchyData : ScriptableObject
    {
        public SerializeableDicitonary<string, SceneData> sceneDatasByGuid = new SerializeableDicitonary<string, SceneData>();
        public Dictionary<Scene, SceneData> sceneDatasByScene = new Dictionary<Scene, SceneData>();

        [System.Serializable]
        public class SceneData
        {
            public SerializeableDicitonary<string, GameObjectData> goDatasByGlobalId = new SerializeableDicitonary<string, GameObjectData>();
            public Dictionary<int, GameObjectData> goDatasByInstanceId = new Dictionary<int, GameObjectData>();

        }

        [System.Serializable]
        public class GameObjectData
        {
            public Color color => VHierarchyIconEditor.GetColor(iColor);
            public int iColor;
            public string icon = "";

        }
    }
}
#endif