using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;
using UnityEngine.UI;
using UnityEngine;
using UnityEditor;

public class ThumbnailDownloader : MonoBehaviour
{

    void SaveThumbnail(string subDirectory, byte[] bytes)
    {
        if (!System.IO.Directory.Exists(Application.persistentDataPath + "/Thumbnails/" + subDirectory))
        {
            System.IO.Directory.CreateDirectory(Application.persistentDataPath + "/Thumbnails/" + subDirectory);
        }

        System.IO.FileStream file = System.IO.File.Open(Application.persistentDataPath + "/Thumbnails/" + subDirectory +  "image.png", System.IO.FileMode.Create);
        System.IO.BinaryWriter binary = new System.IO.BinaryWriter(file);
        binary.Write(bytes);
        binary.Close();
        file.Close();
    }

    public Texture2D LoadImage(string subDirectory)
    {
        if (System.IO.File.Exists(Application.persistentDataPath + "/Thumbnails/" + subDirectory + "image.png"))
        {
            byte[] bytes = System.IO.File.ReadAllBytes(Application.persistentDataPath + "/Thumbnails/" + subDirectory + "image.png");
            Texture2D texture = new Texture2D(1, 1, TextureFormat.DXT5, false);
            texture.LoadImage(bytes);

            return texture;
            //SetRawImageThumbnailObject(thumbnail, texture, thumbnailMinimumHeight, thumbnailMaxHeight);
        }

        return null;
    }


    public void OnDownloadComplete(Texture2D texture, string subDirectory, RawImage graphic)
    {

        SetRawImageThumbnailObject(graphic, texture);

        SaveThumbnail(subDirectory, texture.EncodeToPNG());
    }

    public void StartThumbnailDownload(string thumbnailURI, string subDirectory, RawImage thumbnail)
    {

        IEnumerator downloadObj = UpdateThumbnail(thumbnailURI, thumbnail, subDirectory, OnDownloadComplete);
        StartCoroutine(downloadObj);
    }

    public void LoadThumbnail(string subDirectory, RawImage thumbnail)
    {
        Texture2D texture = LoadImage(subDirectory);

        if (texture != null)
        {
            SetRawImageThumbnailObject(thumbnail, texture);
        }
    }

    protected IEnumerator UpdateThumbnail(string thumbnailURI, RawImage graphic, string subDirectory, System.Action<Texture2D, string, RawImage> onDownloadComplete)
    {
        UnityWebRequest www = UnityWebRequestTexture.GetTexture(thumbnailURI);

        yield return www.SendWebRequest();

        while (!www.isDone)
        {
            yield return 0;
        }

        if (www.error == null)
        {
            Texture2D texture = ((DownloadHandlerTexture)www.downloadHandler).texture;

            if (onDownloadComplete != null)
            {
                onDownloadComplete(((DownloadHandlerTexture)www.downloadHandler).texture, subDirectory, graphic);
            }

            onDownloadComplete = null;

        }

        www.downloadHandler.Dispose();
        www.Dispose();
    }

    public void SetRawImageThumbnailObject(RawImage thumbnail, Texture2D texture)
    {

        thumbnail.color = Color.white;
        thumbnail.texture = texture;

    }



}

