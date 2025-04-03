using UnityEngine;


namespace Assets.Scripts.Utility
{
    public static class Util
    {
        public static GameObject FindParentWithTag(Transform child, string tag)
        {
            Transform parent = child.parent;
            while (parent != null)
            {
                if (parent.CompareTag(tag))
                {
                    return parent.gameObject;
                }
                parent = parent.parent;
            }
            return null;
        }
    }

}