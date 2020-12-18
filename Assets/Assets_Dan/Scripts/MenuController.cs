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
    public GameObject RoomSelectButtonPrefab;
    public Text RoomIdInput;
    public Transform AvailableRoomsParent;
    public Text UploadErrorText;
    public Text DownloadErrorText;

    // Start is called before the first frame update
    void Start()
    {
        UploadMenu.SetActive(false);
        DownloadMenu.SetActive(false);
    }

    public void CloseApplication()
    {
        Application.Quit();
    }

    public void ShowUploadMenu(bool _show)
    {
        DownloadMenu.SetActive(false);
        UploadMenu.SetActive(_show);
        UploadErrorText.gameObject.SetActive(false);
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

        UnityWebRequest www = UnityWebRequest.Put("https://2zipwmez7a.execute-api.us-east-1.amazonaws.com/dev/item", json);
        www.SetRequestHeader("Content-Type", "application/json");
        yield return www.SendWebRequest();

        if (www.isNetworkError || www.isHttpError)
        {
            Debug.Log(www.error);
            UploadErrorText.gameObject.SetActive(true);
            UploadErrorText.text = www.error;
        }
        else
        {
            // Show results as text
            Debug.Log(www.responseCode);
            ShowUploadMenu(false);
        }
    }

    public void ShowDownloadMenu(bool _show)
    {
        UploadMenu.SetActive(false);
        DownloadMenu.SetActive(_show);
        DownloadErrorText.gameObject.SetActive(false);
        StartCoroutine(ListRooms());
    }

    public void Download(string _roomId)
    {
        StartCoroutine(SendDownloadRequest(_roomId));
    }

    private IEnumerator ListRooms()
    {
        for (int i = 0; i < AvailableRoomsParent.childCount; i++)
        {
            Destroy(AvailableRoomsParent.GetChild(i).gameObject);
        }

        UnityWebRequest www = UnityWebRequest.Get("https://2zipwmez7a.execute-api.us-east-1.amazonaws.com/dev/room");
        yield return www.SendWebRequest();

        if (www.isNetworkError || www.isHttpError)
        {
            Debug.Log(www.error);
        }
        else
        {
            Item[] items = JsonHelper.FromJson<Item>(www.downloadHandler.text);

            // Filter out duplicate RoomId entries
            List<string> uniqueRoomIds = new List<string>();
            foreach(Item item in items)
            {
                if (!uniqueRoomIds.Contains(item.roomId))
                    uniqueRoomIds.Add(item.roomId);
            }

            // Populate room download buttons
            foreach(string s in uniqueRoomIds)
            {
                Button button = Instantiate(RoomSelectButtonPrefab).GetComponent<Button>();
                button.transform.GetChild(0).GetComponent<Text>().text = s;
                button.onClick.AddListener(delegate { Download(s); });
                button.transform.SetParent(AvailableRoomsParent);
            }
        }
    }

    private IEnumerator SendDownloadRequest(string _roomId)
    {
        UnityWebRequest www = UnityWebRequest.Get("https://2zipwmez7a.execute-api.us-east-1.amazonaws.com/dev/item?roomId=" + _roomId);
        yield return www.SendWebRequest();

        if (www.isNetworkError || www.isHttpError)
        {
            Debug.Log(www.error);
            DownloadErrorText.gameObject.SetActive(true);
            DownloadErrorText.text = www.error;
        }
        else
        {
            // Show results as text
            Item[] items = JsonHelper.FromJson<Item>(www.downloadHandler.text);

            foreach (Item item in items)
            {
                foreach (RaycastItem rc in RaycastItems)
                {
                    if (item.itemId == rc.name)
                    {
                        // Position
                        string position = item.position.Trim(new char[] { ' ', '(', ')' });
                        string[] pComponents = position.Split(',');
                        rc.transform.position = new Vector3(
                            float.Parse(pComponents[0]), 
                            float.Parse(pComponents[1]), 
                            float.Parse(pComponents[2]));

                        // Rotation
                        string rotation = item.rotation.Trim(new char[] { ' ', '(', ')' });
                        string[] rComponents = rotation.Split(',');
                        rc.transform.rotation = new Quaternion(
                            float.Parse(rComponents[0]),
                            float.Parse(rComponents[1]),
                            float.Parse(rComponents[2]),
                            float.Parse(rComponents[3]));
                    }
                }
            }

            ShowDownloadMenu(false);
        }
    }
}
