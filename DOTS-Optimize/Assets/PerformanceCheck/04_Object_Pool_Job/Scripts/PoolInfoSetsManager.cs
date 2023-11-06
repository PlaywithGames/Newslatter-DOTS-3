using System.Collections.Generic;
using UnityEngine;

namespace Object_Pool_Job
{
    public class PoolInfoSetsManager : MonoBehaviour
    {
        public enum ePoolInfoType
        {
            none = 0,
            Sound,
            Bundle,
        }
        
        private Dictionary<string, PoolInfoSets> 
            m_container = new Dictionary<string, PoolInfoSets>(300);
        private GameObject m_Bundle;

        [SerializeField] private GameObject projectileGO; 
        
        public bool isSpawnComplete = false;
        
        void Start()
        {
            m_Bundle = new GameObject("@Bundle");
            m_Bundle.transform.SetParent(this.transform);

            for (int i = 0; i < 100; i++)
            {
                GameObject projectile = Instantiate(projectileGO);
                Push(projectile, eStoreType.TIMER_DESTROY, 
                    ePoolInfoType.Bundle, 1);
            }
            isSpawnComplete = true;
        }
        
        public GameObject Pop(string obName, System.Int32 setLayer = -1)
        {
            if (m_container.ContainsKey(obName) == false)
            {
                return null;
            }
            return m_container[obName].PopData(setLayer);
        }
        
        public void Push<T>(T _ob, eStoreType _storeType, 
            ePoolInfoType _poolInfoType = ePoolInfoType.none, 
            System.Int32 setLayer = -1) where T : Component
        {
            if (_ob == null)
            {
                return;
            }

            Push(_ob.gameObject, _storeType, _poolInfoType, setLayer);
        }
        
        public void Push(GameObject _ob, eStoreType _storeType, 
            ePoolInfoType _poolInfoType = ePoolInfoType.none, 
            System.Int32 setLayer = -1)
        {
            if(_ob == null)
            {
                return;
            }

            string _obName = _ob.name;
            if (m_container.ContainsKey(_obName) == false)
            {
                m_container.Add(_obName, new 
                    PoolInfoSets(_obName, this.
                        transform, m_Bundle.transform));
            }

            m_container[_obName].PushData(_ob, _storeType, setLayer);
        }
    }
}