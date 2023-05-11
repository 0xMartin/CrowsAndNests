using UnityEngine;
using Cinemachine;

namespace Game
{

    public class CameraYRestrictor : CinemachineExtension
    {
        public float minimumY = -5f; 

        protected override void PostPipelineStageCallback(CinemachineVirtualCameraBase vcam, 
                                                        CinemachineCore.Stage stage, 
                                                        ref CameraState state, 
                                                        float deltaTime)
        {
            if (stage == CinemachineCore.Stage.Body)
            {
                Vector3 cameraPosition = state.FinalPosition; // Získání pozice kamery
                cameraPosition.y = Mathf.Max(cameraPosition.y, minimumY); // Omezení pozice kamery v ose Y
                state.PositionCorrection += cameraPosition - state.FinalPosition; // Nastavení korekce pozice kamery
            }
        }
    }

}