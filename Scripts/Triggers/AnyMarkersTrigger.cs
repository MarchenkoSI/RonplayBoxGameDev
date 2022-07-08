using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityRonplayBoxSDK;

namespace RonplayBoxGameDev
{
    [Serializable]
    public struct MarkerInfo
    {
        public int id;
        public Vector3 bornPosition;
        public Vector3 currentPosition;
        public DateTime bornTime;
        public double lifeTime;
        public FamilyOfSymbolsScript symbolsScript;
    }

    public class AnyMarkersTrigger : MonoBehaviour
    {
        public List<MarkerInfo> markersInfo;

        public UnityEvent OnMarkerAdded;
        public UnityEvent OnMarkerRemoved;

        private void OnTriggerEnter(Collider other)
        {
            FamilyOfSymbolsScript family_of_symbols_script = other.GetComponent<FamilyOfSymbolsScript>();

            if (family_of_symbols_script == null) return;

            AddMarker(family_of_symbols_script);
        }

        private void OnTriggerExit(Collider other)
        {
            FamilyOfSymbolsScript family_of_symbols_script = other.GetComponent<FamilyOfSymbolsScript>();

            if (family_of_symbols_script == null) return;

            RemoveMarker(family_of_symbols_script);
        }

        private void AddMarker(FamilyOfSymbolsScript marker)
        {
            MarkerInfo newMarker = new MarkerInfo()
            {
                id = marker.familyOfSymbols.track_id,
                bornPosition = marker.transform.position,
                currentPosition = marker.transform.position,
                bornTime = DateTime.Now,
                lifeTime = 0,
                symbolsScript = marker
            };

            markersInfo.Add(newMarker);

            OnMarkerAdded.Invoke();
        }

        private void RemoveMarker(FamilyOfSymbolsScript marker)
        {
            MarkerInfo currentMarkerInfo = GetMarkerInfo(marker);

            if (currentMarkerInfo.id != marker.familyOfSymbols.track_id) return;

            markersInfo.Remove(currentMarkerInfo);

            OnMarkerRemoved.Invoke();
        }

        private MarkerInfo GetMarkerInfo(FamilyOfSymbolsScript marker)
        {
            MarkerInfo target = new MarkerInfo();

            foreach(MarkerInfo markerInfo in markersInfo)
            {
                if (markerInfo.id != marker.familyOfSymbols.track_id) continue;

                target = markerInfo;

                break;
            }

            return target;
        }

        // TODO implement update MarkerInfo.
    }
}

