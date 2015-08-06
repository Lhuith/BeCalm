using UnityEngine;

namespace Thinksquirrel.Fluvio.Internal
{
    using Threading;

    // This class sets up implementations of certain platform-specific features.
    [ExecuteInEditMode]
    [AddComponentMenu("")]
    class FluvioRuntimeHelper : MonoBehaviour
    {
        void OnEnable()
        {
            // OpenCL support
            #if UNITY_EDITOR || UNITY_STANDALONE
            Cloo.Bindings.CLInterface.SetInterface(new Cloo.Bindings.CL12());
            #endif

            // Multithreading
            #if !UNITY_WEBGL && !UNITY_WINRT
            Parallel.InitializeThreadPool(new FluvioThreadPool());
            #endif
        }        
    }
}
