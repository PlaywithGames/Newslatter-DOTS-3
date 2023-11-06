using UnityEngine;
using UnityEngine.UI;

namespace Projectile_Simulation
{
    public class PerformanceCheckSelector : MonoBehaviour
    {
        [SerializeField] private GameObject[] projectileObjects;
        [SerializeField] private Button[] performanceCheckButtons;
        [SerializeField] private Text stateText;

        private void Start()
        {
            ButtonListenerInit();
            SetDisableAll();
        }

        private void SetDisableAll()
        {
            for (int i = 0; i < projectileObjects.Length; i++)
            {
                projectileObjects[i].SetActive(false);
            }
        }

        private void ButtonListenerInit()
        {
            for (int i = 0; i < performanceCheckButtons.Length; i++)
            {
                int index = i;
                performanceCheckButtons[i].onClick.AddListener(() =>
                {
                    OnPerformanceCheckButtonClick(index); 
                });
            }
        }

        private void OnPerformanceCheckButtonClick(int index)
        {
            SetDisableAll();
            projectileObjects[index].SetActive(true);
            
            GameObject textObject = performanceCheckButtons[index].transform.GetChild(0).gameObject;
            Text childText = textObject.GetComponent<Text>();
            stateText.text = string.Format("State : {0}",childText.text);
        }
    }
}