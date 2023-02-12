using System;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class Game : MonoBehaviour
{
	[Serializable]
	public struct PlayerGame
	{
		public string Username;
		public string AvatarUrl;
		public int Points;
		public Sprite Icon;
	}
	[SerializeField] Sprite defaultIcon;

	private PlayerGame[] _playerGames;
	void Start ()
	{
		StartCoroutine (GetPlayer ());
	}

	void DrawUI ()
	{
		GameObject playerIcon = GameObject.FindGameObjectWithTag("Button");
		GameObject instantiatePlayer;

		var sorted = from playerGame in _playerGames
			orderby playerGame.Points descending
			select playerGame;
		foreach (var p in sorted)
		{
			instantiatePlayer = Instantiate (playerIcon, transform);
			instantiatePlayer.transform.GetChild (0).GetComponent <Image>().sprite = p.Icon;
			instantiatePlayer.transform.GetChild (1).GetComponent <Text> ().text = p.Username;
			instantiatePlayer.transform.GetChild (2).GetComponent <Text> ().text = p.Points.ToString();
		}
		Destroy (playerIcon);
	}

	private IEnumerator GetPlayer ()
	{
		string url = "https://dfu8aq28s73xi.cloudfront.net/testUsers";

		UnityWebRequest request = UnityWebRequest.Get (url);
		yield return request.SendWebRequest();

		if (request.error != null) {
			//show message "no internet "
			Debug.Log("No connection");
		} else {
			if (request.isDone) {
				_playerGames = JsonHelper.JsonHelper.GetArray<PlayerGame> (request.downloadHandler.text);
				StartCoroutine (GetPlayerIcons ());
			}
		}
	}

	private IEnumerator GetPlayerIcons ()
	{
		for (int i = 0; i < _playerGames.Length; i++) {
			
			UnityWebRequest request = UnityWebRequestTexture.GetTexture(_playerGames[i].AvatarUrl);
			Debug.Log(_playerGames[i].AvatarUrl);
			yield return request.SendWebRequest();

			if (request.error != null) {
				//error
				//show default image
				Debug.Log(request.error);
				_playerGames[i].Icon = defaultIcon;
			} 
			else {
				if (request.isDone)
				{
					Texture2D tx = ((DownloadHandlerTexture)request.downloadHandler).texture;
					_playerGames[i].Icon = Sprite.Create(tx, new Rect(0,0, tx.width, tx.height), Vector2.zero);
				}
			}
		}
		DrawUI ();	
	}
}