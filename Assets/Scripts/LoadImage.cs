using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadImage : MonoBehaviour
{
    // Use this for initialization
    public IEnumerator SetImage(string imageUrl)
    {
		Debug.Log(imageUrl);
        // Start a download of the given URL
        using (WWW www = new WWW(imageUrl))
        {
            // Wait for download to complete
            yield return www;
            Texture2D tex = www.texture;

            // assign texture
            Sprite newSprite = Sprite.Create(tex, new Rect(0.0f, 0.0f, tex.width, tex.height), new Vector2(0.5f, 0.5f), 100.0f);
            GetComponent<UnityEngine.UI.Image>().sprite = newSprite;
        }
    }
}