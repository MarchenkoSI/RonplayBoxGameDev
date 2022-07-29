using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RonplayBoxGameDev
{
    public class ChangeMarkersPipelineButton : MonoBehaviour
    {
        public MarkersPipeline markersPipeline;
        public void ApplyLanguage()
        {
            switch (markersPipeline)
            {
                case MarkersPipeline.Latin:
                    GameConfig.currentMarkersPipeline = MarkersPipeline.Latin;
                    break;
                case MarkersPipeline.Cyrillic:
                    GameConfig.currentMarkersPipeline = MarkersPipeline.Cyrillic;
                    break;
                case MarkersPipeline.Math:
                    GameConfig.currentMarkersPipeline = MarkersPipeline.Math;
                    break;
                default:
                    throw new Exception("Default reached");
            }

            PlayerPrefs.SetString("CurrentMarkersPipeline", GameConfig.currentMarkersPipeline.ToString());

            CVController.instance.UpdateWordsCVManager();
            CVController.instance.UpdateSymbolsCVManager();
        }
    }
}

