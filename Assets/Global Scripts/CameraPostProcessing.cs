using UnityEngine;

[ExecuteInEditMode]
public class CameraPostProcessing : MonoBehaviour
{
    public Material postProcessingMaterial;

    public void OnRenderImage(RenderTexture source, RenderTexture destination)
    {   
        Debug.Log("hey");
        Graphics.Blit(source, destination, postProcessingMaterial);
    }
}
