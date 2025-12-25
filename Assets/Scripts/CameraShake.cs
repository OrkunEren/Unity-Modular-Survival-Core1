using Unity.Cinemachine;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    [SerializeField] Player player;
    [SerializeField] CinemachineBasicMultiChannelPerlin _cam;

    [SerializeField] float lerpFactor;
    [SerializeField] float walkFreq;
    [SerializeField] float runFreq;
    [SerializeField] float defaultFreq;

    float currnetFrequancy;

  

    private void Start()
    {
      currnetFrequancy = _cam.FrequencyGain;
        
    }
    private void Update()
    {
        SetFrequencyGain();
    }

    private void SetFrequencyGain()
    {
        if (player.characterController.velocity.magnitude > 1 && 3 > player.characterController.velocity.magnitude)
        { 
            _cam.FrequencyGain = Mathf.Lerp(currnetFrequancy, walkFreq, lerpFactor * Time.deltaTime);
            _cam.AmplitudeGain = .2f;
        }   
        else if (player.characterController.velocity.magnitude > 4)
        {
            _cam.FrequencyGain = Mathf.Lerp(currnetFrequancy, runFreq, lerpFactor * Time.deltaTime);
            _cam.AmplitudeGain = .5f;
        }
        else
        { 
            _cam.FrequencyGain = Mathf.Lerp(currnetFrequancy, defaultFreq, lerpFactor * Time.deltaTime);
            _cam.AmplitudeGain = .2f;
        }
    }
}
