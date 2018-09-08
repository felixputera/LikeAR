using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using Vuforia;


public class LikeTrackableEventHandler : DefaultTrackableEventHandler
{
	public GameObject canvas;
	GameObject likeButton, likeButtonGolden, friend1, friend2, rest;
	GameObject notLikedButton, notLikedButtonGolden;
	Coroutine httpCoroutine1, httpCoroutine2, httpCoroutine3;
	GameObject eventSystem;
	string HOST = "https://likeplus.ntusu.org";

	VuMarkManager m_VuMarkManager;
	VuMarkTarget m_ClosestVuMark;
	VuMarkTarget m_CurrentVuMark;
	static int currentTargetId;

	[System.Serializable]
	public class Target
	{
		public string title;
		public string description;
		public int like_count;
		public string like_count_humanize;
	}
	[System.Serializable]
	public class Friend
	{
		public int id;
		public string profile_picture;
		public string name;
	}
	[System.Serializable]
	public class LikedBy
	{
		public Friend[] sample_friends;
		public int rest_likes_count;
	}
	[System.Serializable]
	public class StickerSummary
	{
		public Target target;
		public bool is_liked;
		public int type;
		public LikedBy liked_by;
	}
	[System.Serializable]
	public class StickerSummaryResponse
	{
		public bool is_success;
		public StickerSummary data;
	}

	// Use this for initialization
	protected override void Start()
	{
		likeButton = GameObject.FindGameObjectWithTag("LikeButton");
		notLikedButton = GameObject.FindGameObjectWithTag("NotLikedButton");
		likeButtonGolden = GameObject.FindGameObjectWithTag("LikeButtonGolden");
		notLikedButtonGolden = GameObject.FindGameObjectWithTag("NotLikedButtonGolden");
		eventSystem = GameObject.FindGameObjectWithTag("EventSystem");
		friend1 = GameObject.FindGameObjectWithTag("Friend1");
		friend2 = GameObject.FindGameObjectWithTag("Friend2");
		rest = GameObject.FindGameObjectWithTag("Rest");

		base.Start();
		m_VuMarkManager = TrackerManager.Instance.GetStateManager().GetVuMarkManager();
		m_VuMarkManager.RegisterVuMarkDetectedCallback(OnVuMarkDetected);
		m_VuMarkManager.RegisterVuMarkLostCallback(OnVuMarkLost);

	}

	#region PUBLIC_METHODS

	/// <summary>
	/// This method will be called whenever a new VuMark is detected
	/// </summary>
	public void OnVuMarkDetected(VuMarkTarget target)
	{
		likeButton.SetActive(false);
		notLikedButton.SetActive(false);
		likeButtonGolden.SetActive(false);
		notLikedButtonGolden.SetActive(false);

		Debug.Log("New VuMark: " + target.InstanceId.NumericValue);
		currentTargetId = (int)target.InstanceId.NumericValue;
		Debug.Log("new current targetid: " + currentTargetId);
		if (httpCoroutine1 != null) StopCoroutine(httpCoroutine1);
		gameObject.SetActive(true);
		eventSystem.SetActive(true);
		httpCoroutine1 = StartCoroutine(FetchFromBackend("GET", HOST + "/api/stickers/" + currentTargetId + "/", rerender));
	}

	/// <summary>
	/// This method will be called whenever a tracked VuMark is lost
	/// </summary>
	public void OnVuMarkLost(VuMarkTarget target)
	{
		Debug.Log("Lost VuMark: " + GetVuMarkId(target));
		likeButton.SetActive(false);
		notLikedButton.SetActive(false);
		likeButtonGolden.SetActive(false);
		notLikedButtonGolden.SetActive(false);
	}

	public void OnLike()
	{
		if (httpCoroutine2 != null) StopCoroutine(httpCoroutine2);
		gameObject.SetActive(true);
		eventSystem.SetActive(true);
		Debug.Log("debug");
		Debug.Log(gameObject);
		Debug.Log(eventSystem);
		Debug.Log(httpCoroutine2);
		Debug.Log(currentTargetId);
		Debug.Log("enddebug");
		httpCoroutine2 = StartCoroutine(FetchFromBackend("POST", HOST + "/api/stickers/" + currentTargetId + "/like/", rerender));
	}

	public void OnUnlike()
	{
		if (httpCoroutine3 != null) StopCoroutine(httpCoroutine3);
		gameObject.SetActive(true);
		eventSystem.SetActive(true);
		httpCoroutine3 = StartCoroutine(FetchFromBackend("POST", HOST + "/api/stickers/" + currentTargetId + "/dislike/", rerender));
	}

	#endregion // PUBLIC_METHODS

	string GetVuMarkId(VuMarkTarget vumark)
	{
		switch (vumark.InstanceId.DataType)
		{
			case InstanceIdType.BYTES:
				return vumark.InstanceId.HexStringValue;
			case InstanceIdType.STRING:
				return vumark.InstanceId.StringValue;
			case InstanceIdType.NUMERIC:
				return vumark.InstanceId.NumericValue.ToString();
		}
		return string.Empty;
	}

	private void rerender(string data) {
		StickerSummaryResponse resp = JsonUtility.FromJson<StickerSummaryResponse>(data);

		GameObject titleObject = GameObject.FindGameObjectWithTag("Title");
		titleObject.GetComponent<UnityEngine.UI.Text>().text = resp.data.target.title;

		GameObject descriptionObject = GameObject.FindGameObjectWithTag("Description");
		descriptionObject.GetComponent<UnityEngine.UI.Text>().text = resp.data.target.description;

		Debug.Log(friend1);
		Debug.Log(friend2);
		Debug.Log(resp.data.liked_by.sample_friends);
		Debug.Log(resp.data.liked_by.sample_friends[0].profile_picture);
		Debug.Log(resp.data.liked_by.sample_friends[1].profile_picture);
		friend1.GetComponent<LoadImage>().SetImage(resp.data.liked_by.sample_friends[0].profile_picture);
		friend2.GetComponent<LoadImage>().SetImage(resp.data.liked_by.sample_friends[1].profile_picture);
		rest.GetComponent<Text>().text = resp.data.liked_by.rest_likes_count.ToString();

		Debug.Log(resp.data.is_liked);
		Debug.Log(resp.data.type);
		if (resp.data.is_liked == true)
		{
			if (resp.data.type == 1) { // meaning type is golden
				likeButton.SetActive(false);
				notLikedButton.SetActive(false);
				likeButtonGolden.SetActive(true);
				notLikedButtonGolden.SetActive(false);
			} else {
				likeButton.SetActive(true);
				notLikedButton.SetActive(false);
				likeButtonGolden.SetActive(false);
				notLikedButtonGolden.SetActive(false);
			}
		}
		else
		{
			if (resp.data.type == 1) {
				likeButton.SetActive(false);
				notLikedButton.SetActive(false);
				likeButtonGolden.SetActive(false);
				notLikedButtonGolden.SetActive(true);
			} else {
				likeButton.SetActive(false);
				notLikedButton.SetActive(true);
				likeButtonGolden.SetActive(false);
				notLikedButtonGolden.SetActive(false);
			}
		}
		Canvas.ForceUpdateCanvases();
	}

	private IEnumerator FetchFromBackend(string method, string url, System.Action<string> callback)
	{
		UnityWebRequest www;
		if (method == "GET") {
			www = UnityWebRequest.Get(url);
		} else {
			www = UnityWebRequest.Post(url, "{}");
		}
		www.SetRequestHeader("AUTHORIZATION", "JWT " + FacebookInit.GetToken());

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
}
