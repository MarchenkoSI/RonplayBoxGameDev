using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RonplayBoxGameDev
{
    [CreateAssetMenu(fileName = "AlphabetDatabase", menuName = "Database/AlphabetDatabase")]
    public class AlphabetDatabase : ScriptableObject
    {
        #region singleton
        private static AlphabetDatabase instance;
        public static AlphabetDatabase Instance
        {
            get
            {
                if (instance == null)
                    instance = Resources.Load("Databases/AlphabetDatabase") as AlphabetDatabase;

                return instance;
            }
        }
        #endregion

        public AlphabetSO[] alphabet;

        public AlphabetSO Get(int index)
        {
            return (alphabet[index]);
        }
    }
}

