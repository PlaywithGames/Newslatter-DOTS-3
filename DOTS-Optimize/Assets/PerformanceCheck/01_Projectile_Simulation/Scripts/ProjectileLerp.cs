using UnityEngine;

namespace Projectile_Simulation
{
    public class ProjectileLerp : MonoBehaviour
    {
        public Transform startPoint;
        public Transform endPoint;
        private float startTime;

        private void LateUpdate()
        {
            float someValue = Mathf.Sin(Time.time);
            // 예를 들어, -1에서 1의 범위를 가진 사인 함수 결과
            float originalValue = Mathf.Sin(someValue);
            // 0에서 1로 정규화된 값
            float t = (originalValue + 1) / 2; 
                
            Vector3 lerpedPosition = Vector3.Lerp
                (startPoint.position, endPoint.position, t);
            transform.position = new Vector3(lerpedPosition.x, 
                transform.position.y, transform.position.z);
        }
    }
}