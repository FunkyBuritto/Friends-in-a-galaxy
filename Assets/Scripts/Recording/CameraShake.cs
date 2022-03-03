using Cinemachine;
using UnityEngine;

[SaveDuringPlay]
[AddComponentMenu("")] // Hide in menu
public class CameraShake : CinemachineExtension
{
    [Tooltip("Magnitude of the shake")]
    [SerializeField] private float magnitude = 0.5f;
    private static float amplitude = 0.0f;
    private static float falloff = 0.01f;

    public static void Shake(float amplitude = 1.0f, float duration = 0.5f)
    {
        CameraShake.amplitude = amplitude;
        falloff = amplitude * (1.0f / duration);
    }

    protected override void Awake()
    {
        amplitude = 0.0f;

        base.Awake();
    }

    protected override void PostPipelineStageCallback(
        CinemachineVirtualCameraBase vcam,
        CinemachineCore.Stage stage, ref CameraState state, float deltaTime)
    {
        if (stage == CinemachineCore.Stage.Body && amplitude > 0.0f)
        {
            Vector3 shakeAmount = GetOffset();
            amplitude -= falloff * deltaTime;
            state.PositionCorrection += shakeAmount;
        }
    }

    Vector3 GetOffset()
    {
        float min = -magnitude * amplitude;
        float max = magnitude * amplitude;
        return new Vector3(
            Random.Range(min, max),
            Random.Range(min, max),
            Random.Range(min, max));
    }
}
