using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MicrophoneScript : MonoBehaviour
{
    AudioSource _audioSource;
    // Microphone input
    public AudioClip _audioClip;
    public bool _useMicrophone;
    public string _selectedDevice;

    // Start is called before the first frame update
    void Start()
    {
        _audioSource = GetComponent<AudioSource>();
        if(_useMicrophone){
            if(Microphone.devices.Length > 0){
                _selectedDevice = Microphone.devices[0].ToString();
                _audioSource.clip = Microphone.Start(_selectedDevice, true, 10, AudioSettings.outputSampleRate);
                _audioSource.loop = true;
                _audioSource.Play();
            }
            else{
                _useMicrophone = false;
            }
        }
        if(!_useMicrophone){
            _audioSource.clip = _audioClip;
        }

        
    }

    // Update is called once per frame
    void Update()
    {
        //_audioSource.Play();
    }
}
