using System.Collections;
using UnityEngine;

public class Scroller : MonoBehaviour {

    public enum ScrollDirection {
        Horizontal,
        Vertical
    }

    public ScrollDirection scrollDirection = ScrollDirection.Horizontal;

    Vector3 startPosition;
    public float scrollSpeed;
    public float tileSizeZ = 1;

    void Start() {
        startPosition = transform.position;
    }

    void Update() {

        switch (scrollDirection) {

            case ScrollDirection.Horizontal:
                {
                    float newPosition = Mathf.Repeat(Time.time * scrollSpeed, tileSizeZ);
                    transform.position = startPosition + Vector3.right * newPosition;
                }
                break;
            case ScrollDirection.Vertical:
                {
                    float newPosition = Mathf.Repeat(Time.time * scrollSpeed, tileSizeZ);
                    transform.position = startPosition + Vector3.up * newPosition;
                }
                break;

        }

    }
}