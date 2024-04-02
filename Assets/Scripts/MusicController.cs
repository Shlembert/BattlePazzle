using UnityEngine;

public class MusicController : MonoBehaviour
{
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private  GameObject imageOff;
    private bool _mute;

    public void MusicMute()
    {
        _mute = !_mute;
        musicSource.mute = _mute;
        imageOff.SetActive(_mute);
    }
}
