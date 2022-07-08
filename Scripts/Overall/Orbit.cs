using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RonplayBoxGameDev
{
    public class Orbit : MonoBehaviour
    {
        [Range(-10, 10)]
        public float speed;

        public bool rotateX, rotateY, rotateZ;

        private void Update()
        {
            float x = rotateX ? transform.eulerAngles.x + speed * Time.deltaTime : 0;
            float y = rotateY ? transform.eulerAngles.y + speed * Time.deltaTime : 0;
            float z = rotateZ ? transform.eulerAngles.z + speed * Time.deltaTime : 0;

            transform.eulerAngles = new Vector3(x, y, z);
        }
    }
}

