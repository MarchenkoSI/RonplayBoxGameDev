using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RonplayBoxGameDev
{
    [CreateAssetMenu(fileName = "Alphabet", menuName = "Alphabet")]
    public class AlphabetSO : ScriptableObject
    {
        public List<string> letter;
    }
}

