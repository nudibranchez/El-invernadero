using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class VolumeController : MonoBehaviour
{
    public AudioMixer mixer;
    public Slider volumeSlider;

    void Start()
    {
        float value;
        mixer.GetFloat("MasterAudio", out value);
        volumeSlider.value = Mathf.Pow(10f, value / 20f);
    }

    public void SetVolume(Slider slider)
    {
        float dB = Mathf.Log10(Mathf.Clamp(slider.value, 0.0001f, 1f)) * 20f;
        mixer.SetFloat("MasterAudio", dB);
        Debug.Log("Volume set to: " + dB);
    }
}
