using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityRonplayBoxSDK;

namespace RonplayBoxGameDev
{
    public class ConvertorIdToWords
    {
        public static string ConvertToString(List<int> word_symbols_ids_)
        {
            string out_word = "";

            for (int symbol_index = 0; symbol_index < word_symbols_ids_.Count; symbol_index++)
            {
                out_word += AlphabetDatabase.Instance.alphabet[(int)GameConfig.currentMarkersPipeline].letter[word_symbols_ids_[symbol_index]];
            }

            return out_word;
        }
    }
}

