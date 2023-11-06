using System.Collections.Generic;
using UnityEngine;

namespace Projectile_Simulation
{
    public class ProjectileSlerpManager : MonoBehaviour
    {
        public GameObject projectilePrefab;
        public int numProjectiles = 100;
        public Transform startPoint;
        public Transform endPoint;

        private List<GameObject> projectileObjectList = new List<GameObject>();

        private void OnEnable()
        {
            for (int i = 0; i < numProjectiles; i++)
            {
                GameObject projectileObject = Instantiate(projectilePrefab, startPoint.position, Quaternion.identity);
                projectileObjectList.Add(projectileObject);
                ProjectileSlerp projectileLerpComponent = projectileObject.GetComponent<ProjectileSlerp>();
                projectileLerpComponent.startPoint = startPoint;
                projectileLerpComponent.endPoint = endPoint;
            }
        }

        private void OnDisable()
        {
            for (int i = 0; i < projectileObjectList.Count; i++)
            {
                Destroy(projectileObjectList[i]);
            }
        }
    }
}