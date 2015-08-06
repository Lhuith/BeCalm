// ---------------------------------------------------------------------------------------
// Custom plugin properties
// ---------------------------------------------------------------------------------------

#define FLUVIO_PLUGIN_DATA_0 Keyframe // acceleration
#define FLUVIO_PLUGIN_DATA_1 float // radius
#define FLUVIO_PLUGIN_DATA_2 int // touchPointsCount
#define FLUVIO_PLUGIN_DATA_3 float4 // touchPoints

// ---------------------------------------------------------------------------------------
// Main include
// ---------------------------------------------------------------------------------------

#include "../../Includes/FluvioCompute.cginc"

// ---------------------------------------------------------------------------------------
// Main plugin
// ---------------------------------------------------------------------------------------

FLUVIO_KERNEL(OnUpdatePlugin)
{  
    int particleIndex = get_global_id(0);

	if (FluvioShouldUpdatePlugin(particleIndex))
    {
        FLUVIO_BUFFER(Keyframe) acceleration = FluvioGetPluginBuffer(0);
        float radius = FluvioGetPluginValue(1);
        int touchPointsCount = FluvioGetPluginValue(2);
        FLUVIO_BUFFER(float4) touchPoints = FluvioGetPluginBuffer(3);
        uint seed = solverData_GetRandomSeed(particleIndex);

        for(int i = 0, l = touchPointsCount; i < l; ++i)
        {
            float4 pt = touchPoints[i];
            float4 worldPosition = solverData_GetPosition(particleIndex);
            float4 d = pt - worldPosition;
            float len = length(d);
            if (len < radius)
                solverData_AddForce(particleIndex, (d / len) * EvaluateMinMaxCurve(acceleration, len / radius, seed), FLUVIO_FORCE_MODE_ACCELERATION);
        }
    }
}
