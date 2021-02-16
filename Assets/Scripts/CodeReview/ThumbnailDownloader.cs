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


    public void OnDownloadComplete(Texture2D texture, string fileName, ImageComponentController sri)
    {
        SetRawImageThumbnailObject(sri, texture);
        SaveThumbnail(fileName, texture.EncodeToPNG());
    }

    public void StartThumbnailDownload(string thumbnailURL, ImageComponentController sri)
    {
        IEnumerator downloadObj = UpdateThumbnail(thumbnailURL, sri, OnDownloadComplete);
        StartCoroutine(downloadObj);
    }

    public void LoadThumbnail(string url, ImageComponentController sri)
    {
        Texture2D texture = LoadImageFromDisk(url);
        if (texture != null)
        {
            SetRawImageThumbnailObject(sri, texture);
        }
    }

    protected IEnumerator UpdateThumbnail(string thumbnailURL, ImageComponentController sri, System.Action<Texture2D, string, ImageComponentController> onDownloadComplete)
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

            onDownloadComplete?.Invoke(texture, ExtractFileNameFromURL(thumbnailURL), sri);

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

    public void SetRawImageThumbnailObject(ImageComponentController sri, Texture2D texture)
    {
        if(sri.UsingRawImageComponent == true)
        {
            RawImage ri = sri.GetComponent<RawImage>();
            ri.color = Color.white;
            ri.texture = texture;
        } else
        {
            Image image = sri.GetComponent<Image>();
            image.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), sri.GetComponent<RectTransform>().pivot);
        }

        sri.ImageAssignedSuccessfully = true;
    }

}

