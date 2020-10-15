using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightTexture {
    public RenderTexture renderTexture;

    public int width;
    public int height;

    public int setWidth;
    public int setHeight;

    public LightTexture(int width, int height, int depth, RenderTextureFormat format) {
        renderTexture = new RenderTexture (width, height, depth, format);

        this.width = width;
        this.height = height;
    }

    public LightTexture(int width, int height, int depth) {
        renderTexture = new RenderTexture (width, height, depth);

        this.width = width;
        this.height = height;
    }

    public void Create() {
        renderTexture.Create();
    }
}
