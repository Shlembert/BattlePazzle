using DG.Tweening;
using Plugins.Scripts.Dropbox;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class ImageDownloader : MonoBehaviour
{
    [SerializeField] private string serverURL;
    [SerializeField] private GameObject rawImagePrefab;
    [SerializeField] private Transform content;
    [SerializeField] private int imagesCount;
    [SerializeField] private GameObject downloadScreen;
    [SerializeField] private Image fill;

    private int _nextImageIndex = 1;
    private string _currentURL;
    private string _spaceAlbum = "Space/";
    //private string _superAlbum = "Super/";
    //private string _forestAlbum = "Forest/";
    //private string _fantasyAlbum = "Space/";

    private void Awake()
    {
        _currentURL = serverURL + _spaceAlbum;
    }

    private async void Start()
    {
        await DropboxHelper.Initialize();
        
        CreateEmptyPrefabs();
        StartCoroutine(AnimateDownloadScreen());
    }

    public void ChangeAlbum(string album)
    {
        _currentURL = serverURL +  album;
        _nextImageIndex = 1;
        DOTween.KillAll();

        for (int i = content.childCount - 1; i >= 0; i--)
        {
            DestroyImmediate(content.GetChild(i).gameObject);
        }

        CreateEmptyPrefabs();
    }

    public void CreateEmptyPrefabs()
    {
        for (int i = 0; i < imagesCount; i++)
        {
            GameObject rawImageObject = Instantiate(rawImagePrefab, content);
            rawImageObject.GetComponent<SpriteRenderer>().enabled = true;
        }
    }

    public void RequestNextImage(Action<Texture2D> callback)
    {
        if (_nextImageIndex > imagesCount)
        {
            Debug.LogError("All images have been requested.");
            callback(null);
            return;
        }

        GameObject rawImageObject = content.GetChild(_nextImageIndex - 1).gameObject;

        string imageURL = _currentURL + _nextImageIndex.ToString() + ".png";
        StartCoroutine(DownloadImage(imageURL, rawImageObject, callback));
        _nextImageIndex++;
    }

    private IEnumerator DownloadImage(string imageURL, GameObject rawImageObject, Action<Texture2D> callback)
    {
        using (var uwr = UnityWebRequestTexture.GetTexture(imageURL))
        {
            yield return uwr.SendWebRequest();

            if (uwr.result != UnityWebRequest.Result.Success)
            {
                callback(null);
                yield break;
            }

            Texture2D texture = ((DownloadHandlerTexture)uwr.downloadHandler).texture;
            callback(texture);
        }
    }

    private IEnumerator AnimateDownloadScreen()
    {
        float duration = 2f;
        float elapsedTime = 0f;
        float startFillAmount = fill.fillAmount;
        float targetFillAmount = 1f;

        while (elapsedTime < duration)
        {
            float fillAmount = Mathf.Lerp(startFillAmount, targetFillAmount, elapsedTime / duration);
            fill.fillAmount = fillAmount;

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        fill.gameObject.SetActive(false);

        // Fade out animation
        CanvasGroup canvasGroup = downloadScreen.GetComponent<CanvasGroup>();
        canvasGroup.DOFade(0f, 0.5f).SetEase(Ease.Linear).OnComplete(() =>
        {
            // Deactivate the download screen
            downloadScreen.SetActive(false);
        });
    }
}
