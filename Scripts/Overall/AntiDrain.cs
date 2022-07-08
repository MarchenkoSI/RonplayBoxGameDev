using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RonplayBoxGameDev
{
    public class AntiDrain : MonoBehaviour
    {
        public float teleportY = 2.0f;

        private void OnTriggerEnter(Collider other)
        {
            Vector3 pos = other.transform.position;

            pos.y = teleportY;

            other.transform.position = pos;
        }
    }
}
