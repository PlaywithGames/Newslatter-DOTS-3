using UnityEngine;

namespace Projectile_Testing
{
    public class Config_Projectile : MonoBehaviour
    {
        [SerializeField] private int targetFrameRate;
        private void Awake()
        {
            Application.targetFrameRate = targetFrameRate;
        }
    }
}