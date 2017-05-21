using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapDisplay : MonoBehaviour {

    public bool drawOnPixelPerfectGrid;
    public Renderer textureRenderer;
    private MeshFilter meshType;

    private bool DrawOnPlane = false;

    void Start () {

        meshType = textureRenderer.GetComponent<MeshFilter> ();

        if (meshType.mesh.name == "Quad Instance") {
            Debug.Log ("Drawing on a quad");
            DrawOnPlane = false;
        } else {
            Debug.Log ("Drawing on a plane");
            DrawOnPlane = true;
        }

    }

    public void DrawTexture (Texture2D texture) {

        textureRenderer.sharedMaterial.mainTexture = texture;

        if (DrawOnPlane) {

            if (drawOnPixelPerfectGrid) {
                textureRenderer.transform.localScale = new Vector3 (texture.width * 0.01f, 1, texture.height * 0.01f);
            } else {
                textureRenderer.transform.localScale = new Vector3 (texture.width, 1, texture.height);
            }

        } else {

            if (drawOnPixelPerfectGrid) {
                textureRenderer.transform.localScale = new Vector3 (texture.width * 0.01f, texture.height * 0.01f, 1);
            } else {
                textureRenderer.transform.localScale = new Vector3 (texture.width, texture.height, 1);
            }

        }

    }

}