using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Facebook.Unity;
using Facebook.MiniJSON;
using UnityEngine.Networking;

public class FacebookInit : MonoBehaviour
{
	string HOST = "https://likeplus.ntusu.org";
	static string token;

	public static string GetToken()
	{
		return token;
	}

	void Awake()
	{
		if (!FB.IsInitialized)
		{
			// Initialize the Facebook SDK
			FB.Init(InitCallback, OnHideUnity);
		}
		else
		{
			// Already initialized, signal an app activation App Event
			FB.ActivateApp();
		}
	}

	private void InitCallback()
	{
		if (FB.IsInitialized)
		{
			// Signal an app activation App Event
			FB.ActivateApp();
			// Continue with Facebook SDK
			// ...
			var perms = new List<string>() { "public_profile", "email" };
			FB.LogInWithReadPermissions(perms, AuthCallback);

		}
		else
		{
			Debug.Log("Failed to Initialize the Facebook SDK");
		}
	}

	private void OnHideUnity(bool isGameShown)
	{
		if (!isGameShown)
		{
			// Pause the game - we will need to hide
			Time.timeScale = 0;
		}
		else
		{
			// Resume the game - we're getting focus again
			Time.timeScale = 1;
		}
	}

	private void AuthCallback(ILoginResult result)
	{
		if (FB.IsLoggedIn)
		{
			// AccessToken class will have session details
			var aToken = Facebook.Unity.AccessToken.CurrentAccessToken;
			// Print current access token's User ID
			Debug.Log(aToken.TokenString);
			Debug.Log(aToken.UserId);
			// Print current access token's granted permissions
			foreach (string perm in aToken.Permissions)
			{
				Debug.Log(perm);
			}

			StartCoroutine(FetchFromBackend("POST", HOST + "/api/login/", "{\"access_token\":\"" + aToken.TokenString + "\"}", loginCallback));
		}
		else
		{
			Debug.Log("User cancelled login");
		}
	}

	private void loginCallback(string data) {
		token = data;
		Debug.Log(token);
	}

	private IEnumerator FetchFromBackend(string method, string url, string data, System.Action<string> callback)
	{
		UnityWebRequest www;
		if (method == "GET")
		{
			www = UnityWebRequest.Get(url);
		}
		else
		{
			www = UnityWebRequest.Post(url, data);
		}

		yield return www.SendWebRequest();

		if (www.isNetworkError || www.isHttpError)
		{
			Debug.Log(url);
			Debug.Log(www.downloadHandler.text);
			Debug.Log(www.error);
		}
		else
		{
			Debug.Log(url);
			callback(www.downloadHandler.text);
		}
	}

	//private void LoginCallback(IGraphResult result) {
	//	string fb_id = (string)result.ResultDictionary["id"];
	//	string fb_name = (string)result.ResultDictionary["name"];
	//	Debug.Log(fb_id);
	//	Debug.Log(fb_name);
	//}

	// Use this for initialization
	void Start()
	{

	}

	// Update is called once per frame
	void Update()
	{

	}
}
