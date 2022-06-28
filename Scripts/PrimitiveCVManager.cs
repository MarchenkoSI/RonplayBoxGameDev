using OpenCVMedia;
using RonplayBoxSDK;
using System;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;
using UnityMediaCore;
using UnityOrbbecMedia;
using UnityRonplayBoxSDK;

namespace RonplayBoxGameDev
{
    public class PrimitiveCVManager : MonoBehaviour
    {
        [SerializeField] private SymbolsTool symbolsTool;

        public void SetMarkersPipline()
        {
            if (_markers_tracker == null) return;

            DisposeIfCan(_primitive_detector);

            _primitive_detector = null;

            symbolsTool.FinalizeMarkersTool();

            SetPrimitiveMarkersPipeline();
        }

        private void Start()
        {
            _main_thread_manager = MainThreadManager.Instance;

            RonplayBoxSDK.CalibrationV_1_0_0.CalibrationManager.LoadGlobalMarkersParamsAndThresholdSurface
            (
                out _markers_params,
                _threshold_surface
            );

            if (_markers_params == null || !_threshold_surface.IsValid()) return;

            GetOrCreateGlobalAstraDevice();

            if (!_astra_device.infra_stream.IsInfraStreamActive())
            {
                _astra_device.infra_stream.StartInfraStream();
            }
            else
            {
                // Do nothing.
            }

            _astra_device.params_control.SetLDPEnabled(false);
            _astra_device.params_control.SetLaserEnabled(_markers_params.sensor_params.laser_enabled);
            _astra_device.params_control.SetIRGain(_markers_params.sensor_params.gain);
            _astra_device.params_control.SetIRExposure(_markers_params.sensor_params.exposure);

            _cv_simple_filter =
                new CVSimpleFilter
                (
                    _astra_device.infra_stream.GetInfraSamplesSource(),
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

            _markers_isolator =
                new MarkersIsolator
                (
                    _cv_simple_filter.GetSamplesSource(),
                    new MarkersIsolatorParams
                    (
                        threshold_surface_: _threshold_surface,
                        min_enclosing_circle_radius_: 1.0f,
                        max_enclosing_circle_radius_: 20.0f,
                        block_size_: MarkersIsolatorParams.BlockSize.S35
                    ),
                    ThreadPoolExecutor.instance
                );

            _markers_tracker =
                new MarkersTracker
                (
                    _markers_isolator.GetSamplesSource(),
                    new MarkersTrackingParams
                    (
                        enclosing_circle_center_max_dist_: 10.0f,
                        max_total_perimeter_diff_: 15.0f,
                        max_tracking_weight_: 30
                    ),
                    ThreadPoolExecutor.instance
                );

            SetMarkersPipline();
        }

        /////////////////////////////////////////////
        // Private
        /////////////////////////////////////////////

        private void SetPrimitiveMarkersPipeline()
        {
            _primitive_detector =
                new PrimitiveDetector
                (
                    _markers_tracker.GetSamplesSource(),
                    new PrimitiveDetectingParams(),
                    ThreadPoolExecutor.instance
                );

            symbolsTool.InitMarkersTool(_primitive_detector.GetSamplesSource());
        }

        private void UnpackFileFromAPK(string file_name_)
        {
            var loading_request =
                    UnityWebRequest.Get
                    (
                        Path.Combine(Application.streamingAssetsPath, file_name_)
                    );

            loading_request.SendWebRequest();

            while (!loading_request.isDone)
            {
                // Do nothing.
            }

            File.WriteAllBytes
            (
                Path.Combine(Application.persistentDataPath, file_name_),
                loading_request.downloadHandler.data
            );
        }

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
            DisposeIfCan(_markers_isolator);
            DisposeIfCan(_markers_tracker);
            DisposeIfCan(_primitive_detector);

            _cv_simple_filter   = null;
            _markers_isolator   = null;
            _markers_tracker    = null;
            _primitive_detector = null;

            _threshold_surface.Reset();
        }

        private void DisposeIfCan(IDisposable obj_)
        {
            if (obj_ == null) return;
            obj_.Dispose();
        }

        /////////////////////////////////////////////
        // Private
        /////////////////////////////////////////////

        private MainThreadManager _main_thread_manager = null;

        private UnityAstraDevice _astra_device = null;

        private RonplayBoxSDK.CalibrationV_1_0_0.MarkersParams _markers_params = null;

        private readonly SharedImmutableCVMat _threshold_surface = new SharedImmutableCVMat();

        private CVSimpleFilter _cv_simple_filter = null;
        private MarkersIsolator _markers_isolator = null;
        private MarkersTracker _markers_tracker = null;

        private PrimitiveDetector _primitive_detector = null;
    }
}

