﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Seed : MonoBehaviour {

    public int seed = 9999;

    public float scale;

    public float xOffset;
    public float yOffset;


    void Start() {
        scale = seed % 123456;
        xOffset = seed % 3214;
        yOffset = seed % 56789;
    }

    public bool ShouldSpawn(int x, int y, int width, int height, float threshold) {
        float xCoord = (((float)x / (float)width) * scale) + xOffset;
        float yCoord = (((float)y / (float)width) * scale) + yOffset;

        float z = Mathf.PerlinNoise(xCoord, yCoord);
        if(z >= threshold) {
            return true;
        }
        return false;
    }

}
