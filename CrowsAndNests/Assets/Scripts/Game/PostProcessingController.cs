using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;
using UnityEngine.Rendering.PostProcessing;

namespace Game {

    public class PostProcessingController : MonoBehaviour
    {
        private PostProcessVolume postProcessingVolume;
        private PostProcessProfile postProcessProfile;

        private Bloom bloom;
        private ColorGrading colorGrading;
        private Vignette vignette;
        private ChromaticAberration chromaticAberration;
        private MotionBlur motionBlur;

        private void Start()
        {
            UpdateConfig();
        }

        private void Awake()
        {
            UpdateConfig();
        }

        private void UpdateConfig() {
            postProcessingVolume = GetComponent<PostProcessVolume>();
            postProcessProfile = postProcessingVolume.profile;

            postProcessProfile.TryGetSettings(out bloom);
            postProcessProfile.TryGetSettings(out colorGrading);
            postProcessProfile.TryGetSettings(out vignette);
            postProcessProfile.TryGetSettings(out chromaticAberration);
            postProcessProfile.TryGetSettings(out motionBlur);

            SetBloomEffect(PlayerPrefs.GetInt("Bloom") == 1);
            SetColorGradingEffect(PlayerPrefs.GetInt("ColorGrading") == 1);
            SetVignetteEffect(PlayerPrefs.GetInt("Vignette") == 1);
            SetChromaticAberrationEffect(PlayerPrefs.GetInt("ChromaticAberration") == 1);
            SetMotionBlurEffect(PlayerPrefs.GetInt("MotionBlur") == 1);
        }

       private void SetBloomEffect(bool isEnabled)
        {
            if (bloom != null)
            {
                bloom.enabled.value = isEnabled;
                Debug.Log(GameGlobal.Util.BuildMessage(typeof(PostProcessingController), "Bloom set on: " + isEnabled.ToString()));
            } 
            else
            {
                Debug.Log(GameGlobal.Util.BuildMessage(typeof(PostProcessingController), "Bloom is null"));
            }
        }

        private void SetColorGradingEffect(bool isEnabled)
        {
            if (colorGrading != null)
            {
                colorGrading.enabled.value = isEnabled;
                Debug.Log(GameGlobal.Util.BuildMessage(typeof(PostProcessingController), "Color Grading set on: " + colorGrading.ToString()));
            } 
            else
            {
                Debug.Log(GameGlobal.Util.BuildMessage(typeof(PostProcessingController), "Color Grading is null"));
            }
        }

        private void SetVignetteEffect(bool isEnabled)
        {
            if (vignette != null)
            {
                vignette.enabled.value = isEnabled;
                Debug.Log(GameGlobal.Util.BuildMessage(typeof(PostProcessingController), "Vignette set on: " + vignette.ToString()));
            }
            else
            {
                Debug.Log(GameGlobal.Util.BuildMessage(typeof(PostProcessingController), "Vignette is null"));
            }
        }

        private void SetChromaticAberrationEffect(bool isEnabled)
        {
            if (chromaticAberration != null)
            {
                chromaticAberration.enabled.value = isEnabled;
                Debug.Log(GameGlobal.Util.BuildMessage(typeof(PostProcessingController), "Chromatic Aberration set on: " + chromaticAberration.ToString()));
            }
            else
            {
                Debug.Log(GameGlobal.Util.BuildMessage(typeof(PostProcessingController), "Chromatic Aberration is null"));
            }
        }

        private void SetMotionBlurEffect(bool isEnabled)
        {
            if (motionBlur != null)
            {
                motionBlur.enabled.value = isEnabled;
                Debug.Log(GameGlobal.Util.BuildMessage(typeof(PostProcessingController), "Motion Blur set on: " + motionBlur.ToString()));
            }
            else
            {
                Debug.Log(GameGlobal.Util.BuildMessage(typeof(PostProcessingController), "Motion Blur is null"));
            }
        }
    }

}