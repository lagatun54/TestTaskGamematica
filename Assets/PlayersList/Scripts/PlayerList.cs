using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerList : MonoBehaviour
{
    private const string UsersUrl = "https://dfu8aq28s73xi.cloudfront.net/testUsers";

    [Serializable]
    private struct PlayerData
    {
        public string Username;
        public string AvatarUrl;
        public int Points;
    }

    [SerializeField] private TMPro.TMP_Text infoText;
    [SerializeField] private PlayerItem playerPrefab;
    [SerializeField] private Texture2D defaultAvatar;
    [SerializeField] private Texture2D loadingTexture;

    private IEnumerator Start()
    {
        infoText.enabled = true;
        infoText.text = "Loading...";
        var request = UnityWebRequest.Get(UsersUrl);
        yield return request.SendWebRequest();
        if (request.error != null)
        {
            infoText.text = request.error;
            Debug.LogWarning("No connection");
            yield break;
        }

        if (request.isDone)
        {
            var list = JsonHelper.JsonHelper.GetArray<PlayerData>(request.downloadHandler.text);
            Array.Sort(list, SortByScore);
            foreach (var player in list)
                AddPlayer(player);
        }
        infoText.enabled = false;
    }

    private int SortByScore(PlayerData data1, PlayerData data2) => data2.Points.CompareTo(data1.Points);

    private IEnumerator RequestAvatar(string url, PlayerItem player)
    {
        var request = UnityWebRequestTexture.GetTexture(url);
        yield return request.SendWebRequest();
        if (request.error != null)
        {
            Debug.LogWarning(request.error);
            player.SetAvatar(defaultAvatar);
            yield break;
        }

        if (request.isDone)
        {
            var avatar = ((DownloadHandlerTexture)request.downloadHandler).texture;
            player.SetAvatar(avatar);
        }
    }

    private void AddPlayer(PlayerData data)
    {
        var player = Instantiate(playerPrefab, transform);
        player.Init(data.Username, data.Points);
        player.SetAvatar(loadingTexture);
        StartCoroutine(RequestAvatar(data.AvatarUrl, player));
    }
}
