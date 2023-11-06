using UnityEngine;

namespace Projectile_Simulation
{
    public class ProjectileSlerp : MonoBehaviour
    {
        public Transform startPoint;
        public Transform endPoint;
        private float startTime;

        private void Update()
        {
            float someValue = Mathf.Sin(Time.time);
            float originalValue = Mathf.Sin(someValue); // 예를 들어, -1에서 1의 범위를 가진 사인 함수 결과
            float t = (originalValue + 1) / 2; // 0에서 1로 정규화된 값
                
            Vector3 lerpedPosition = Vector3.Slerp(startPoint.position, endPoint.position, t);
            transform.position = new Vector3(lerpedPosition.x, transform.position.y, transform.position.z);
        }
    }
}