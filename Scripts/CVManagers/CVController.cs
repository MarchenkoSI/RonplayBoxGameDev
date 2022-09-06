using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RonplayBoxGameDev
{
    public class CVController : MonoBehaviour
    {
        public static CVController instance = null;

        [SerializeField] private WordsCVManager         wordsCVManager          = null;
        [SerializeField] private SymbolsCVManager       symbolsCVManager        = null;
        [SerializeField] private PrimitiveCVManager     primitiveCVManager      = null;
        [SerializeField] private AnyMarkersCVManager    anyMarkersCVManager     = null;
        [SerializeField] private SurfaceCVManager       surfaceCVManager        = null;
        [SerializeField] private WallCVManager          wallCVManager           = null;

        public void UpdateWordsCVManager()
        {
            if (wordsCVManager == null) return;

            wordsCVManager.UpdateMarkersPipline();
        }

        public void UpdateSymbolsCVManager()
        {
            if (symbolsCVManager == null) return;

            symbolsCVManager.SetMarkersPipline();
        }

        public void UpdatePrimitiveCVManager()
        {
            if (primitiveCVManager == null) return;

            primitiveCVManager.SetMarkersPipline();
        }

        public void UpdateAnyMarkersCVManager()
        {
            if (anyMarkersCVManager == null) return;

            anyMarkersCVManager.SetMarkersPipline();
        }

        public void UpdateSurfaceCVManager()
        {
            // Do nothing.
        }

        public void UpdateWallCVManager()
        {
            // Do nothing.
        }

        private void Start()
        {
            if (instance == null)
            {
                instance = this;
            }
            else
            {
                Destroy(gameObject);
            }

            DontDestroyOnLoad(gameObject);
        }
    }
}

