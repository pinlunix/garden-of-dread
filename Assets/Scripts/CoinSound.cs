using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinSound : MonoBehaviour
{
    // Start is called before the first frame update
    public AudioSource Sound;

    void OnCollisionEnter(Collision collision)
    {
        Sound.Play();
    }


}
