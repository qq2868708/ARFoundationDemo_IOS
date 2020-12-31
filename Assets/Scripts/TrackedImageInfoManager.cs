using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.XR.ARFoundation;
using static System.Math;

[RequireComponent(typeof(ARTrackedImageManager))]
public class TrackedImageInfoManager : MonoBehaviour
{
    [SerializeField]
    [Tooltip("The camera to set on the world space UI canvas for each instantiated image info.")]
    Camera m_WorldSpaceCanvasCamera;

    public Camera worldSpaceCanvasCamera
    {
        get { return m_WorldSpaceCanvasCamera; }
        set { m_WorldSpaceCanvasCamera = value; }
    }

    [SerializeField]
    [Tooltip("If an image is detected but no source texture can be found, this texture is used instead.")]
    Texture2D m_DefaultTexture;

    public Texture2D defaultTexture
    {
        get { return m_DefaultTexture; }
        set { m_DefaultTexture = value; }
    }

    ARTrackedImageManager m_TrackedImageManager;

    public ARSessionOrigin aRSessionOrigin;
    public Camera arCamera;

    public Text debugText;

    public Transform quad;
    public Transform root;

    public Transform temp;

    void Awake()
    {
        m_TrackedImageManager = GetComponent<ARTrackedImageManager>();
    }

    void OnEnable()
    {
        m_TrackedImageManager.trackedImagesChanged += OnTrackedImagesChanged;
    }

    void OnDisable()
    {
        m_TrackedImageManager.trackedImagesChanged -= OnTrackedImagesChanged;
    }

    void UpdateInfo(ARTrackedImage trackedImage)
    {
        // Set canvas camera
        var canvas = trackedImage.GetComponentInChildren<Canvas>();
        canvas.worldCamera = worldSpaceCanvasCamera;

        // Update information about the tracked image
        var text = canvas.GetComponentInChildren<Text>();
        text.text = string.Format(
            "{0}\ntrackingState: {1}\nGUID: {2}\nReference size: {3} cm\nDetected size: {4} cm\nTransfrom:{5}\nRotation:{6}   {7}",
            trackedImage.referenceImage.name,
            trackedImage.trackingState,
            trackedImage.referenceImage.guid,
            trackedImage.referenceImage.size * 100f,
            trackedImage.size * 100f,
            trackedImage.transform.position,
            trackedImage.transform.rotation,
            trackedImage.transform.rotation.eulerAngles);

        var planeParentGo = trackedImage.transform.GetChild(0).gameObject;
        var planeGo = planeParentGo.transform.GetChild(0).gameObject;

        //quad.transform.position = trackedImage.transform.position;
        //quad.transform.rotation = trackedImage.transform.rotation;
        //quad.transform.rotation *= Quaternion.Euler(90, 0, 0);

        //aRSessionOrigin.MakeContentAppearAt(quad, trackedImage.transform.position);

        //aRSessionOrigin.MakeContentAppearAt(trackedImage.transform, trackedImage.transform.position);
        //var angle = trackedImage.transform.rotation;
        //aRSessionOrigin.MakeContentAppearAt(quad, angle);
        //aRSessionOrigin.MakeContentAppearAt(quad, angle * Quaternion.Euler(90, 0, 0));

        //aRSessionOrigin.MakeContentAppearAt(trackedImage.transform, trackedImage.transform.position);
        //var angle = trackedImage.transform.rotation;
        //aRSessionOrigin.MakeContentAppearAt(quad, angle);
        //Quaternion q=new Quaternion();
        //Vector3 a = Vector3.Cross(quad.forward, trackedImage.transform.position);
        //q.x=a.x;
        //q.y = a.y;
        //q.z = a.z;
        //q.w = (float)Sqrt(Pow(quad.forward.magnitude,2) * Pow(trackedImage.transform.up.magnitude , 2)) + Vector3.Dot(quad.forward, trackedImage.transform.up);
        //aRSessionOrigin.MakeContentAppearAt(quad, quad.transform.rotation * q);

        //aRSessionOrigin.transform.position = Vector3.zero;
        //aRSessionOrigin.transform.rotation = Quaternion.identity;
        //temp.position = quad.position;
        //temp.rotation = Quaternion.identity;
        ////aRSessionOrigin.MakeContentAppearAt(temp, trackedImage.transform.rotation*Quaternion.Inverse(quad.transform.rotation));
        //aRSessionOrigin.MakeContentAppearAt(temp, trackedImage.transform.position, trackedImage.transform.localRotation * Quaternion.Inverse(quad.rotation));

        //var re = Quaternion.FromToRotation(quad.forward, trackedImage.transform.up);
        //aRSessionOrigin.MakeContentAppearAt(trackedImage.transform, trackedImage.transform.position);
        //aRSessionOrigin.MakeContentAppearAt(quad, quad.transform.rotation*Quaternion.Inverse(re));

        debugText.text = string.Format("angle:{0}\nImageUp:{1}\nQuadForward:{2}\nQuadPosition:{3}\nQuadRotation:{4}",
            "123",
            trackedImage.transform.up,
            quad.forward,
            quad.position,
            quad.rotation.eulerAngles);

        if (trackedImage.trackingState != TrackingState.None)
        {
            planeGo.SetActive(true);

            // The image extents is only valid when the image is being tracked
            trackedImage.transform.localScale = new Vector3(trackedImage.size.x, 1f, trackedImage.size.y);

            // Set the texture
            var material = planeGo.GetComponentInChildren<MeshRenderer>().material;
            material.mainTexture = (trackedImage.referenceImage.texture == null) ? defaultTexture : trackedImage.referenceImage.texture;

            aRSessionOrigin.transform.position = Vector3.zero;
            aRSessionOrigin.transform.rotation = Quaternion.identity;
            temp.position = quad.position;
            temp.rotation = Quaternion.identity;
            //aRSessionOrigin.MakeContentAppearAt(quad, Quaternion.Inverse(quad.transform.rotation));
            aRSessionOrigin.MakeContentAppearAt(temp, trackedImage.transform.position, trackedImage.transform.localRotation * Quaternion.Inverse(quad.rotation));

        }
        else
        {
            planeGo.SetActive(false);
        }
    }

    void OnTrackedImagesChanged(ARTrackedImagesChangedEventArgs eventArgs)
    {
        foreach (var trackedImage in eventArgs.added)
        {
            // Give the initial image a reasonable default scale
            trackedImage.transform.localScale = new Vector3(0.01f, 1f, 0.01f);

            UpdateInfo(trackedImage);
        }

        foreach (var trackedImage in eventArgs.updated)
            UpdateInfo(trackedImage);
    }
}
