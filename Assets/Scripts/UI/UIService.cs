using System.Collections;
using UnityEngine;

namespace UI
{
    public class UIService : MonoBehaviour
    {
        [SerializeField]
        private GameObject popupCheckText;
        private Animation _popupCheckAnimation;
        
        [SerializeField]
        private GameObject popupCheckmateText;

        private void Awake()
        {
            _popupCheckAnimation = popupCheckText.GetComponent<Animation>();
        }

        public void ShowCheckText()
        {
            popupCheckText.SetActive(true);
            StartCoroutine(CheckTextCoroutine());
        }

        private IEnumerator CheckTextCoroutine()
        {
            yield return new WaitUntil(() => !_popupCheckAnimation.isPlaying);
            popupCheckText.SetActive(false);
        }

        public void ShowCheckmateText()
        {
            popupCheckmateText.SetActive(true);
        }
    }
}