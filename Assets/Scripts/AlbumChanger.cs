using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AlbumChanger : MonoBehaviour
{
    [SerializeField] private List<Button> buttons;

    private ImageDownloader _imageDownloader;

    private void Start()
    {
        _imageDownloader = GetComponent<ImageDownloader>();
    }

    public void ActiveOtherButtons(Button pressedButton)
    {
        foreach (var button in buttons) button.interactable = true;
        pressedButton.interactable = false;
        _imageDownloader.ChangeAlbum(GetAlbumName(pressedButton.name));
    }

    private string GetAlbumName(string buttonName)
    {
        string albumName = buttonName.Substring(6) + "/";

        return albumName;
    }
}
