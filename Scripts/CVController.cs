using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RonplayBoxGameDev
{
    public class CVController : MonoBehaviour
    {
        public static CVController instance = null;

        [SerializeField] private WordsCVManager wordsCVManager = null;

        public void UpdateWordsCVManager()
        {
            if (wordsCVManager == null) return;

            wordsCVManager.SetMarkersPipline();
        }

        private void Start()
        {
            if (instance == null)
            {
                instance = this;
            }
            else
            {
                Destroy(gameObject);
            }

            DontDestroyOnLoad(gameObject);
        }
    }
}

