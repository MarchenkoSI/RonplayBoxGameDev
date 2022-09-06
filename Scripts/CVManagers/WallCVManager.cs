using OpenCVMedia;
using RonplayBoxSDK;
using System;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;
using UnityMediaCore;
using UnityOpenCVMedia;
using UnityOrbbecMedia;
using UnityRonplayBoxSDK;

namespace RonplayBoxGameDev
{
    public class WallCVManager : MonoBehaviour
    {
        public WallCollisionTool wallCollisionTool;

        void Start()
        {
            RonplayBoxSDK.CalibrationV_1_0_0.CalibrationManager.LoadGlobalMarkersParams
            (
                out _markers_params
            );

            if (_markers_params == null) return;

            GetOrCreateGlobalAstraDevice();

            if (!_astra_device.depth_stream.IsDepthStreamActive())
            {
                _astra_device.depth_stream.StartDepthStream();
                _astra_device.depth_stream.AssignDepthFix(null);
            }
            else
            {
                // Do nothing.
            }

            _astra_device.params_control.SetLDPEnabled(false);
            _astra_device.params_control.SetLaserEnabled(true);
            _astra_device.params_control.SetIRExposure(1049);
            _astra_device.params_control.SetIRGain(8000);

            _cv_simple_filter =
                new CVSimpleFilter
                (
                    _astra_device.depth_stream.GetDepthSamplesSource(),
                    new CVSimpleFilteringParams
                    (
                        new CVRect()
                        {
                            x = _markers_params.infra_roi.x,
                            y = _markers_params.infra_roi.y,
                            width = _markers_params.infra_roi.width,
                            height = _markers_params.infra_roi.height
                        },
                        new CVSize(),
                        new CVPoint2f(),
                        _markers_params.infra_flip.x,
                        _markers_params.infra_flip.y,
                        1.0,
                        0.0
                    ),
                    ThreadPoolExecutor.instance
                );

            _background_recognizer =
                new BackgroundRecognizer
                (
                    _cv_simple_filter.GetSamplesSource(),
                    new BackgroundRecognizingParams
                    (
                        half_dispersion_: 75,
                        max_weight_     : 30
                    ),
                    ThreadPoolExecutor.instance
                );

            _wall_collision_detector =
                new WallCollisionDetector
                (
                    _background_recognizer.GetSamplesSource(),
                    new WallCollisionDetectingParams
                    (
                        layer_thickness_        : 300.0f,
                        with_erode_delate_      : true,
                        erode_delate_size_      : 6,
                        max_tracking_countdown_ : 2,
                        groupping_dist_         : 10.0f,
                        tracking_small_radius_  : 15.0f,
                        tracking_displacement_  : 15.0f
                    ),
                    ThreadPoolExecutor.instance
                );

            wallCollisionTool.InitWallCollisionTool
            (
                _wall_collision_detector.GetSamplesSource()
            );
        }

        /////////////////////////////////////////////
        // Private
        /////////////////////////////////////////////

        private void GetOrCreateGlobalAstraDevice()
        {
            GlobalAstraDevice gloabal_astra_device = FindObjectOfType<GlobalAstraDevice>();

            if (gloabal_astra_device == null)
            {
                var new_obj = new GameObject();
                gloabal_astra_device = new_obj.AddComponent<GlobalAstraDevice>();
                new_obj.name = "GlobalAstraDevice";
            }
            else
            {
                // Do nothing.
            }

            _astra_device = gloabal_astra_device.astra_device;
        }

        private void OnDestroy()
        {
            DisposeIfCan(_cv_simple_filter);
            DisposeIfCan(_background_recognizer);
            DisposeIfCan(_wall_collision_detector);

            _cv_simple_filter = null;
            _background_recognizer = null;
            _wall_collision_detector = null;
        }

        private void DisposeIfCan(IDisposable obj_)
        {
            if (obj_ == null) return;
            obj_.Dispose();
        }

        /////////////////////////////////////////////
        // Private
        /////////////////////////////////////////////

        private static Texture2D Create8UC3DebugTexture(int width_, int height_)
        {
            Texture2D out_texture =
                new Texture2D
                (
                    width_,
                    height_,
                    TextureFormat.RGB24,
                    1,
                    false
                );

            return out_texture;
        }

        private static unsafe void Update8UC3DebugTexture(SharedImmutableCVMat shared_cv_mat_, Texture2D texture_)
        {
            IntPtr texture_buffer_ptr = new IntPtr(texture_.GetRawTextureData<byte>().GetUnsafePtr());
            IntPtr ptr_to_data = shared_cv_mat_.Get().GetPtrToData();

            UnsafeUtility.MemCpy
            (
                texture_buffer_ptr.ToPointer(),
                ptr_to_data.ToPointer(),
                texture_.width * texture_.height * 3
            );
        }

        /////////////////////////////////////////////
        // Private
        /////////////////////////////////////////////

        private UnityAstraDevice _astra_device = null;

        private RonplayBoxSDK.CalibrationV_1_0_0.MarkersParams _markers_params = null;

        private CVSimpleFilter _cv_simple_filter = null;
        private BackgroundRecognizer _background_recognizer = null;
        private WallCollisionDetector _wall_collision_detector = null;
    }
}

