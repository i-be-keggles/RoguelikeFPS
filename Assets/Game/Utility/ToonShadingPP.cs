using System;
using UnityEngine.Rendering.HighDefinition;
using UnityEngine.Rendering;
using UnityEngine;

[Serializable, VolumeComponentMenu("Post-processing/Custom/ToonShadingPP")]
public sealed class ToonShadingPP : CustomPostProcessVolumeComponent, IPostProcessComponent
{
    [Tooltip("Controls the intensity of the effect.")]
    [SerializeField] private FloatParameter _posterizeAmount = new FloatParameter(1f);
    [SerializeField] private ClampedFloatParameter _testOverride = new ClampedFloatParameter(0f,0f,1f);

    Material m_Material;

    public bool IsActive() => m_Material != null && _posterizeAmount.value > 0f;

    public override CustomPostProcessInjectionPoint injectionPoint => CustomPostProcessInjectionPoint.BeforePostProcess;

    const string kShaderName = "Hidden/Shader/ToonShadingPP";

    public override void Setup()
    {
        if (Shader.Find(kShaderName) != null)
            m_Material = new Material(Shader.Find(kShaderName));
        else
            Debug.LogError(
                $"Unable to find shader '{kShaderName}'. Post Process Volume ToonShadingPP is unable to load.");
    }

    public override void Render(CommandBuffer cmd, HDCamera camera, RTHandle source, RTHandle destination)
    {
        if (m_Material == null)
            return;

        m_Material.SetFloat("_Intensity", _posterizeAmount.value);
        m_Material.SetFloat("_Override", _testOverride.value);
        m_Material.SetTexture("_InputTexture", source);
        HDUtils.DrawFullScreen(cmd, m_Material, destination);
    }

    public override void Cleanup()
    {
        CoreUtils.Destroy(m_Material);
    }
}