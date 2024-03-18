using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AlbumChanger : MonoBehaviour
{
    [SerializeField] private List<Button> buttons;
    [SerializeField] private Color enableColor, disableColor;
    
    private ImageDownloader _imageDownloader;
    
    private string _buttonAlbumName;

    public string ButtonAlbumName { get => _buttonAlbumName; set => _buttonAlbumName = value; }

    private void Start()
    {
        _imageDownloader = GetComponent<ImageDownloader>();
    }

    public void ActiveOtherButtons(Button pressedButton)
    {
        foreach (var button in buttons)
        {
            button.interactable = true;
            button.transform.GetChild(1).GetComponent<TMP_Text>().color = enableColor;
        } 

        pressedButton.interactable = false;
        pressedButton.transform.GetChild(1).GetComponent<TMP_Text>().color = disableColor;

        _imageDownloader.ChangeAlbum(GetAlbumName(pressedButton.name));
    }

    public void ButtonState(string buttonName)
    {
        foreach (var button in buttons) 
        {
            if (buttonName == button.gameObject.name)
            {
                button.interactable = false;
                button.transform.GetChild(1).GetComponent<TMP_Text>().color = disableColor;
            }
            else 
            {
                button.interactable = true;
                button.transform.GetChild(1).GetComponent<TMP_Text>().color = enableColor;
            } 
        } 
    }

    private string GetAlbumName(string buttonName)
    {
        string albumName = buttonName.Substring(6) + "/";

        return albumName;
    }
}
