using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

/// <summary>
/// Helper script used for managing camera letterbox/pillarbox.
/// </summary>
[ExecuteInEditMode]
public class CameraHelper : MonoBehaviour
{
    /// <summary>
    /// Target camera size in units.
    /// </summary>
    public float2 targetSize = new float2(4.0f, 4.0f);
    
    /// <summary>
    /// The managed camera component.
    /// </summary>
    private Camera mCamera;

    /// <summary>
    /// Current resolution we are working with.
    /// </summary>
    private float2 mResolution;
    
    /// <summary>
    /// Called before the first frame update.
    /// </summary>
    void Start()
    { mCamera = GetComponent<Camera>(); }

    /// <summary>
    /// Update called once per frame.
    /// </summary>
    void Update()
    {
        // Update is only needed when the resolution is changed.
        float2 currentResolution = new float2((float)Screen.width, (float)Screen.height);
        if (mResolution.Equals(currentResolution))
        { return; }

        // Set the extent of size we want to use.
        var cameraSize = Math.Max(targetSize.x, targetSize.y);
        mCamera.orthographicSize = cameraSize;
        
        // Calculate the current aspect ratio of the screen and the requested target.
        var currentAspectRatio = (float)Screen.width / Screen.height;
        var targetAspectRatio = targetSize.x / targetSize.y;
        // How much of a letterbox do we need?
        var letterboxRatio = currentAspectRatio / targetAspectRatio;

        // Prepare letterbox-ed rectangle for the camera.
        var cameraRect = new Rect();
        if (letterboxRatio >= 1.0f)
        { // The screen is too wide -> Vertical letterbox.
            var letterboxWidth = 1.0f / letterboxRatio;
            cameraRect.x = (1.0f - letterboxWidth) / 2.0f;
            cameraRect.y = 0.0f;
            cameraRect.width = letterboxWidth;
            cameraRect.height = 1.0f;
        }
        else
        { // The screen is too high -> Horizontal letterbox.
            var letterboxHeight = letterboxRatio;
            cameraRect.x = 0.0f;
            cameraRect.y = (1.0f - letterboxHeight) / 2.0f;
            cameraRect.width = 1.0f;
            cameraRect.height = letterboxHeight;
        }

        // Update the camera to include our new letterbox.
        mCamera.rect = cameraRect;
        mResolution = currentResolution;
    }
}
