using UnityEngine;

[ExecuteInEditMode]
public class CameraPostProcessing : MonoBehaviour
{
    public Material postProcessingMaterial;

    public void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        Graphics.Blit(source, destination, postProcessingMaterial);
    }
}
