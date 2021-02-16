using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ImageComponentController : MonoBehaviour
{
    private bool usingRawImageComponent = false;
    public bool UsingRawImageComponent { get => usingRawImageComponent; }

    private bool imageAssignedSuccessfully;
    public bool ImageAssignedSuccessfully { get => imageAssignedSuccessfully; set => imageAssignedSuccessfully = value; }


    private void Awake()
    {
        Setup();
    }

    private void Setup()
    {
        if(GetComponent<RawImage>() == null && GetComponent<Image>() == null)
        {
            Debug.LogError("Neither an Image component or a RawImage component is attached");
            //Could add one automatically here as a default but thats prob overkill for now

        }
        else if (GetComponent<RawImage>() != null)
        {
            usingRawImageComponent = true;
        }
        else {
            usingRawImageComponent = false;
        }
        
    }

    public void RefreshImage(string url, ThumbnailDownloader downloader)
    {
        
        downloader.LoadThumbnail(url, this);

        //Only download if we have not successfully loaded it from a local directory
        if (imageAssignedSuccessfully == false)
        {
            if (Application.internetReachability != NetworkReachability.NotReachable)
            {
                downloader.StartThumbnailDownload(url, this);
            }
        }
        
    }
}
