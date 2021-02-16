using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateEntries : MonoBehaviour
{
    [SerializeField]
    GameObject entryPrefab;

    [SerializeField]
    Transform parent;

    [SerializeField]
    string[] urls;

    [SerializeField]
    ThumbnailDownloader downloader;

    public void RefreshEntries()
    {
        for(int i = 0; i < urls.Length; i++)
        {
            GameObject go = Instantiate(entryPrefab, parent);

            ImageComponentController imageComponentController = go.GetComponent<ImageComponentController>();

            imageComponentController.RefreshImage(urls[i], downloader);
        }
    }
}
