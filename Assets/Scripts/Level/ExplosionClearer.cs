using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosionClearer : MonoBehaviour
{
    // Start is called before the first frame update
        private void Start()
        {
            Destroy(gameObject, 1.5f);
        }
}
