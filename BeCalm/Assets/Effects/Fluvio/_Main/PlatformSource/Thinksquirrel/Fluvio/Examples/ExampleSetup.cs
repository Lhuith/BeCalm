using System;
using Thinksquirrel.Fluvio.Internal;
using UnityEngine;
#pragma warning disable 0429

//! \cond PRIVATE
namespace Thinksquirrel.Fluvio.Examples
{
    #region Serializable classes
    [Serializable]
    public class TextureProperty
    {
        public string property;
        public Texture value;
        public Texture freeValue;
    }
    [Serializable]
    public class VectorProperty
    {
        public string property;
        public Vector4 value;
        public Vector4 freeValue;
    }
    [Serializable]
    public class ColorProperty
    {
        public string property;
        public Color value;
        public Color freeValue;
    }
    [Serializable]
    public class FloatProperty
    {
        public string property;
        public float value;
        public float freeValue;
    }

    [Serializable]
    public class FluidInitializationInfo
    {
        public FluidParticleSystem fluid;
        public int particleCount = 500;
        public int freeParticleCount = 500;
        public float emission = 100;
        public float freeEmission = 100;
        public bool fluidEffect;    
    }

    [Serializable]
    public class MaterialInitializationInfo
    {
        public Material material;
        public string shader;
        public string freeShader;
        public TextureProperty[] textures;
        public VectorProperty[] vectors;
        public ColorProperty[] colors;
        public FloatProperty[] floats;
    }
    #endregion

    /// <summary>
    ///     Sets up an example scene depending on the user's Fluvio license.
    /// </summary>
    #if !FLUVIO_DEVELOPMENT
    [ExecuteInEditMode]
    #endif
    [AddComponentMenu("")]
    public class ExampleSetup : MonoBehaviour
    {
        public FluidInitializationInfo[] fluids;
        public MaterialInitializationInfo[] materials;

        void Awake()
        {
            foreach (var info in fluids)
            {
                if (info == null) continue;

                var fluid = info.fluid;
                if (!fluid) continue;

                var pSystem = fluid.GetParticleSystem();
                if (!pSystem) continue;

                pSystem.maxParticles = VersionInfo.isFreeEdition ? info.freeParticleCount : info.particleCount;
                pSystem.emissionRate = VersionInfo.isFreeEdition ? info.freeEmission : info.emission;

                var fluidEffect = fluid.GetComponent<FluidEffect>();
                if (fluidEffect) fluidEffect.enabled = !VersionInfo.isFreeEdition && info.fluidEffect;
            }
            
            foreach(var info in materials)
            {
                if (info == null) continue;

                var mat = info.material;
                if (!mat) continue;

                mat.shader = VersionInfo.isFreeEdition ? Shader.Find(info.freeShader) : Shader.Find(info.shader);

                foreach (var tex in info.textures)
                {
                    if (tex != null && mat.HasProperty(tex.property))
                        mat.SetTexture(tex.property, VersionInfo.isFreeEdition ? tex.freeValue : tex.value);
                }

                foreach (var vec in info.vectors)
                {
                    if (vec != null && mat.HasProperty(vec.property))
                        mat.SetVector(vec.property, VersionInfo.isFreeEdition ? vec.freeValue : vec.value);
                }

                foreach (var col in info.colors)
                {
                    if (col != null && mat.HasProperty(col.property))
                        mat.SetColor(col.property, VersionInfo.isFreeEdition ? col.freeValue : col.value);
                }

                foreach (var flt in info.floats)
                {
                    if (flt != null && mat.HasProperty(flt.property))
                        mat.SetFloat(flt.property, VersionInfo.isFreeEdition ? flt.freeValue : flt.value);
                }
            }

            DestroyImmediate(this);
        }
    }
}
//! \endcond
