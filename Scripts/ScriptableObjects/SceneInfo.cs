using UnityEngine;

namespace RonplayBoxGameDev.ScriptableObjects
{
    [CreateAssetMenu(fileName = "SceneInfo", menuName = "ScriptableObjects/SceneInfo", order = 0)]
    public class SceneInfo : ScriptableObject
    {
        public string UUID = System.Guid.NewGuid().ToString();
        public Object sceneObject;
    }
}

