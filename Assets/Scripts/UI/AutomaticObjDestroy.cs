using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutomaticObjDestroy : MonoBehaviour
{
    [SerializeField] float deleteIn = 5f;

    private void Awake()
    {
        Destroy(gameObject, deleteIn);
    }
}
