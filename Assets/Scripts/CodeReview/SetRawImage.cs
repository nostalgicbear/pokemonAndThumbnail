using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SetRawImage : MonoBehaviour
{


    public void RefreshImage(string url, ThumbnailDownloader downloader)
    {
        RawImage rawImage = GetComponent<RawImage>();

        downloader.LoadThumbnail("Thumbnails", rawImage);
        if (Application.internetReachability != NetworkReachability.NotReachable)
        {
            downloader.StartThumbnailDownload(url, "Thumbnails", rawImage);
        }
    }
}
