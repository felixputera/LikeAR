using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using Vuforia;

public class LikeTrackableEventHandler : DefaultTrackableEventHandler
{
	public GameObject canvas;
	Coroutine httpCoroutine;

	VuMarkManager m_VuMarkManager;
	VuMarkTarget m_ClosestVuMark;
	VuMarkTarget m_CurrentVuMark;

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
		base.Start();
		m_VuMarkManager = TrackerManager.Instance.GetStateManager().GetVuMarkManager();
		m_VuMarkManager.RegisterVuMarkDetectedCallback(OnVuMarkDetected);
		m_VuMarkManager.RegisterVuMarkLostCallback(OnVuMarkLost);
		Debug.Log("Start OMEGALUL");

	}

	#region PUBLIC_METHODS

	/// <summary>
	/// This method will be called whenever a new VuMark is detected
	/// </summary>
	public void OnVuMarkDetected(VuMarkTarget target)
	{
		Debug.Log("New VuMark: " + target.InstanceId.NumericValue);
		if (httpCoroutine != null) StopCoroutine(httpCoroutine);
		gameObject.SetActive(true);
		httpCoroutine = StartCoroutine("FetchFromBackend", (int)target.InstanceId.NumericValue);
	}

	/// <summary>
	/// This method will be called whenever a tracked VuMark is lost
	/// </summary>
	public void OnVuMarkLost(VuMarkTarget target)
	{
		Debug.Log("Lost VuMark: " + GetVuMarkId(target));
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

	private IEnumerator FetchFromBackend(int id)
	{
		UnityWebRequest www = UnityWebRequest.Get("https://likeplus.ntusu.org/api/stickers/" + id);
		yield return www.SendWebRequest();

		if (www.isNetworkError || www.isHttpError)
		{
			Debug.Log("https://likeplus.ntusu.org/api/stickers/" + id);
			Debug.Log(www.error);
		}
		else
		{
			// Hide loader
			//loader.SetActive(false);

			// Show results as text
			StickerSummaryResponse resp = JsonUtility.FromJson<StickerSummaryResponse>(www.downloadHandler.text);
			Debug.Log(resp.data);
			Debug.Log(www.downloadHandler.text);
			Debug.Log(resp.data.target.title);

			GameObject titleObject = GameObject.FindGameObjectWithTag("Title");
			titleObject.GetComponent<UnityEngine.UI.Text>().text = resp.data.target.title;

			GameObject descriptionObject = GameObject.FindGameObjectWithTag("Description");
			descriptionObject.GetComponent<UnityEngine.UI.Text>().text = resp.data.target.description;

			if (resp.data.is_liked) {
				GameObject likeButton = GameObject.FindGameObjectWithTag("LikeButton");
				GameObject notLikedButton = GameObject.FindGameObjectWithTag("NotLikedButton");
				likeButton.SetActive(true);
				notLikedButton.SetActive(false);
			} else {
				GameObject likeButton = GameObject.FindGameObjectWithTag("LikeButton");
				GameObject notLikedButton = GameObject.FindGameObjectWithTag("NotLikedButton");
				likeButton.SetActive(false);
				notLikedButton.SetActive(true);
			}

			// Or retrieve results as binary data
			byte[] results = www.downloadHandler.data;
		}
	}
}
