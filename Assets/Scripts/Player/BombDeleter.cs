using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombDeleter : MonoBehaviour
{
    public void DeleteMeIn(float time)
    {
        Destroy(gameObject, time);
    }
}
