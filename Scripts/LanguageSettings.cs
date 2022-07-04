using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RonplayGameDev
{
    public class LanguageSettings
    {
        public enum languageName { English, Russian }
        public static languageName curMarkerLanguage = languageName.English;

        public enum languageShortName { EN, RU }
        public static languageShortName curLanguageShortName = languageShortName.EN;
    }
}

