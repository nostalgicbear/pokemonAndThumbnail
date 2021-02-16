using System.Collections;
using UnityEngine.Networking;
using UnityEngine.UI;
using UnityEngine;
using System.IO;

public class ThumbnailDownloader : MonoBehaviour
{
    private string fullSavePath = "";
    private void Start()
    {
        fullSavePath = Application.persistentDataPath + "/Thumbnails/";
    }

    void SaveThumbnail(string fileName, byte[] bytes)
    {
        if (!Directory.Exists(fullSavePath))
        {
            Directory.CreateDirectory(fullSavePath);
        }

        string savePathIncludingFile = fullSavePath + fileName;
        File.WriteAllBytes(savePathIncludingFile, bytes);
    }

    public Texture2D LoadImageFromDisk(string url)
    {
        string fileToLoad = ExtractFileNameFromURL(url);
        if (File.Exists(fullSavePath + fileToLoad))
        {
            byte[] bytes = File.ReadAllBytes(fullSavePath + fileToLoad);
            Texture2D texture = new Texture2D(1, 1, TextureFormat.DXT5, false);
            texture.LoadImage(bytes);

            return texture;
        }

        return null;
    }


    public void OnDownloadComplete(Texture2D texture, string fileName, ImageComponentController imageComponentController)
    {
        SetRawImageThumbnailObject(imageComponentController, texture);
        SaveThumbnail(fileName, texture.EncodeToPNG());
    }

    public void StartThumbnailDownload(string thumbnailURL, ImageComponentController imageComponentController)
    {
        IEnumerator downloadObj = UpdateThumbnail(thumbnailURL, imageComponentController, OnDownloadComplete);
        StartCoroutine(downloadObj);
        
    }

    public void LoadThumbnail(string url, ImageComponentController imageComponentController)
    {
        Texture2D texture = LoadImageFromDisk(url);

        if (texture != null)
        {
            SetRawImageThumbnailObject(imageComponentController, texture);
        }
    }

    protected IEnumerator UpdateThumbnail(string thumbnailURL, ImageComponentController imageComponentController, System.Action<Texture2D, string, ImageComponentController> onDownloadComplete)
    {
        UnityWebRequest www = UnityWebRequestTexture.GetTexture(thumbnailURL);

        yield return www.SendWebRequest();

        while (!www.isDone)
        {
            yield return 0;
        }

        if (www.error == null)
        {
            Texture2D texture = ((DownloadHandlerTexture)www.downloadHandler).texture;

            onDownloadComplete?.Invoke(texture, ExtractFileNameFromURL(thumbnailURL), imageComponentController);

            onDownloadComplete = null;
        }

        www.downloadHandler.Dispose();
        www.Dispose();
    }

    private static string ExtractFileNameFromURL(string url)
    {
        int fileNameStartIndex = url.LastIndexOf("/");
        string fileName = url.Substring(fileNameStartIndex + 1, url.Length - fileNameStartIndex - 1);

        int extensionStartIndex = fileName.LastIndexOf(".");
        fileName = fileName.Remove(extensionStartIndex, fileName.Length - extensionStartIndex);
        fileName += ".png";

        return fileName;
    }

    public void SetRawImageThumbnailObject(ImageComponentController imageComponentController, Texture2D texture)
    {
        if(imageComponentController.UsingRawImageComponent == true)
        {
            RawImage ri = imageComponentController.GetComponent<RawImage>();
            ri.color = Color.white;
            ri.texture = texture;
        } else
        {
            Image image = imageComponentController.GetComponent<Image>();
            image.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), imageComponentController.GetComponent<RectTransform>().pivot);
        }

        imageComponentController.ImageAssignedSuccessfully = true;
    }

}

