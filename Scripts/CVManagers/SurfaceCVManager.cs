using OpenCVMedia;
using RonplayBoxSDK;
using System;
using System.Collections;
using UnityEngine;
using UnityMediaCore;
using UnityOrbbecMedia;
using UnityRonplayBoxSDK;

namespace RonplayBoxGameDev
{
    public class SurfaceCVManager : MonoBehaviour
    {
        ////////////////////////////////////////
        // Public
        ////////////////////////////////////////

        public MatterSurface matterSurfaceLowRes = null;
        public MatterSurface matterSurfaceHighRes = null;

        public GameObject noDepthFixObject;

        ////////////////////////////////////////
        // Private
        ////////////////////////////////////////

        private void Awake()
        {
            _main_thread_manager = MainThreadManager.Instance;

            RonplayBoxSDK.CalibrationV_1_0_0.CalibrationManager.LoadGlobalSandboxParamsAndDepthFix
            (
                out _sandbox_params,
                _depth_fix
            );

            GlobalAstraDevice global_astra_device = FindObjectOfType<GlobalAstraDevice>();

            if (!global_astra_device)
            {
                var new_obj = new GameObject();
                global_astra_device = new_obj.AddComponent<GlobalAstraDevice>();
                new_obj.name = "GlobalAstraDevice";
            }
            else
            {
                // Do nothing.
            }

            _astra_device = global_astra_device.astra_device;

            if (!_astra_device.depth_stream.IsDepthStreamActive())
            {
                _astra_device.depth_stream.StartDepthStream();
            }
            else
            {
                // Do nothing.
            }

            _astra_device.depth_stream.AssignDepthFix(_depth_fix);

            if (!_depth_fix.IsValid() && noDepthFixObject != null)
            {
                StartCoroutine("WaitDepthFix");
                noDepthFixObject.SetActive(true);
            }
            else
            {
                // Do othing
            }

            _depth_fix.Reset();

            StartCoroutine("ChangeGainExplosureDelayed");
        }

        IEnumerator WaitDepthFix()
        {
            for (; ; )
            {
                yield return new WaitForSeconds(0.5f);

                _astra_device.depth_stream.TakeDepthFix(_depth_fix);

                if (_depth_fix.IsValid())
                {
                    noDepthFixObject.SetActive(false);

                    StopCoroutine("WaitDepthFix");

                    _depth_to_sand_surface_heightmap.SetParamsDelayed
                    (
                        new DepthToSandSurfaceHeightmapFilteringParams
                        (
                            min_depth_: _sandbox_params.near_depth,
                            max_depth_: _sandbox_params.far_depth,
                            half_dispersion_: _sandbox_params.half_dispersion_normal,
                            out_pixel_type_: DepthToSandSurfaceHeightmapFilteringParams.OutPixelType.U8
                        )
                    );
                }
                else
                {
                    // Do nothing.
                }

                _depth_fix.Reset();
            }
        }

        IEnumerator ChangeGainExplosureDelayed()
        {
            yield return new WaitForSeconds(1.0f);

            _astra_device.params_control.SetIRGain(_sandbox_params.sensor_params.gain);
            _astra_device.params_control.SetIRExposure(_sandbox_params.sensor_params.exposure);

        }

        private void OnDepthFixValid()
        {
            StopAllCoroutines();
        }

        private void Start()
        {
            if (_sandbox_params == null) return;

            _cv_simple_filter =
                new CVSimpleFilter
                (
                    _astra_device.depth_stream.GetDepthSamplesSource(),
                    new CVSimpleFilteringParams
                    (
                        roi_: new CVRect
                        {
                            x = _sandbox_params.depth_roi.x,
                            y = _sandbox_params.depth_roi.y,
                            width = _sandbox_params.depth_roi.width,
                            height = _sandbox_params.depth_roi.height
                        },
                        post_roi_strictly_resize_: new CVSize(),
                        post_roi_scale_resize_: new CVPoint2f
                        {
                            x = 0.25f,
                            y = 0.25f
                        },
                        post_resize_mirror_x_: _sandbox_params.depth_flip.x,
                        post_resize_mirror_y_: _sandbox_params.depth_flip.y,
                        post_mirror_linear_alpha_: 1.0,
                        post_mirror_linear_beta_: 0.0
                    ),
                    ThreadPoolExecutor.instance
                );

            _depth_to_sand_surface_heightmap =
                new DepthToSandSurfaceHeightmap
                (
                    _cv_simple_filter.GetSamplesSource(),
                    new DepthToSandSurfaceHeightmapFilteringParams
                    (
                        min_depth_: _sandbox_params.near_depth,
                        max_depth_: _sandbox_params.far_depth,
                        half_dispersion_: _sandbox_params.half_dispersion_normal,
                        out_pixel_type_: DepthToSandSurfaceHeightmapFilteringParams.OutPixelType.U8
                    ),
                    ThreadPoolExecutor.instance
                );

            _cv_simple_filter_low_res =
                new CVSimpleFilter
                (
                    _depth_to_sand_surface_heightmap.GetSamplesSource(),
                    new CVSimpleFilteringParams
                    (
                        roi_: new CVRect(),
                        post_roi_strictly_resize_:
                            new CVSize()
                            {
                                width = 33,
                                height = 21
                            },
                        post_roi_scale_resize_: new CVPoint2f(),
                        post_resize_mirror_x_: false,
                        post_resize_mirror_y_: false,
                        post_mirror_linear_alpha_: 1.0,
                        post_mirror_linear_beta_: 0.0
                    ),
                    ThreadPoolExecutor.instance
                );

            matterSurfaceHighRes.InitMatterSurface(_depth_to_sand_surface_heightmap.GetSamplesSource());
            matterSurfaceLowRes.InitMatterSurface(_cv_simple_filter_low_res.GetSamplesSource());

            matterSurfaceHighRes.GetSurfaceTextureStreamer().textureChanged.AddListener(OnTextureChanged);
        }

        private void OnTextureChanged()
        {
            matterSurfaceLowRes.GetSurfaceMesh().GetComponent<MeshRenderer>().sharedMaterial.SetTexture
            (
                "SurfaceHeightfield",
                 matterSurfaceHighRes.GetSurfaceTextureStreamer().GetStreamTexture()
            );

            //matterSurfaceLowRes.GetSurfaceMesh().GetComponent<MeshRenderer>().sharedMaterial.SetTextureScale
            //(
            //    "SurfaceHeightfield",
            //    new Vector2(1, -1)
            //);

            matterSurfaceHighRes.GetSurfaceTextureStreamer().textureChanged.RemoveListener(OnTextureChanged);
        }

        private void OnDestroy()
        {
            StopAllCoroutines();

            DisposeIfCan(_cv_simple_filter);
            DisposeIfCan(_cv_simple_filter_low_res);
            DisposeIfCan(_depth_to_sand_surface_heightmap);

            _depth_fix.Reset();
        }

        private void DisposeIfCan(IDisposable obj_)
        {
            if (obj_ == null) return;

            obj_.Dispose();
        }

        ////////////////////////////////////////
        // Private
        ////////////////////////////////////////

        private MainThreadManager _main_thread_manager = null;

        private UnityAstraDevice _astra_device = null;
        private CVSimpleFilter _cv_simple_filter = null;
        private CVSimpleFilter _cv_simple_filter_low_res = null;

        private DepthToSandSurfaceHeightmap _depth_to_sand_surface_heightmap = null;

        private readonly SharedImmutableCVMat _depth_fix = new SharedImmutableCVMat();

        private RonplayBoxSDK.CalibrationV_1_0_0.SandboxParams _sandbox_params = null;
    }
}
