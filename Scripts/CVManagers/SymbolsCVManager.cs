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
    public class SymbolsCVManager : MonoBehaviour
    {
        [SerializeField] private SymbolsTool symbolsTool;

        public void SetMarkersPipline()
        {
            if (_markers_tracker == null) return;

            DisposeIfCan(_latin_markers_r_i_recognizer);
            DisposeIfCan(_latin_markers_orientation_determiner);
            DisposeIfCan(_rus_markers_r_i_recognizer);
            DisposeIfCan(_rus_markers_orientation_determiner);
            DisposeIfCan(_math_markers_r_i_recognizer);
            DisposeIfCan(_math_markers_orientation_determiner);

            DisposeIfCan(_complex_symbols_detector);

            _latin_markers_r_i_recognizer = null;
            _latin_markers_orientation_determiner = null;
            _rus_markers_r_i_recognizer = null;
            _rus_markers_orientation_determiner = null;
            _math_markers_r_i_recognizer = null;
            _math_markers_orientation_determiner = null;

            _complex_symbols_detector = null;

            symbolsTool.FinalizeMarkersTool();

            switch (MarkersConfig.currentMarkersPipeline)
            {
                case MarkersPipeline.Latin:
                    SetLatinMarkersPipeline();

                    break;
                case MarkersPipeline.Cyrillic:
                    SetCyrillicMarkersPipeline();

                    break;
                case MarkersPipeline.Math:
                    SetMathMarkersPipeline();

                    break;
                default:
                    throw new Exception("Default reached!");

            }
        }

        private void Start()
        {
            _main_thread_manager = MainThreadManager.Instance;

            UnpackFileFromAPK(RonplayBoxValues.LATIN_KNN_FILE);
            UnpackFileFromAPK(RonplayBoxValues.LATIN_TABLES_FILE);
            UnpackFileFromAPK(RonplayBoxValues.RUS_KNN_FILE);
            UnpackFileFromAPK(RonplayBoxValues.RUS_TABLES_FILE);
            UnpackFileFromAPK(RonplayBoxValues.MATH_KNN_FILE);
            UnpackFileFromAPK(RonplayBoxValues.MATH_TABLES_FILE);


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
                        block_size_: MarkersIsolatorParams.BlockSize.S15
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

        private void SetLatinMarkersPipeline()
        {
            _latin_markers_r_i_recognizer =
                new LatinMarkersRIRecognizer
                (
                    _markers_tracker.GetSamplesSource(),
                    new LatinMarkersRIRecognizingParams
                    (
                        model_path_: Path.Combine(Application.persistentDataPath, RonplayBoxValues.LATIN_KNN_FILE)
                    ),
                    ThreadPoolExecutor.instance
                );

            _latin_markers_orientation_determiner =
                new LatinMarkersOrientationDeterminer
                (
                    _latin_markers_r_i_recognizer.GetSamplesSource(),
                    new LatinMarkersOrientationDeterminingParams
                    (
                        path_to_symbols_table_: Path.Combine(Application.persistentDataPath, RonplayBoxValues.LATIN_TABLES_FILE)
                    ),
                    ThreadPoolExecutor.instance
                );

            _complex_symbols_detector =
                new ComplexSymbolsDetector
                (
                    _latin_markers_orientation_determiner.GetSamplesSource(),
                    new ComplexSymbolsDetectingParams
                    (
                        path_to_composite_symbols_pattern_: Path.Combine(Application.persistentDataPath, RonplayBoxValues.LATIN_TABLES_FILE)
                    ),
                    ThreadPoolExecutor.instance
                );

            symbolsTool.InitMarkersTool(_complex_symbols_detector.GetSamplesSource());
        }

        private void SetCyrillicMarkersPipeline()
        {
            _rus_markers_r_i_recognizer =
                new RusMarkersRIRecognizer
                (
                    _markers_tracker.GetSamplesSource(),
                    new RusMarkersRIRecognizingParams
                    (
                        model_path_: Path.Combine(Application.persistentDataPath, RonplayBoxValues.RUS_KNN_FILE)
                    ),
                    ThreadPoolExecutor.instance
                );

            _rus_markers_orientation_determiner =
                new RusMarkersOrientationDeterminer
                (
                    _rus_markers_r_i_recognizer.GetSamplesSource(),
                    new RusMarkersOrientationDeterminingParams
                    (
                        path_to_symbols_table_: Path.Combine(Application.persistentDataPath, RonplayBoxValues.RUS_TABLES_FILE)
                    ),
                    ThreadPoolExecutor.instance
                );

            _complex_symbols_detector =
                new ComplexSymbolsDetector
                (
                    _rus_markers_orientation_determiner.GetSamplesSource(),
                    new ComplexSymbolsDetectingParams
                    (
                        path_to_composite_symbols_pattern_: Path.Combine(Application.persistentDataPath, RonplayBoxValues.RUS_TABLES_FILE)
                    ),
                    ThreadPoolExecutor.instance
                );

            symbolsTool.InitMarkersTool(_complex_symbols_detector.GetSamplesSource());
        }

        private void SetMathMarkersPipeline()
        {
            _math_markers_r_i_recognizer =
                new MathMarkersRIRecognizer
                (
                    _markers_tracker.GetSamplesSource(),
                    new MathMarkersRIRecognizingParams
                    (
                        model_path_: Path.Combine(Application.persistentDataPath, RonplayBoxValues.MATH_KNN_FILE)
                    ),
                    ThreadPoolExecutor.instance
                );

            _math_markers_orientation_determiner =
                new MathMarkersOrientationDeterminer
                (
                    _math_markers_r_i_recognizer.GetSamplesSource(),
                    new MathMarkersOrientationDeterminingParams
                    (
                        path_to_symbols_table_: Path.Combine(Application.persistentDataPath, RonplayBoxValues.MATH_TABLES_FILE)
                    ),
                    ThreadPoolExecutor.instance
                );

            _complex_symbols_detector =
                new ComplexSymbolsDetector
                (
                    _math_markers_orientation_determiner.GetSamplesSource(),
                    new ComplexSymbolsDetectingParams
                    (
                        path_to_composite_symbols_pattern_: Path.Combine(Application.persistentDataPath, RonplayBoxValues.MATH_TABLES_FILE)
                    ),
                    ThreadPoolExecutor.instance
                );

            symbolsTool.InitMarkersTool(_complex_symbols_detector.GetSamplesSource());
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
            DisposeIfCan(_markers_isolator_debug);
            DisposeIfCan(_markers_tracker);
            DisposeIfCan(_markers_tracker_debug);

            DisposeIfCan(_latin_markers_r_i_recognizer);
            DisposeIfCan(_latin_markers_orientation_determiner);
            DisposeIfCan(_rus_markers_r_i_recognizer);
            DisposeIfCan(_rus_markers_orientation_determiner);
            DisposeIfCan(_math_markers_r_i_recognizer);
            DisposeIfCan(_math_markers_orientation_determiner);

            DisposeIfCan(_complex_symbols_detector);
            DisposeIfCan(_complex_symbols_detector_debug);

            _cv_simple_filter = null;
            _markers_isolator = null;
            _markers_isolator_debug = null;
            _markers_tracker = null;
            _markers_tracker_debug = null;

            _latin_markers_r_i_recognizer = null;
            _latin_markers_orientation_determiner = null;
            _rus_markers_r_i_recognizer = null;
            _rus_markers_orientation_determiner = null;
            _math_markers_r_i_recognizer = null;
            _math_markers_orientation_determiner = null;

            _complex_symbols_detector = null;
            _complex_symbols_detector_debug = null;

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

        private CVSimpleFilter          _cv_simple_filter           = null;
        private MarkersIsolator         _markers_isolator           = null;
        private MarkersIsolatorDebug    _markers_isolator_debug     = null;
        private MarkersTracker          _markers_tracker            = null;
        private MarkersTrackerDebug     _markers_tracker_debug      = null;

        private LatinMarkersRIRecognizer            _latin_markers_r_i_recognizer = null;
        private LatinMarkersOrientationDeterminer   _latin_markers_orientation_determiner = null;

        private RusMarkersRIRecognizer              _rus_markers_r_i_recognizer = null;
        private RusMarkersOrientationDeterminer     _rus_markers_orientation_determiner = null;

        private MathMarkersRIRecognizer             _math_markers_r_i_recognizer = null;
        private MathMarkersOrientationDeterminer    _math_markers_orientation_determiner = null;

        private ComplexSymbolsDetector              _complex_symbols_detector = null;
        private ComplexSymbolsDetectorDebug         _complex_symbols_detector_debug = null;
    }
}

