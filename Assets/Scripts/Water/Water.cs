using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Water : MonoBehaviour
{
    public new GameObject camera;

    [Header("Waves")]
    public float amplitude;
    public float wavelength;
    public float speed;
    [Range(1, 50)]
    public int waveCount;
    [Range(0f, 1f)]
    public float hurstExponent;
    [Range(1f, 2f)]
    public float frequencyMultiplier;
    [Range(1f, 2f)]
    public float phaseMultiplier;

    [Header("Lighting")]
    public Vector4 specularColor = new Vector4(0.3f, 0.3f, 0.3f, 0f);
    public float shininess = 10f;

    private new Renderer renderer;

    private void Start()
    {
        renderer = GetComponent<MeshRenderer>();
    }

    private void Update()
    {
        SetupShader();
    }

    private void SetupShader()
    {
        MaterialPropertyBlock props = new MaterialPropertyBlock();
        Light light = RenderSettings.sun;
        props.SetFloat("_PositionY", transform.position.y);
        props.SetFloat("_ScaleY", transform.localScale.y);
        props.SetFloat("_Amplitude", amplitude);
        props.SetFloat("_Wavelength", wavelength);
        props.SetFloat("_Speed", speed);
        props.SetInt("_WaveCount", waveCount);
        props.SetFloat("_HurstExponent", hurstExponent);
        props.SetFloat("_FrequencyMultiplier", frequencyMultiplier);
        props.SetFloat("_PhaseMultiplier", phaseMultiplier);
        renderer.SetPropertyBlock(props);
    }
}
