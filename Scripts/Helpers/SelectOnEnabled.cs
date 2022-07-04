using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace RonplayGameDev
{
    public class SelectOnEnabled : MonoBehaviour
    {
        public GameObject objectToSelectOnEnabled;
        public GameObject helperObject;

        public bool saveSelectedOnDisable = true;

        private void OnEnable()
        {
            if (!_started) return;

            SelectOject();
            Invoke(nameof(SelectOject), 0.1f);
        }

        private void OnDisable()
        {
            if (!saveSelectedOnDisable) return;

            var event_system = FindObjectOfType<EventSystem>();

            if (event_system == null) return;

            objectToSelectOnEnabled = event_system.currentSelectedGameObject;
        }

        private void Awake()
        {
            _started = true;

            SelectOject();

            Invoke(nameof(SelectOject), 0.1f); 
        }

        private void Start()
        {
            _started = true;

            SelectOject();

            Invoke(nameof(SelectOject), 0.1f);
        }

        private void SelectOject()
        {
            var event_system = FindObjectOfType<EventSystem>();

            event_system.SetSelectedGameObject(helperObject);
            event_system.SetSelectedGameObject(objectToSelectOnEnabled);
        }

        private bool _started = false;
    }
}
