using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace RonplayBoxGameDev
{
    public class ApplicationVersion : MonoBehaviour
    {
        public Text version;

        private void Start()
        {
            version.text = $"v.{Application.version}";
        }
    }
}

