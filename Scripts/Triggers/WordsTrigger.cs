using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityRonplayBoxSDK;

namespace RonplayBoxGameDev
{
    [System.Serializable]
    public class WordInformation
    {
        public int id;
        public List<string> word;
        public DateTime bornTime;
        public double timeOnTheTable;
        public Vector3 bornPosition;
        public Vector3 currentPosition;
        public WordsFamilyScript wordsFamilyScript;
    }

    public class WordsTrigger : MonoBehaviour
    {
        public List<WordInformation> wordsInformation;

        public UnityEvent OnWordsAdded; 
        public UnityEvent OnWordsRemoved;

        private void OnTriggerEnter(Collider other)
        {
            WordsFamilyScript wordsFamilyScript = other.attachedRigidbody.GetComponent<WordsFamilyScript>();

            if (wordsFamilyScript == null) return;

            HandleWordsAdded(wordsFamilyScript);
        }

        private void OnTriggerExit(Collider other)
        {
            WordsFamilyScript wordsFamilyScript = other.attachedRigidbody.GetComponent<WordsFamilyScript>();

            if (wordsFamilyScript == null) return;

            HandleWordsRemoved(wordsFamilyScript);
        }

        private void HandleWordsAdded(WordsFamilyScript wordsFamilyScript)
        {
            if (GetWordsInformation(wordsFamilyScript) != null) return;

            WordInformation newWordsInfo = new WordInformation();

            newWordsInfo.id                 = wordsFamilyScript.words_family.track_id;
            newWordsInfo.word               = GetWords(wordsFamilyScript);
            newWordsInfo.bornTime           = DateTime.Now;
            newWordsInfo.timeOnTheTable     = (DateTime.Now - newWordsInfo.bornTime).TotalSeconds;
            newWordsInfo.bornPosition       = wordsFamilyScript.words_symbols_transforms[0][0].position;
            newWordsInfo.currentPosition    = newWordsInfo.bornPosition;
            newWordsInfo.wordsFamilyScript  = wordsFamilyScript;

            wordsInformation.Add(newWordsInfo);

            OnWordsAdded.Invoke();
        }

        private void HandleWordsRemoved(WordsFamilyScript wordsFamilyScript)
        {
            if (GetWordsInformation(wordsFamilyScript) == null) return;

            wordsInformation.Remove(GetWordsInformation(wordsFamilyScript));

            OnWordsRemoved.Invoke();
        }

        private List<string> GetWords(WordsFamilyScript wordsFamilyScript)
        {
            List<string> out_words = new List<string>();

            foreach (var word_symbols_ids in wordsFamilyScript.words_symbols_ids)
            {
                out_words.Add(ConvertorIdToWords.ConvertToString(word_symbols_ids));
            }

            return out_words;
        }

        private WordInformation GetWordsInformation(WordsFamilyScript wordsFamilyScript)
        {
            WordInformation newWordsInfo = null;

            foreach (WordInformation wordInformation in wordsInformation)
            {
                if (wordsFamilyScript.words_family.track_id != wordInformation.id) continue;

                newWordsInfo = wordInformation;

                break;
            }

            return newWordsInfo;
        }
    }
}

