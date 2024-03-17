using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ImageController : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    private ImageDownloader imageDownloader;
    private RawImage rawImage;
    private Image rotatingImage;
    private bool isRequested = false;
    private Vector2 _offsetScrool;

    private void Start()
    {
        StartImageDownload();
    }

    public void StartImageDownload()
    {
        imageDownloader = FindObjectOfType<ImageDownloader>();
        rawImage = GetComponent<RawImage>();
        rotatingImage = GetComponentInChildren<Image>();

        StartRotationAnimation();
    }

    private void StartRotationAnimation()
    {
        rotatingImage.transform.DORotate(new Vector3(0f, 0f, 360f), 1f, RotateMode.FastBeyond360)
            .SetLoops(-1, LoopType.Restart)
            .SetEase(Ease.Linear);
    }

    private void StopRotationAnimation()
    {
        rotatingImage.transform.DOKill();
    }

    private void OnBecameVisible()
    {
        if (!isRequested)
        {
            RequestImage();
        }
    }

    private void RequestImage()
    {
        imageDownloader.RequestNextImage(SetImage);
    }

    private void SetImage(Texture2D texture)
    {
        if (texture != null)
        {
            rawImage.texture = texture;
            isRequested = true;

            // ќстанавливаем анимацию вращени€ и пр€чем объект
            StopRotationAnimation();
            rotatingImage.gameObject.SetActive(false);
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (!isRequested) return;

        _offsetScrool = Input.mousePosition;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (Vector2.Distance(Input.mousePosition, _offsetScrool) > 10f) return;

        SaveTextureToPlayerPrefs(rawImage.texture as Texture2D);
    }


    private void SaveTextureToPlayerPrefs(Texture2D texture)
    {
        // —оздаем новую текстуру с такими же размерами и без сжати€
        Texture2D newTexture = new Texture2D(texture.width, texture.height, TextureFormat.RGBA32, false);
        newTexture.SetPixels(texture.GetPixels());
        newTexture.Apply();

        // —охран€ем новую текстуру
        byte[] textureBytes = newTexture.EncodeToPNG();
        string textureData = System.Convert.ToBase64String(textureBytes);
        PlayerPrefs.SetString("SavedTexture", textureData);

        DOTween.KillAll();
        SceneManagerController.Instance.SwitchScene(2);
    }
}
