using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine; 

public class CameraShake : MonoBehaviour
{

    public static CameraShake Instance { get; private set; }
    private CinemachineVirtualCamera cinemachineCam;
    private CinemachineBasicMultiChannelPerlin cinemachinePerlin; 
    private float shakeTimer; 
    // Start is called before the first frame update
    void Awake()
    {
        Instance = this; 
        cinemachineCam = GetComponent<CinemachineVirtualCamera>();
        cinemachinePerlin = cinemachineCam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();

    }

    // Call it like: CinemachineShake.Instance.ShakeCamera(5f, .1f);
    public void ShakeCamera(float intensity, float time)
    {
        cinemachinePerlin.m_AmplitudeGain = intensity;
        shakeTimer = time; 
    }

    private void Update()
    {
        shakeTimer -= Time.deltaTime; 

        if(shakeTimer <= 0f)
        {
            cinemachinePerlin.m_AmplitudeGain = 0f;

        }
    }
}
