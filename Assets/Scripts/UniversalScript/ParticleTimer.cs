using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleTimer : MonoBehaviour
{

    void Start()
    {
        GetComponent<ParticleSystem>().time = 150;
    }
}
