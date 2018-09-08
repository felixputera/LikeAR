using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using Vuforia;

public class LikeTrackableEventHandler : DefaultTrackableEventHandler
{
	public GameObject canvas;
	GameObject likeButton;
	GameObject notLikedButton;
	Coroutine httpCoroutine1, httpCoroutine2, httpCoroutine3;

	VuMarkManager m_VuMarkManager;
	VuMarkTarget m_ClosestVuMark;
	VuMarkTarget m_CurrentVuMark;
	VuMarkTarget currentTarget;

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
		public string name;
	}
	[System.Serializable]
	public class LikedBy
	{
		public Friend[] friends;
		public int rest_likes_count;
	}
	[System.Serializable]
	public class StickerSummary
	{
		public Target target;
		public bool is_liked;
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
		Debug.Log("New VuMark: " + target.InstanceId.NumericValue);
		currentTarget = target;
		if (httpCoroutine1 != null) StopCoroutine(httpCoroutine1);
		gameObject.SetActive(true);
		httpCoroutine1 = StartCoroutine(FetchFromBackend("GET", "http://localhost:8000/api/stickers/" + (int)target.InstanceId.NumericValue, rerender));
	}

	/// <summary>
	/// This method will be called whenever a tracked VuMark is lost
	/// </summary>
	public void OnVuMarkLost(VuMarkTarget target)
	{
		Debug.Log("Lost VuMark: " + GetVuMarkId(target));
	}

	public void OnLike()
	{
		if (httpCoroutine2 != null) StopCoroutine(httpCoroutine2);
		gameObject.SetActive(true);
		httpCoroutine2 = StartCoroutine(FetchFromBackend("POST", "http://localhost:8000/api/stickers/" + (int)currentTarget.InstanceId.NumericValue + "/like/", rerender));
	}

	public void OnUnlike()
	{
		if (httpCoroutine3 != null) StopCoroutine(httpCoroutine3);
		gameObject.SetActive(true);
		httpCoroutine3 = StartCoroutine(FetchFromBackend("POST", "http://localhost:8000/api/stickers/" + (int)currentTarget.InstanceId.NumericValue + "/dislike/", rerender));
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

		if (resp.data.is_liked == true)
		{
			likeButton.SetActive(false);
			notLikedButton.SetActive(true);
		}
		else
		{
			likeButton.SetActive(true);
			notLikedButton.SetActive(false);
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

		yield return www.SendWebRequest();

		if (www.isNetworkError || www.isHttpError)
		{
			Debug.Log(url);
			Debug.Log(www.downloadHandler.text);
			Debug.Log(www.error);
		}
		else
		{
			callback(www.downloadHandler.text);
		}
	}
}
