using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Vuforia;

public class LikeTrackableEventHandler : DefaultTrackableEventHandler {

    public GameObject canvas;
    public GameObject loader;

    VuMarkManager m_VuMarkManager;
    VuMarkTarget m_ClosestVuMark;
    VuMarkTarget m_CurrentVuMark;


    // Use this for initialization
    protected override void Start () {
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
        Debug.Log("New VuMark: " + GetVuMarkId(target));
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

    //protected new void OnTrackingFound() {
    //    Debug.Log("FOUND XD");

    //}

    //protected new void OnTrackingLost() {

    //}

    //private IEnumerator FetchFromBackend(string id) {
    //    UnityWebRequest www = UnityWebRequest.Get("http://www.example.com/" + id);
    //    yield return www.SendWebRequest();

    //    if (www.isNetworkError || www.isHttpError)
    //    {
    //        Debug.Log(www.error);
    //    }
    //    else
    //    {
    //        // Hide loader
    //        loader.SetActive(false);

    //        // Show results as text
    //        Debug.Log(www.downloadHandler.text);

    //        // Or retrieve results as binary data
    //        byte[] results = www.downloadHandler.data;
    //    }
    //}
}
