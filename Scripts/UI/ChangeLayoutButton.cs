using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace RonplayBoxGameDev
{
    public class ChangeLayoutButton : MonoBehaviour
    {
        [SerializeField] private GameObject currentlayout;
        [SerializeField] private GameObject taretLayout;

        public UnityEvent OnLayoutChanged;

        private void Start()
        {
            Button changeLayoutButton = GetComponent<Button>();

            changeLayoutButton.onClick.AddListener
                (
                    () =>
                    {
                        ChangeLayout();
                    }
                );
        }

        private void ChangeLayout()
        {
            AudioManager.instance.PlayClickSound();

            OnLayoutChanged.Invoke();

            currentlayout.SetActive(false);
            taretLayout.SetActive(true);
        }
    }
}

