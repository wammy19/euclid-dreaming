using UnityEngine;
using System;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace OG {
    [Serializable, VolumeComponentMenu("Post-processing/SimpleAtmosphericFog")]
    public class SimpleAtmosphericFog : VolumeComponent, IPostProcessComponent {
        [Tooltip("Enable/Disable")]
        public BoolParameter on = new BoolParameter(false, false);

        [Header("General")]
        [Tooltip("Controls when start the fog")]
        public FloatParameter fogDensity = new FloatParameter(0.001f, false);
        [Tooltip("Exponent of the power calculation, Controls fog transition")]
        public MinFloatParameter fogPower = new MinFloatParameter(1, 0, false);
        [Tooltip("0 fog affect the sky, 1 the fog not affect sky")]
        public ClampedFloatParameter skyAlpha = new ClampedFloatParameter(0, 0, 1, false);
        [Tooltip("Color of fog")]
        public ColorParameter fogColor = new ColorParameter(new Color(0.7490196f, 0.8901961f, 1, 1), true, false, false);


        [Header("Height")]
        [Tooltip("Disable/Enable Height Fog")]
        public BoolParameter useHeightFog = new BoolParameter(false, false);
        [Tooltip("Y position when start height fog")]
        public FloatParameter baseHeight = new FloatParameter(0, false);
        [Tooltip("Maximum y height from the base height, if is 0 this layer is disable")]
        public MinFloatParameter maxHeight = new MinFloatParameter(500, 0, false);
        [Tooltip("Exponent of the power calculation, Controls height fog transition")]
        public MinFloatParameter heightPower = new MinFloatParameter(2, 0, false);

        [Header("Second Height Layer")]
        [Tooltip("Y position when start height fog")]
        public FloatParameter secondFogBaseHeight = new FloatParameter(0, false);
        [Tooltip("Maximum y height from the base height, if is 0 this layer is disable")]
        public MinFloatParameter secondMaxHeight = new MinFloatParameter(0, 0, false);
        [Tooltip("Exponent of the power calculation, Controls height fog transition")]
        public MinFloatParameter secondFogHeightPower = new MinFloatParameter(2, 0, false);

        [Header("Directional Inscattering")]
        [Tooltip("Directional Intesity, if is 0 the Directional Inscattering is disable")]
        public MinFloatParameter directionalIntesity = new MinFloatParameter(0, 0, false);
        [Tooltip("Exponent of the power calculation, Controls Directional transition")]
        public MinFloatParameter directionalPower = new MinFloatParameter(8, 0, false);
        [Tooltip("Use Main Light Color")]
        public BoolParameter useMainLightColor = new BoolParameter(true, false);
        [Tooltip("if useMainLightColor is disable use this color")]
        public ColorParameter directionalColor = new ColorParameter(new Color(0.8773585f, 0.8098478f, 0.6911268f, 1), true, false, false);
        
        public bool IsActive() => (bool)on;
        public bool IsTileCompatible() => false;
    }
}
