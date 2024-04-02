using DG.Tweening;
using UnityEngine;

public class ButtonController : MonoBehaviour
{
    [SerializeField] private AudioSource _audioSource;
    [SerializeField] private AudioClip _audioClip;

    public void LoadScene( int index)
    {
        _audioSource.PlayOneShot(_audioClip);
        DOTween.KillAll();
        SceneManagerController.Instance.SwitchScene(index);
    }

    public void CloseApplication()
    {
        Application.Quit();
    }
}
