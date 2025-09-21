using UnityEngine;
using UnityEngine.EventSystems;

public class LensDistortionInputModule : StandaloneInputModule
{
    protected override MouseState GetMousePointerEventData(int id)
    {
        MouseState mouseState = base.GetMousePointerEventData(id);
        
        // Apply lens distortion compensation to mouse position
        CompensatePointerData(mouseState.GetButtonState(PointerEventData.InputButton.Left).eventData.buttonData);
        CompensatePointerData(mouseState.GetButtonState(PointerEventData.InputButton.Right).eventData.buttonData);
        CompensatePointerData(mouseState.GetButtonState(PointerEventData.InputButton.Middle).eventData.buttonData);

        return mouseState;
    }

    private void CompensatePointerData(PointerEventData pointerData)
    {
        if (pointerData == null) return;

        // Get lens distortion settings (same as your GunShoot script)
        var volumeStack = UnityEngine.Rendering.VolumeManager.instance.stack;
        var lensDistortion = volumeStack.GetComponent<UnityEngine.Rendering.Universal.LensDistortion>();

        if (lensDistortion != null && lensDistortion.active)
        {
            Vector2 mousePos = pointerData.position;
            
            Vector2 normalizedPos = new Vector2(
                mousePos.x / Screen.width,
                mousePos.y / Screen.height
            );

            Vector2 correctedPos = UndistortPosition(normalizedPos, lensDistortion.intensity.value);

            pointerData.position = new Vector2(
                correctedPos.x * Screen.width,
                correctedPos.y * Screen.height
            );
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