// ---------------------------------------------------------------------------------------
// Custom plugin properties
// ---------------------------------------------------------------------------------------

#define FLUVIO_PLUGIN_DATA_0 int // fluidB
#define FLUVIO_PLUGIN_DATA_1 int // fluidC - 0 if null, otherwise 1
#define FLUVIO_PLUGIN_DATA_2 int // fluidD - 0 if null, otherwise 2
#define FLUVIO_PLUGIN_DATA_3 int // mixingFluidsAreTheSame
#define FLUVIO_PLUGIN_DATA_4 float // mixingDistanceSq
#define FLUVIO_PLUGIN_DATA_RW_5 int // particleSystems
#define FLUVIO_PLUGIN_DATA_RW_6 float4 // emitPositions
#define FLUVIO_PLUGIN_DATA_RW_7 float4 // emitVelocities

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

    if (!FluvioShouldUpdatePlugin(particleIndex)) return;
    
    for(EachNeighbor(particleIndex, neighborIndex))
    {
        if (!FluvioShouldUpdatePluginNeighbor(neighborIndex)) continue;

        int fluid = fluvio_PluginFluidID;
        int fluidB = FluvioGetPluginValue(0);
        int fluidC = FluvioGetPluginValue(1);
        int fluidD = FluvioGetPluginValue(2);
        int mixingFluidsAreTheSame = FluvioGetPluginValue(3);
        float mixingDistanceSq = FluvioGetPluginValue(4);

        FLUVIO_BUFFER_RW(int) particleSystems = FluvioGetPluginBuffer(5);
        FLUVIO_BUFFER_RW(float4) emitPositions = FluvioGetPluginBuffer(6);
        FLUVIO_BUFFER_RW(float4) emitVelocities = FluvioGetPluginBuffer(7);

        int particleFluid = solverData_GetFluidID(particleIndex);
        int neighborFluid = solverData_GetFluidID(neighborIndex);
     
        bool particleIsA = particleFluid == fluid;
            
        if ((mixingFluidsAreTheSame == 1 || particleFluid != neighborFluid) && // Fluids are not the same, unless A and B are the same
            (particleIsA || particleFluid == fluidB) && // First fluid is A or B
            (neighborFluid == fluid || neighborFluid == fluidB)) // Second fluid is A or B
        {

            float4 position = solverData_GetPosition(particleIndex);
            float4 neighborPosition = solverData_GetPosition(neighborIndex);
            float4 dist = position - neighborPosition;

            // Make sure the particles are actually close
            if (dot(dist,dist) > mixingDistanceSq)
                return;

            float4 velocity = solverData_GetVelocity(particleIndex);
            float4 invMass = 1.0f/solverData_GetMass(particleIndex);

            bool emitC = false;
            bool particleIsA = fluid == particleFluid;

            // Despawn the current particle, unless Fluid C is null and the particle is from Fluid A
            if (fluidC != 0 || !particleIsA)
            {
                emitC = true;
                solverData_SetLifetime(particleIndex, -1.0f);
            }

            particleSystems[particleIndex] = fluidC;

            // Emit a particle. We don't emit from the neighbor position, that gets handled in the opposite pair
            // We handle actual emission on the main thread.
            if (fluidD == 0 || particleIsA)
            {
                if (emitC)
                {
                    // Set the system to Fluid C
                    particleSystems[particleIndex] = fluidC;
                }
                else
                {
                    particleSystems[particleIndex] = 0;
                }
            }
            else
            {
                // Set the system to Fluid D
                particleSystems[particleIndex] = fluidD;
            }

            // Set emit position and velocity. These must be manually integrated since they won't be applied until after the solver has run.
            // The below is a standard Euler explicit integrator.
            float4 acceleration = solverData_GetFluid(fluid).gravity + solverData_GetForce(particleIndex)*invMass;
            float dtIter = fluvio_Time.y;
            int iterations = fluvio_Time.w;

            for (int iter = 0; iter < iterations; ++iter)
            {
                float4 t = dtIter*acceleration;
                    
                // Ignore very large velocity changes
                if (dot(t,t) > (FLUVIO_MAX_SQR_VELOCITY_CHANGE * fluvio_KernelSize.w * fluvio_KernelSize.w))
                    t *= 0;
                    
                velocity += t;
            }

            emitVelocities[particleIndex] = velocity;
            emitPositions[particleIndex] = position;
        }
    }
}
