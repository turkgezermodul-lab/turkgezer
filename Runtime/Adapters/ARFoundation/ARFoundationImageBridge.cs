using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

namespace TurkGezer.Platform
{
    /// <summary>One content root follows one configured reference image. Optional AR Foundation adapter.</summary>
    public sealed class ARFoundationImageBridge : MonoBehaviour
    {
        public ARTrackedImageManager imageManager;
        public ARTrackingBridge tracking;
        [Tooltip("Reference Image Library icindeki ad. Birden fazla hedef icin zorunludur.")]
        public string referenceImageName;
        [Tooltip("Takip pozuna tasinacak kok. Bridge bilesenini bunun disina koyun.")]
        public Transform poseRoot;
        private readonly Dictionary<TrackableId, ARTrackedImage> images = new Dictionary<TrackableId, ARTrackedImage>();
        private void OnEnable()
        {
            if (tracking != null) tracking.TrackingLost();
            if (imageManager == null) { Debug.LogWarning("ARFoundationImageBridge: Image Manager baglanmali.", this); return; }
            imageManager.trackedImagesChanged += Changed;
            foreach (var image in imageManager.trackables) images[image.trackableId] = image;
        }
        private void Changed(ARTrackedImagesChangedEventArgs args)
        {
            foreach (var image in args.added) images[image.trackableId] = image;
            foreach (var image in args.updated) images[image.trackableId] = image;
            foreach (var image in args.removed) images.Remove(image.trackableId);
        }
        private void LateUpdate()
        {
            if (tracking == null) return;
            ARTrackedImage selected = null;
            if (imageManager != null && imageManager.isActiveAndEnabled)
            foreach (var image in images.Values)
            {
                if (image == null || image.trackingState != TrackingState.Tracking) continue;
                if (!string.IsNullOrEmpty(referenceImageName) && image.referenceImage.name != referenceImageName) continue;
                selected = image;
                break;
            }
            if (selected != null && poseRoot != null && poseRoot != transform && !transform.IsChildOf(poseRoot))
                poseRoot.SetPositionAndRotation(selected.transform.position, selected.transform.rotation);
            bool found = selected != null;
            if (tracking.IsTracked != found) tracking.SetTracking(found);
        }
        private void OnDisable()
        {
            if (imageManager != null) imageManager.trackedImagesChanged -= Changed;
            images.Clear();
            if (tracking != null) tracking.TrackingLost();
        }
    }
}
