using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityRonplayBoxSDK;

namespace RonplayBoxGameDev
{
    public class WallCollideTrigger : MonoBehaviour
    {
        public Action<Transform> OnTriggered;

        private void OnTriggerEnter(Collider other)
        {
            WallCollisionScript wallCollisionScript = other.GetComponent<WallCollisionScript>();

            if (wallCollisionScript == null) return;

            OnTriggered.Invoke(other.transform);
        }
    }
}

