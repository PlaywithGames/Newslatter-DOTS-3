using System;
using UnityEngine;

namespace Object_Pool_Job
{
    public class GLUtil
    {
        public static void SetActiveObject(GameObject _go, bool _value)
        {
            if (_go == null)
            {
                return;
            }

            if (_go.activeSelf != _value)
            {
                _go.SetActive(_value);
            }
        }
        
        public static void _SetLayerRecursively(Int32 nLayer, Transform t)
        {
            t.gameObject.layer = nLayer;

            for (int i = 0; i < t.childCount; i++)
            {
                _SetLayerRecursively(nLayer, t.GetChild(i));
            }
        }
    }
}