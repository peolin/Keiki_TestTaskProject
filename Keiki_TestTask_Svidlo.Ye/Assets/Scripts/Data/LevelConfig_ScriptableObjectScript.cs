using UnityEngine;

namespace Data
{
    [CreateAssetMenu(fileName = "LevelConfig", menuName = "Scriptable Objects/LevelConfiguration")]
    public class LevelConfig : ScriptableObject
    {
        [Header("Visuals")]
        public Sprite PathImage;
        public Color LevelColor;

        [Header("Prefab Setup")]
        public GameObject StrokeGuidesPrefab; 
    
        [Header("Path Data for Logic")]
        public StrokeData[] StrokePath; 
    }
}