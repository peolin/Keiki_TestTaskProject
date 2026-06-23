using UnityEngine;

namespace Data
{
    public enum CategoryType { Letters, Numbers, Shapes}

    [CreateAssetMenu(fileName = "CategoryConfig", menuName = "Scriptable Objects/CategoryConfiguration")]
    public class CategoryConfig : ScriptableObject
    {
        public CategoryType Category;
        public LevelConfig[] Levels;
    }
}