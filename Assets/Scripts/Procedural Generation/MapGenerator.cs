using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour {

    public enum DrawMode {
        NoiseMap,
        ColorMap
    }

    public DrawMode drawMode;

    public int mapWidth;
    public int mapHeight;
    public int seed;
    public float noiseScale;
    [RangeAttribute(1, 16)]
    public int octaves;
    [RangeAttribute(0, 1)]
    public float persistance = 0.5f;
    public float lacunarity;
    private Vector2 offset;
    public Vector2 offsetScrollSpeed;

    public bool autoUpdate;

    public TerrainType[] regions;

    /// <summary>
    /// Update is called every frame, if the MonoBehaviour is enabled.
    /// </summary>
    void Update() {

        if (drawMode == DrawMode.ColorMap) {
            
            offset = new Vector2(offset.x + Time.deltaTime * offsetScrollSpeed.x, offset.y + Time.deltaTime * offsetScrollSpeed.y);
            GenerateMap();
            
        }

    }

    public void GenerateMap() {

        float[, ] noiseMap = Noise.GenerateNoiseMap(mapWidth, mapHeight, seed, noiseScale, octaves, persistance, lacunarity, offset);

        Color[] colorMap = new Color[mapWidth * mapHeight];

        for (int y = 0; y < mapHeight; y++) {

            for (int x = 0; x < mapWidth; x++) {

                float currentHeight = noiseMap[x, y];

                for (int i = 0; i < regions.Length; i++) {

                    if (currentHeight <= regions[i].height) {

                        colorMap[y * mapWidth + x] = regions[i].color;
                        break;
                    }

                }

            }

        }

        MapDisplay display = FindObjectOfType<MapDisplay>();

        if (drawMode == DrawMode.NoiseMap) {
            display.DrawTexture(TextureGenerator.TextureFromHeightMap(noiseMap));
        } else if (drawMode == DrawMode.ColorMap) {
            display.DrawTexture(TextureGenerator.TextureFromColorMap(colorMap, mapWidth, mapHeight));
        }

    }

    /// <summary>
    /// Called when the script is loaded or a value is changed in the
    /// inspector (Called in the editor only).
    /// </summary>
    void OnValidate() {

        if (mapWidth < 1) {
            mapWidth = 1;
        }

        if (mapHeight < 1) {
            mapHeight = 1;
        }

        if (lacunarity < 1) {
            lacunarity = 1;
        }

        if (octaves < 0) {
            octaves = 0;
        }

    }

}

[System.SerializableAttribute]
public struct TerrainType {
    public string name;
    public float height;
    public Color color;
}