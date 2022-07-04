using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityRonplayBoxSDK;

namespace RonplayBoxGameDev
{
    [System.Serializable]
    public class SymbolInformation
    {
        public int id;
        public string symbolAsString;
        public DateTime bornTime;
        public double timeOnTheTable;
        public Vector3 bornPosition;
        public Vector3 currentPosition;
        public FamilyOfSymbolsScript familyOfSymbolsScript;
    }

    public class SymbolsTrigger : MonoBehaviour
    {
        public List<SymbolInformation> symbolInformation;

        public UnityEvent OnWordsAdded;
        public UnityEvent OnWordsRemoved;

        private void FixedUpdate()
        {
            UpdateSymbolInformation();
        }

        private void OnTriggerEnter(Collider other)
        {
            FamilyOfSymbolsScript family_of_symbols_script = other.GetComponent<FamilyOfSymbolsScript>();

            if (family_of_symbols_script == null) return;

            if (_family_of_symbols_scripts.Contains(family_of_symbols_script)) return;

            _family_of_symbols_scripts.Add(family_of_symbols_script);

            OnWordsFamilyAdded(family_of_symbols_script);
        }

        private void OnTriggerExit(Collider other)
        {
            FamilyOfSymbolsScript family_of_symbols_script = other.GetComponent<FamilyOfSymbolsScript>();

            if (family_of_symbols_script == null) return;

            if (!_family_of_symbols_scripts.Contains(family_of_symbols_script)) return;

            _family_of_symbols_scripts.Remove(family_of_symbols_script);

            RemoveWord(family_of_symbols_script);
        }

        private void OnWordsFamilyAdded(FamilyOfSymbolsScript family_of_symbols_script_)
        {
            if (_family_of_symbols_script_to_born_time_seconds.ContainsKey(family_of_symbols_script_)) return;

            _family_of_symbols_script_to_born_time_seconds.Add
            (
                family_of_symbols_script_,
                Time.time
            );

            AddWord(family_of_symbols_script_);
        }

        private void AddWord(FamilyOfSymbolsScript family_of_symbols_script_)
        {
            SymbolInformation symbol = new SymbolInformation();

            symbol.id = family_of_symbols_script_.familyOfSymbols.track_id;

            if (ContainsInWordsInformation(symbol.id)) return;

            var current_symbol_id = family_of_symbols_script_.familyOfSymbols.complex_symbols_ids_and_clockwise_rotations[0].id;

            //List<string> symbol_as_string = ConvertWordSymbolsIdsToString(family_of_symbols_script_.familyOfSymbols.symbols);

            symbol.symbolAsString = AlphabetDatabase.Instance.Get(0).letter[current_symbol_id];// symbol_as_string;

            symbol.bornTime = DateTime.Now;
            symbol.timeOnTheTable = (DateTime.Now - symbol.bornTime).TotalSeconds;
            symbol.bornPosition = family_of_symbols_script_.gameObject.transform.position;
            symbol.currentPosition = symbol.bornPosition;
            symbol.familyOfSymbolsScript = family_of_symbols_script_;

            symbolInformation.Add(symbol);
            OnWordsAdded.Invoke();
        }

        private void RemoveWord(FamilyOfSymbolsScript family_of_symbols_script_)
        {
            int id = family_of_symbols_script_.familyOfSymbols.track_id;

            symbolInformation.Remove(GetSymbolByID(id));

            _family_of_symbols_script_to_born_time_seconds.Remove(family_of_symbols_script_);

            OnWordsRemoved.Invoke();
        }

        private SymbolInformation GetSymbolByID(int id)
        {
            SymbolInformation our_symbol = new SymbolInformation();

            bool is_symbol_found = false;

            for (int index = 0; index < symbolInformation.Count && !is_symbol_found; ++index)
            {
                if (symbolInformation[index].id != id) continue;

                is_symbol_found = true;

                our_symbol = symbolInformation[index];
            }

            return our_symbol;
        }

        //private List<string> ConvertWordSymbolsIdsToString(List<SymbolInfo> symbol_info_)
        //{
        //    List<string> out_symbols_list = new List<string>();

        //    string out_word = "";

        //    for (int symbol_index = 0; symbol_index < symbol_info_.Count; symbol_index++)
        //    {
        //        char    symbol_as_char      = char.Parse(char.ConvertFromUtf32(symbol_info_[symbol_index].symbol_id));
        //        string  symbol_as_string    = char.ConvertFromUtf32(symbol_info_[symbol_index].symbol_id);
                
        //        out_word += symbol_as_char;

        //        out_symbols_list.Add(symbol_as_string);
        //    }

        //    return out_symbols_list;
        //}

        private bool ContainsInWordsInformation(int id_)
        {
            bool contain = false;

            for (int index = 0; index < symbolInformation.Count && !contain; ++index)
            {
                if (symbolInformation[index].id != id_) continue;

                contain = true;
            }

            return contain;
        }

        private void UpdateSymbolInformation()
        {
            foreach (var symbol_information in symbolInformation)
            {
                FamilyOfSymbolsScript symbols_family_script = symbol_information.familyOfSymbolsScript;

                symbol_information.currentPosition = symbols_family_script.gameObject.transform.position;

                symbol_information.timeOnTheTable = (DateTime.Now - symbol_information.bornTime).TotalSeconds;
            }
        }

        Dictionary<FamilyOfSymbolsScript, float> _family_of_symbols_script_to_born_time_seconds =
            new Dictionary<FamilyOfSymbolsScript, float>();

        private List<FamilyOfSymbolsScript> _family_of_symbols_scripts =
            new List<FamilyOfSymbolsScript>();
    }
}

