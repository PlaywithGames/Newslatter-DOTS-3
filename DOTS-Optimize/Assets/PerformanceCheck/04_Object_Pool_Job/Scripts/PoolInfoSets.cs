using System.Collections.Generic;
using UnityEngine;

namespace Object_Pool_Job
{
    public enum eStoreType
    {
        DONT_DESTROY,   //파괴 금지 영구 보관
        TIMER_DESTROY,  //일정 시간 뒤 사용 안하면 파괴
        STAGE_DESTROY,  //스테이지 전환시 파괴.
        __MAX__
    }

    public class PoolInfoSets
    {
        private Transform ParentTF = null;
        private Transform BundleTF = null;
        private float ReleaseMaxTime = 180; //3분 동안 안쓰면 제거.
        private GameObject BundleObject = null;
        private List<GameObject> StoreList = new List<GameObject>(10);
        
        public string StoreName
        {
            get
            {
                return _StoreName;
            }
            set
            {
                _StoreName = value;
            }
        }
        public string _StoreName = string.Empty;
        public eStoreType StoreType = eStoreType.__MAX__;

        private int UsedInstanceCount = 0;
        
        public PoolInfoSets(string _storeName, Transform _parentTF, Transform _BundleTF)
        {
            ParentTF = _parentTF;
            BundleTF = _BundleTF;

            StoreName = _storeName;

            UsedInstanceCount = 0;
        }
        
        public GameObject PopData(System.Int32 setLayer = -1)
        {
            GameObject _spawnObj = null;
            if (StoreList.Count > 0)
            {
                _spawnObj = StoreList[0];
                StoreList.RemoveAt(0);
            }
            else
            {
                if(BundleObject == null)
                {
                    return null;
                }

                _spawnObj = GameObject.Instantiate(BundleObject);
                _spawnObj.name = StoreName;
                _spawnObj.transform.parent = ParentTF;

                UsedInstanceCount++;
            }

            _spawnObj.transform.position = Vector3.zero;
            return _spawnObj;
        }
        
        public void PushData(GameObject _storeObj, eStoreType _storeType, System.Int32 setLayer = -1)
        {
            GLUtil.SetActiveObject(_storeObj, false);

            if (setLayer > -1)
            {
                GLUtil._SetLayerRecursively(setLayer, _storeObj.transform);
            }

            StoreList.Add(_storeObj);
            _storeObj.transform.SetParent(BundleTF, false);
            _storeObj.transform.position = Vector3.zero;
            _storeObj.transform.eulerAngles = Vector3.zero;
            StoreType = _storeType;
            StoreName = _storeObj.name;
        }
    }
}