using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class PlayerBehaviour : MonoBehaviour
{
    [SerializeField] 
    private AudioSource _audio;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ShootSound()
    {
        _audio.Play();
    }
}
