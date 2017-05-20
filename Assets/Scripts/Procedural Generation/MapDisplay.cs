using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapDisplay : MonoBehaviour {

    public Renderer textureRenderer;

    public bool DrawOnPlane;
    public void DrawTexture(Texture2D texture) {
		
		Debug.Log(textureRenderer.GetComponent<Renderer>().GetType());
		
        textureRenderer.sharedMaterial.mainTexture = texture;

        if (DrawOnPlane) {
            textureRenderer.transform.localScale = new Vector3(texture.width, 1, texture.height);
        } else {
            textureRenderer.transform.localScale = new Vector3(texture.width, texture.height, 1);
        }

    }

}