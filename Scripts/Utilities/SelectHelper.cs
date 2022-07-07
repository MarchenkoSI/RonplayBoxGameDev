using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace RonplayBoxGameDev
{
    public class SelectHelper : MonoBehaviour
    {
        public GameObject overlay;

        public GameObject selectedObject;
        public GameObject helperObject;

        private void Update()
        {
            var event_system = FindObjectOfType<EventSystem>();

            if (overlay.activeSelf)
            {
                if (_is_selected) return;

                _is_selected = true;

                event_system.SetSelectedGameObject(helperObject);
                event_system.SetSelectedGameObject(selectedObject);
            }
            else
            {
                _is_selected = false;

                event_system.SetSelectedGameObject(null);
            }
        }

        private bool _is_selected = false;
    }

}
