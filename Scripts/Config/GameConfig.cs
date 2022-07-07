using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RonplayBoxGameDev
{
    public class GameConfig
    {
        #region Game language
        public enum languageName { English, Russian }
        public static languageName curMarkerLanguage = languageName.English;

        public enum languageShortName { EN, RU }
        public static languageShortName curLanguageShortName = languageShortName.EN;
        #endregion

        #region Markers Settings
        public static MarkersPipeline currentMarkersPipeline;
        #endregion
    }
}

