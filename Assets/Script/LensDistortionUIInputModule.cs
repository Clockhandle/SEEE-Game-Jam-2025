using UnityEngine;
using UnityEngine.EventSystems;

public class LensDistortionUIInputModule : StandaloneInputModule
{
    protected override MouseState GetMousePointerEventData(int id)
    {
        MouseState mouseState = base.GetMousePointerEventData(id);
        
        // Compensate for lens distortion in all button states
        CompensateMouseData(mouseState.GetButtonState(PointerEventData.InputButton.Left).eventData.buttonData);
        CompensateMouseData(mouseState.GetButtonState(PointerEventData.InputButton.Right).eventData.buttonData);
        CompensateMouseData(mouseState.GetButtonState(PointerEventData.InputButton.Middle).eventData.buttonData);

        return mouseState;
    }

    private void CompensateMouseData(PointerEventData pointerData)
    {
        if (pointerData == null) return;

        Vector2 mousePos = pointerData.position;

        // Get lens distortion from volume stack (same as your GunShoot script)
        var volumeStack = UnityEngine.Rendering.VolumeManager.instance.stack;
        UnityEngine.Rendering.Universal.LensDistortion lensDistortion = volumeStack.GetComponent<UnityEngine.Rendering.Universal.LensDistortion>();

        if (lensDistortion != null && lensDistortion.active)
        {
            Vector2 normalizedPos = new Vector2(
                mousePos.x / Screen.width,
                mousePos.y / Screen.height
            );

            Vector2 correctedPos = UndistortPosition(normalizedPos, lensDistortion.intensity.value);

            mousePos.x = correctedPos.x * Screen.width;
            mousePos.y = correctedPos.y * Screen.height;

            pointerData.position = mousePos;
        }
    }

    private Vector2 UndistortPosition(Vector2 normalizedPos, float distortionIntensity)
    {
        Vector2 centered = normalizedPos - Vector2.one * 0.5f;
        float distance = centered.magnitude;
        
        float distortedDistance = distance;
        if (distance > 0.001f)
        {
            distortedDistance = distance / (1.0f + distortionIntensity * distance * distance);
        }
        
        Vector2 corrected = centered.normalized * distortedDistance;
        return corrected + Vector2.one * 0.5f;
    }
}