using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuController : MonoBehaviour
{
    public GameObject UploadMenu;
    public GameObject DownloadMenu;

    // Start is called before the first frame update
    void Start()
    {
        UploadMenu.SetActive(false);
    }

    public void ShowUploadMenu(bool _show)
    {
        UploadMenu.SetActive(_show);
    }

    public void Upload(string _roomID, GameObject[] _items)
    {
        StartCoroutine(SendUploadRequest(_roomID, _items));
    }

    private IEnumerator SendUploadRequest(string _roomID, GameObject[] _items)
    {
        yield return null;
    }

    public void ShowDownloadMenu(bool _show)
    {
        DownloadMenu.SetActive(_show);
    }

    public void Download(string _roomID)
    {
        StartCoroutine(SendDownloadRequest(_roomID));
    }

    private IEnumerator SendDownloadRequest(string _roomID)
    {
        yield return null;
    }
}
