using UnityEngine;

namespace RPG.Character
{
    public class FaceCamera : MonoBehaviour
    {
        Camera cameraToLookAt;

        void Start()
        {
            cameraToLookAt = Camera.main;
        }

        void LateUpdate()
        {
            transform.LookAt(cameraToLookAt.transform);
        }
    }
}