using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class MenuController : MonoBehaviour
{
    [System.Serializable]
    public struct Item
    {
        public string roomId;
        public string itemId;
        public string position;
        public string rotation;
    }

    public RaycastItem[] RaycastItems;

    public GameObject UploadMenu;
    public GameObject DownloadMenu;
    public Button RoomSelectButtonPrefab;
    public Text RoomIdInput;
    public Transform AvailableRoomsParent;

    // Start is called before the first frame update
    void Start()
    {
        UploadMenu.SetActive(false);
    }

    public void ShowUploadMenu(bool _show)
    {
        UploadMenu.SetActive(_show);
    }

    public void Upload()
    {
        StartCoroutine(SendUploadRequest());
    }

    private IEnumerator SendUploadRequest()
    {
        string json = "{\"items\":[";
        for (int i = 0; i < RaycastItems.Length; i++)
        {
            Item item = new Item();
            item.itemId = RaycastItems[i].name;
            item.roomId = RoomIdInput.text;
            item.position = RaycastItems[i].transform.position.ToString();
            item.rotation = RaycastItems[i].transform.rotation.ToString();
            json += JsonUtility.ToJson(item);
            if (i < RaycastItems.Length - 1)
                json += ",";
        }
        json += "]}";
        Debug.Log(json);

        UnityWebRequest www = UnityWebRequest.Post("https://2zipwmez7a.execute-api.us-east-1.amazonaws.com/dev/item", json);
        yield return www.SendWebRequest();

        if (www.isNetworkError || www.isHttpError)
        {
            Debug.Log(www.error);
        }
        else
        {
            // Show results as text
            Debug.Log(www.responseCode);
        }
    }

    public void ShowDownloadMenu(bool _show)
    {
        DownloadMenu.SetActive(_show);
    }

    public void Download(string _roomId)
    {
        StartCoroutine(SendDownloadRequest(_roomId));
    }

    private IEnumerator ListRooms()
    {
        UnityWebRequest www = UnityWebRequest.Get("https://2zipwmez7a.execute-api.us-east-1.amazonaws.com/dev/room");
        yield return www.SendWebRequest();

        if (www.isNetworkError || www.isHttpError)
        {
            Debug.Log(www.error);
        }
        else
        {
            Debug.Log("Data: " + www.downloadHandler.text);
            Item[] items = JsonHelper.FromJson<Item>(www.downloadHandler.text);

            List<string> uniqueRoomIds = new List<string>();
            foreach(Item item in items)
            {
                if (!uniqueRoomIds.Contains(item.roomId))
                    uniqueRoomIds.Add(item.roomId);
            }
        }
    }

    private IEnumerator SendDownloadRequest(string _roomId)
    {
        UnityWebRequest www = UnityWebRequest.Get("https://2zipwmez7a.execute-api.us-east-1.amazonaws.com/dev/item?roomId=" + _roomId);
        yield return www.SendWebRequest();
    }
}
