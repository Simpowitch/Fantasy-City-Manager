using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Rendering.Light {

    public static class ShadowSetup {
        public static bool drawAbove = false;

        public static void Calculate(LightingBuffer2D buffer) {	
            if (buffer == null) {
                return;
            }
            
            drawAbove = buffer.lightSource.whenInsideCollider == LightingSource2D.WhenInsideCollider.DrawAbove;
        }
    }
    
}