#ifndef FLUVIO_COMPUTE_INCLUDED
#define FLUVIO_COMPUTE_INCLUDED

// We include this manually due to how nested includes work currently with the HLSL compiler
#ifndef FLUVIO_LANG_INCLUDED
#define FLUVIO_LANG_INCLUDED

// Define FALSE as 0 and TRUE as non-zero
#define FALSE 0
#define TRUE !FALSE

// OpenCL/HLSL specific syntax
#ifdef FLUVIO_API_OPENCL
    // Buffers
    #define FLUVIO_BUFFER(bufferType) __global __read_only const bufferType*
    #define FLUVIO_BUFFER_RW(bufferType) __global bufferType*
    #define FLUVIO_INITIALIZE(type) ((type)(0))
    inline float lerp(float a, float b, float w) { return a + w*(b-a); }
    inline float2 lerp2(float2 a, float2 b, float w) { return a + w*(b-a); }
    inline float3 lerp3(float3 a, float3 b, float w) { return a + w*(b-a); }
    inline float4 lerp4(float4 a, float4 b, float w) { return a + w*(b-a); }
    #define mat4x4 float16
    inline float4 mul3x4(mat4x4 M, float4 v)
    {
        float4 r;

        float4 a = M.s048C;
        float4 b = M.s159D;
        float4 c = M.s26AE;

        r.x = dot(a.xyz, v.xyz) + a.w;
        r.y = dot(b.xyz, v.xyz) + b.w;
        r.z = dot(c.xyz, v.xyz) + c.w;
        r.w = v.w;

        return r;
    }
    inline float4 mulv(mat4x4 M, float4 v)
    {
        float4 r;

        float3 a = M.s048;
        float3 b = M.s159;
        float3 c = M.s26A;

        r.x = dot(a, v.xyz);
        r.y = dot(b, v.xyz);
        r.z = dot(c, v.xyz);
        r.w = v.w;

        return r;
    }
#else
    #ifndef FLUVIO_API_UNITYNATIVE
        #define FLUVIO_API_UNITYNATIVE
    #endif
    #define FLUVIO_BUFFER(bufferType) StructuredBuffer<bufferType>
    #define FLUVIO_BUFFER_RW(bufferType) globallycoherent RWStructuredBuffer<bufferType>
    #define FLUVIO_INITIALIZE(type) ((type)0)
    #define lerp2 lerp
    #define lerp3 lerp
    #define lerp4 lerp
    #define fabs abs
    #define mat4x4 float4x4
    inline float4 mul3x4(mat4x4 M, float4 v)
    {
        float4 r;

        float4 a = float4(M._m00, M._m01, M._m02, M._m03);
        float4 b = float4(M._m10, M._m11, M._m12, M._m13);
        float4 c = float4(M._m20, M._m21, M._m22, M._m23);

        r.x = dot(a.xyz, v.xyz) + a.w;
        r.y = dot(b.xyz, v.xyz) + b.w;
        r.z = dot(c.xyz, v.xyz) + c.w;
        r.w = v.w;

        return r;
    }
    inline float4 mulv(mat4x4 M, float4 v)
    {
        float4 r;

        float3 a = float3(M._m00, M._m01, M._m02);
        float3 b = float3(M._m10, M._m11, M._m12);
        float3 c = float3(M._m20, M._m21, M._m22);

        r.x = dot(a, v.xyz);
        r.y = dot(b, v.xyz);
        r.z = dot(c, v.xyz);
        r.w = v.w;

        return r;
    }
#endif

// Fluid data struct
typedef struct {
    float4 gravity;
    float initialDensity;
    float minimumDensity;
    float particleMass;
    float viscosity;
    float turbulence;
    float surfaceTension;
    float gasConstant;
    float buoyancyCoefficient;
} FluidData;

// Keyframe struct
typedef struct {
    float time;
    float value;
    float inTangent;
    float outTangent;
    int tangentMode;
} Keyframe;

// Gradient color key struct
typedef struct {
    float cR;
    float cG;
    float cB;
    float cA;
    float time;
} GradientColorKey;

// Gradient alpha key struct
typedef struct {
    float alpha;
    float time;
} GradientAlphaKey;

#endif // FLUVIO_LANG_INCLUDED

#define FLUVIO_PI 3.14159265359f

#define FLUVIO_THREAD_GROUP_SIZE 128 // FluvioSettings.kComputeThreadGroupSize
#define FLUVIO_GRID_BUCKET_SIZE 32 // FluvioSettings.kGridBucketSize
#define FLUVIO_MAX_GRID_SIZE 32 // FluvioSettings.kMaxGridSize
#define FLUVIO_EPSILON 0.0001f // FluvioSettings.kEpsilon
#define FLUVIO_MAX_SQR_VELOCITY_CHANGE 100.0f // FluvioSettings.kMaxSqrVelocityChange
#define FLUVIO_TURBULENCE_CONSTANT 10.0f // FluvioSettings.kTurbulenceConstant

// Force mode
#define FLUVIO_FORCE_MODE_FORCE 0
#define FLUVIO_FORCE_MODE_ACCELERATION 1
#define FLUVIO_FORCE_MODE_IMPULSE 2
#define FLUVIO_FORCE_MODE_VELOCITY_CHANGE 3

// Solver read-write only buffers
#ifdef FLUVIO_SOLVER
    #define FLUVIO_BUFFER_SOLVER_RW(bufferType) FLUVIO_BUFFER_RW(bufferType)
#else
    #define FLUVIO_PLUGIN
    #define FLUVIO_BUFFER_SOLVER_RW(bufferType) FLUVIO_BUFFER(bufferType)
#endif

// Kernel entry point mega-macro
// OpenCL passes data in the form of arguments, wheras Unity compute shaders do not.
// This macro takes care of all of that.
// To use this macro, buffer types need to be provided. For example:
//		#define FLUVIO_PLUGIN_DATA_0 float4
//		#define FLUVIO_PLUGIN_DATA_RW_1 float
// Up to 10 buffers are supported (one for each slot). Use the FluvioGetPluginBuffer/FluvioGetPluginValue macros to get plugin data.
// Min-max curves should be defined as the type Keyframe.
// Min-max gradients use two buffers and should be defined as the types GradientColorKey and GradientAlphaKey.
#ifdef FLUVIO_API_OPENCL

    #ifdef FLUVIO_PLUGIN
        #define FLUVIO_PLUGIN_ARG_0
        #define FLUVIO_PLUGIN_ARG_1
        #define FLUVIO_PLUGIN_ARG_2
        #define FLUVIO_PLUGIN_ARG_3
        #define FLUVIO_PLUGIN_ARG_4
        #define FLUVIO_PLUGIN_ARG_5
        #define FLUVIO_PLUGIN_ARG_6
        #define FLUVIO_PLUGIN_ARG_7
        #define FLUVIO_PLUGIN_ARG_8
        #define FLUVIO_PLUGIN_ARG_9
        
        #ifdef FLUVIO_PLUGIN_DATA_0
            #undef FLUVIO_PLUGIN_ARG_0
            #define FLUVIO_PLUGIN_ARG_0 , FLUVIO_BUFFER(FLUVIO_PLUGIN_DATA_0) fluvio_PluginData0
        #endif
        #ifdef FLUVIO_PLUGIN_DATA_1
            #undef FLUVIO_PLUGIN_ARG_1
            #define FLUVIO_PLUGIN_ARG_1 , FLUVIO_BUFFER(FLUVIO_PLUGIN_DATA_1) fluvio_PluginData1
        #endif
        #ifdef FLUVIO_PLUGIN_DATA_2
            #undef FLUVIO_PLUGIN_ARG_2
            #define FLUVIO_PLUGIN_ARG_2 , FLUVIO_BUFFER(FLUVIO_PLUGIN_DATA_2) fluvio_PluginData2
        #endif
        #ifdef FLUVIO_PLUGIN_DATA_3
            #undef FLUVIO_PLUGIN_ARG_3
            #define FLUVIO_PLUGIN_ARG_3 , FLUVIO_BUFFER(FLUVIO_PLUGIN_DATA_3) fluvio_PluginData3
        #endif
        #ifdef FLUVIO_PLUGIN_DATA_4
            #undef FLUVIO_PLUGIN_ARG_4
            #define FLUVIO_PLUGIN_ARG_4 , FLUVIO_BUFFER(FLUVIO_PLUGIN_DATA_4) fluvio_PluginData4
        #endif
        #ifdef FLUVIO_PLUGIN_DATA_5
            #undef FLUVIO_PLUGIN_ARG_5
            #define FLUVIO_PLUGIN_ARG_5 , FLUVIO_BUFFER(FLUVIO_PLUGIN_DATA_5) fluvio_PluginData5
        #endif
        #ifdef FLUVIO_PLUGIN_DATA_6
            #undef FLUVIO_PLUGIN_ARG_6
            #define FLUVIO_PLUGIN_ARG_6 , FLUVIO_BUFFER(FLUVIO_PLUGIN_DATA_6) fluvio_PluginData6
        #endif
        #ifdef FLUVIO_PLUGIN_DATA_7
            #undef FLUVIO_PLUGIN_ARG_7
            #define FLUVIO_PLUGIN_ARG_7 , FLUVIO_BUFFER(FLUVIO_PLUGIN_DATA_7) fluvio_PluginData7
        #endif
        #ifdef FLUVIO_PLUGIN_DATA_8
            #undef FLUVIO_PLUGIN_ARG_8
            #define FLUVIO_PLUGIN_ARG_8 , FLUVIO_BUFFER(FLUVIO_PLUGIN_DATA_8) fluvio_PluginData8
        #endif
        #ifdef FLUVIO_PLUGIN_DATA_9
            #undef FLUVIO_PLUGIN_ARG_9
            #define FLUVIO_PLUGIN_ARG_9 , FLUVIO_BUFFER(FLUVIO_PLUGIN_DATA_9) fluvio_PluginData9
        #endif
        
        #ifdef FLUVIO_PLUGIN_DATA_RW_0
            #undef FLUVIO_PLUGIN_ARG_0
            #define FLUVIO_PLUGIN_ARG_0 , FLUVIO_BUFFER_RW(FLUVIO_PLUGIN_DATA_RW_0) fluvio_PluginData0
        #endif
        #ifdef FLUVIO_PLUGIN_DATA_RW_1
            #undef FLUVIO_PLUGIN_ARG_1
            #define FLUVIO_PLUGIN_ARG_1 , FLUVIO_BUFFER_RW(FLUVIO_PLUGIN_DATA_RW_1) fluvio_PluginData1
        #endif
        #ifdef FLUVIO_PLUGIN_DATA_RW_2
            #undef FLUVIO_PLUGIN_ARG_2
            #define FLUVIO_PLUGIN_ARG_2 , FLUVIO_BUFFER_RW(FLUVIO_PLUGIN_DATA_RW_2) fluvio_PluginData2
        #endif
        #ifdef FLUVIO_PLUGIN_DATA_RW_3
            #undef FLUVIO_PLUGIN_ARG_3
            #define FLUVIO_PLUGIN_ARG_3 , FLUVIO_BUFFER_RW(FLUVIO_PLUGIN_DATA_RW_3) fluvio_PluginData3
        #endif
        #ifdef FLUVIO_PLUGIN_DATA_RW_4
            #undef FLUVIO_PLUGIN_ARG_4
            #define FLUVIO_PLUGIN_ARG_4 , FLUVIO_BUFFER_RW(FLUVIO_PLUGIN_DATA_RW_4) fluvio_PluginData4
        #endif
        #ifdef FLUVIO_PLUGIN_DATA_RW_5
            #undef FLUVIO_PLUGIN_ARG_5
            #define FLUVIO_PLUGIN_ARG_5 , FLUVIO_BUFFER_RW(FLUVIO_PLUGIN_DATA_RW_5) fluvio_PluginData5
        #endif
        #ifdef FLUVIO_PLUGIN_DATA_RW_6
            #undef FLUVIO_PLUGIN_ARG_6
            #define FLUVIO_PLUGIN_ARG_6 , FLUVIO_BUFFER_RW(FLUVIO_PLUGIN_DATA_RW_6) fluvio_PluginData6
        #endif
        #ifdef FLUVIO_PLUGIN_DATA_RW_7
            #undef FLUVIO_PLUGIN_ARG_7
            #define FLUVIO_PLUGIN_ARG_7 , FLUVIO_BUFFER_RW(FLUVIO_PLUGIN_DATA_RW_7) fluvio_PluginData7
        #endif
        #ifdef FLUVIO_PLUGIN_DATA_RW_8
            #undef FLUVIO_PLUGIN_ARG_8
            #define FLUVIO_PLUGIN_ARG_8 , FLUVIO_BUFFER(FLUVIO_PLUGIN_DATA_RW_8) fluvio_PluginData8
        #endif
        #ifdef FLUVIO_PLUGIN_DATA_RW_9
            #undef FLUVIO_PLUGIN_ARG_9
            #define FLUVIO_PLUGIN_ARG_9 , FLUVIO_BUFFER(FLUVIO_PLUGIN_DATA_RW_9) fluvio_PluginData9
        #endif
    #endif

    #ifdef FLUVIO_SOLVER
        #define FLUVIO_KERNEL_INDEX_GRID(kernelName) \
            __kernel \
            void kernelName( \
                int fluvio_FluidCount, \
                int fluvio_ParticleCount, \
                int fluvio_DynamicParticleCount, \
                int fluvio_Stride, \
                float4 fluvio_KernelSize, \
                float4 fluvio_KernelFactors, \
                float4 fluvio_Time, \
                FLUVIO_BUFFER(FluidData) fluvio_Fluid, \
                FLUVIO_BUFFER(int) fluvio_FluidID, \
                FLUVIO_BUFFER(int) fluvio_ParticleID, \
                FLUVIO_BUFFER(uint) fluvio_RandomSeed, \
                FLUVIO_BUFFER_SOLVER_RW(float4) fluvio_Position, \
                FLUVIO_BUFFER_RW(float4) fluvio_Velocity, \
                FLUVIO_BUFFER_RW(float4) fluvio_Force, \
                FLUVIO_BUFFER_SOLVER_RW(float) fluvio_Density, \
                FLUVIO_BUFFER_SOLVER_RW(float) fluvio_Pressure, \
                FLUVIO_BUFFER_SOLVER_RW(float4) fluvio_Normal, \
                FLUVIO_BUFFER_SOLVER_RW(float) fluvio_NormalLen, \
                FLUVIO_BUFFER_RW(float) fluvio_Turbulence, \
                FLUVIO_BUFFER_RW(float4) fluvio_Vorticity, \
                FLUVIO_BUFFER_RW(float) fluvio_Lifetime, \
                FLUVIO_BUFFER_RW(float4) fluvio_Color, \
                FLUVIO_BUFFER_SOLVER_RW(int) fluvio_NeighborCount, \
                FLUVIO_BUFFER_SOLVER_RW(int) fluvio_Neighbors, \
                FLUVIO_BUFFER_SOLVER_RW(int) fluvio_IndexGrid, \
                int fluvio_IndexGridDepth)
        #define FLUVIO_KERNEL(kernelName) \
            __kernel \
            void kernelName( \
                int fluvio_FluidCount, \
                int fluvio_ParticleCount, \
                int fluvio_DynamicParticleCount, \
                int fluvio_Stride, \
                float4 fluvio_KernelSize, \
                float4 fluvio_KernelFactors, \
                float4 fluvio_Time, \
                FLUVIO_BUFFER(FluidData) fluvio_Fluid, \
                FLUVIO_BUFFER(int) fluvio_FluidID, \
                FLUVIO_BUFFER(int) fluvio_ParticleID, \
                FLUVIO_BUFFER(uint) fluvio_RandomSeed, \
                FLUVIO_BUFFER_SOLVER_RW(float4) fluvio_Position, \
                FLUVIO_BUFFER_RW(float4) fluvio_Velocity, \
                FLUVIO_BUFFER_RW(float4) fluvio_Force, \
                FLUVIO_BUFFER_SOLVER_RW(float) fluvio_Density, \
                FLUVIO_BUFFER_SOLVER_RW(float) fluvio_Pressure, \
                FLUVIO_BUFFER_SOLVER_RW(float4) fluvio_Normal, \
                FLUVIO_BUFFER_SOLVER_RW(float) fluvio_NormalLen, \
                FLUVIO_BUFFER_RW(float) fluvio_Turbulence, \
                FLUVIO_BUFFER_RW(float4) fluvio_Vorticity, \
                FLUVIO_BUFFER_RW(float) fluvio_Lifetime, \
                FLUVIO_BUFFER_RW(float4) fluvio_Color, \
                FLUVIO_BUFFER_SOLVER_RW(int) fluvio_NeighborCount, \
                FLUVIO_BUFFER_SOLVER_RW(int) fluvio_Neighbors)
    #else
        #define FLUVIO_KERNEL(kernelName) \
            __kernel \
            void kernelName( \
                int fluvio_FluidCount, \
                int fluvio_ParticleCount, \
                int fluvio_DynamicParticleCount, \
                int fluvio_Stride, \
                float4 fluvio_KernelSize, \
                float4 fluvio_KernelFactors, \
                float4 fluvio_Time, \
                FLUVIO_BUFFER(FluidData) fluvio_Fluid, \
                FLUVIO_BUFFER(int) fluvio_FluidID, \
                FLUVIO_BUFFER(int) fluvio_ParticleID, \
                FLUVIO_BUFFER(uint) fluvio_RandomSeed, \
                FLUVIO_BUFFER_SOLVER_RW(float4) fluvio_Position, \
                FLUVIO_BUFFER_RW(float4) fluvio_Velocity, \
                FLUVIO_BUFFER_RW(float4) fluvio_Force, \
                FLUVIO_BUFFER_SOLVER_RW(float) fluvio_Density, \
                FLUVIO_BUFFER_SOLVER_RW(float) fluvio_Pressure, \
                FLUVIO_BUFFER_SOLVER_RW(float4) fluvio_Normal, \
                FLUVIO_BUFFER_SOLVER_RW(float) fluvio_NormalLen, \
                FLUVIO_BUFFER_RW(float) fluvio_Turbulence, \
                FLUVIO_BUFFER_RW(float4) fluvio_Vorticity, \
                FLUVIO_BUFFER_RW(float) fluvio_Lifetime, \
                FLUVIO_BUFFER_RW(float4) fluvio_Color, \
                FLUVIO_BUFFER_SOLVER_RW(int) fluvio_NeighborCount, \
                FLUVIO_BUFFER_SOLVER_RW(int) fluvio_Neighbors, \
                int fluvio_IncludeFluidGroup, \
                int fluvio_PluginFluidID \
                FLUVIO_PLUGIN_ARG_0 \
                FLUVIO_PLUGIN_ARG_1 \
                FLUVIO_PLUGIN_ARG_2 \
                FLUVIO_PLUGIN_ARG_3 \
                FLUVIO_PLUGIN_ARG_4 \
                FLUVIO_PLUGIN_ARG_5 \
                FLUVIO_PLUGIN_ARG_6 \
                FLUVIO_PLUGIN_ARG_7 \
                FLUVIO_PLUGIN_ARG_8 \
                FLUVIO_PLUGIN_ARG_9)
    #endif
#else
    #ifdef FLUVIO_PLUGIN
        #define FLUVIO_PLUGIN_DECL_0
        #define FLUVIO_PLUGIN_DECL_1
        #define FLUVIO_PLUGIN_DECL_2
        #define FLUVIO_PLUGIN_DECL_3
        #define FLUVIO_PLUGIN_DECL_4
        #define FLUVIO_PLUGIN_DECL_5
        #define FLUVIO_PLUGIN_DECL_6
        #define FLUVIO_PLUGIN_DECL_7
        #define FLUVIO_PLUGIN_DECL_8
        #define FLUVIO_PLUGIN_DECL_9
        
        #ifdef FLUVIO_PLUGIN_DATA_0
            #undef FLUVIO_PLUGIN_DECL_0
            #define FLUVIO_PLUGIN_DECL_0 FLUVIO_BUFFER(FLUVIO_PLUGIN_DATA_0) fluvio_PluginData0
        #endif
        #ifdef FLUVIO_PLUGIN_DATA_1
            #undef FLUVIO_PLUGIN_DECL_1
            #define FLUVIO_PLUGIN_DECL_1 FLUVIO_BUFFER(FLUVIO_PLUGIN_DATA_1) fluvio_PluginData1
        #endif
        #ifdef FLUVIO_PLUGIN_DATA_2
            #undef FLUVIO_PLUGIN_DECL_2
            #define FLUVIO_PLUGIN_DECL_2 FLUVIO_BUFFER(FLUVIO_PLUGIN_DATA_2) fluvio_PluginData2
        #endif
        #ifdef FLUVIO_PLUGIN_DATA_3
            #undef FLUVIO_PLUGIN_DECL_3
            #define FLUVIO_PLUGIN_DECL_3 FLUVIO_BUFFER(FLUVIO_PLUGIN_DATA_3) fluvio_PluginData3
        #endif
        #ifdef FLUVIO_PLUGIN_DATA_4
            #undef FLUVIO_PLUGIN_DECL_4
            #define FLUVIO_PLUGIN_DECL_4 FLUVIO_BUFFER(FLUVIO_PLUGIN_DATA_4) fluvio_PluginData4
        #endif
        #ifdef FLUVIO_PLUGIN_DATA_5
            #undef FLUVIO_PLUGIN_DECL_5
            #define FLUVIO_PLUGIN_DECL_5 FLUVIO_BUFFER(FLUVIO_PLUGIN_DATA_5) fluvio_PluginData5
        #endif
        #ifdef FLUVIO_PLUGIN_DATA_6
            #undef FLUVIO_PLUGIN_DECL_6
            #define FLUVIO_PLUGIN_DECL_6 FLUVIO_BUFFER(FLUVIO_PLUGIN_DATA_6) fluvio_PluginData6
        #endif
        #ifdef FLUVIO_PLUGIN_DATA_7
            #undef FLUVIO_PLUGIN_DECL_7
            #define FLUVIO_PLUGIN_DECL_7 FLUVIO_BUFFER(FLUVIO_PLUGIN_DATA_7) fluvio_PluginData7
        #endif
        #ifdef FLUVIO_PLUGIN_DATA_8
            #undef FLUVIO_PLUGIN_DECL_8
            #define FLUVIO_PLUGIN_DECL_8 FLUVIO_BUFFER(FLUVIO_PLUGIN_DATA_8) fluvio_PluginData8
        #endif
        #ifdef FLUVIO_PLUGIN_DATA_9
            #undef FLUVIO_PLUGIN_DECL_9
            #define FLUVIO_PLUGIN_DECL_9 FLUVIO_BUFFER(FLUVIO_PLUGIN_DATA_9) fluvio_PluginData9
        #endif
                
        #ifdef FLUVIO_PLUGIN_DATA_RW_0
            #undef FLUVIO_PLUGIN_DECL_0
            #define FLUVIO_PLUGIN_DECL_0 FLUVIO_BUFFER_RW(FLUVIO_PLUGIN_DATA_RW_0) fluvio_PluginData0
        #endif
        #ifdef FLUVIO_PLUGIN_DATA_RW_1
            #undef FLUVIO_PLUGIN_DECL_1
            #define FLUVIO_PLUGIN_DECL_1 FLUVIO_BUFFER_RW(FLUVIO_PLUGIN_DATA_RW_1) fluvio_PluginData1
        #endif
        #ifdef FLUVIO_PLUGIN_DATA_RW_2
            #undef FLUVIO_PLUGIN_DECL_2
            #define FLUVIO_PLUGIN_DECL_2 FLUVIO_BUFFER_RW(FLUVIO_PLUGIN_DATA_RW_2) fluvio_PluginData2
        #endif
        #ifdef FLUVIO_PLUGIN_DATA_RW_3
            #undef FLUVIO_PLUGIN_DECL_3
            #define FLUVIO_PLUGIN_DECL_3 FLUVIO_BUFFER_RW(FLUVIO_PLUGIN_DATA_RW_3) fluvio_PluginData3
        #endif
        #ifdef FLUVIO_PLUGIN_DATA_RW_4
            #undef FLUVIO_PLUGIN_DECL_4
            #define FLUVIO_PLUGIN_DECL_4 FLUVIO_BUFFER_RW(FLUVIO_PLUGIN_DATA_RW_4) fluvio_PluginData4
        #endif
        #ifdef FLUVIO_PLUGIN_DATA_RW_5
            #undef FLUVIO_PLUGIN_DECL_5
            #define FLUVIO_PLUGIN_DECL_5 FLUVIO_BUFFER_RW(FLUVIO_PLUGIN_DATA_RW_5) fluvio_PluginData5
        #endif
        #ifdef FLUVIO_PLUGIN_DATA_RW_6
            #undef FLUVIO_PLUGIN_DECL_6
            #define FLUVIO_PLUGIN_DECL_6 FLUVIO_BUFFER_RW(FLUVIO_PLUGIN_DATA_RW_6) fluvio_PluginData6
        #endif
        #ifdef FLUVIO_PLUGIN_DATA_RW_7
            #undef FLUVIO_PLUGIN_DECL_7
            #define FLUVIO_PLUGIN_DECL_7 FLUVIO_BUFFER_RW(FLUVIO_PLUGIN_DATA_RW_7) fluvio_PluginData7
        #endif
        #ifdef FLUVIO_PLUGIN_DATA_RW_8
            #undef FLUVIO_PLUGIN_DECL_8
            #define FLUVIO_PLUGIN_DECL_8 FLUVIO_BUFFER(FLUVIO_PLUGIN_DATA_RW_8) fluvio_PluginData8
        #endif
        #ifdef FLUVIO_PLUGIN_DATA_RW_9
            #undef FLUVIO_PLUGIN_DECL_9
            #define FLUVIO_PLUGIN_DECL_9 FLUVIO_BUFFER(FLUVIO_PLUGIN_DATA_RW_9) fluvio_PluginData9
        #endif        
    #endif

    // Variable declaration
    int fluvio_FluidCount;
    int fluvio_ParticleCount;
    int fluvio_DynamicParticleCount;
    int fluvio_Stride;
    float4 fluvio_KernelSize;
    float4 fluvio_KernelFactors;
    float4 fluvio_Time;
    FLUVIO_BUFFER(FluidData) fluvio_Fluid;
    FLUVIO_BUFFER(int) fluvio_FluidID;
    FLUVIO_BUFFER(int) fluvio_ParticleID;
    FLUVIO_BUFFER(uint) fluvio_RandomSeed;
    FLUVIO_BUFFER_SOLVER_RW(float4) fluvio_Position;
    FLUVIO_BUFFER_RW(float4) fluvio_Velocity;
    FLUVIO_BUFFER_RW(float4) fluvio_Force;
    FLUVIO_BUFFER_SOLVER_RW(float) fluvio_Density;
    FLUVIO_BUFFER_SOLVER_RW(float) fluvio_Pressure;
    FLUVIO_BUFFER_SOLVER_RW(float4) fluvio_Normal;
    FLUVIO_BUFFER_SOLVER_RW(float) fluvio_NormalLen;
    FLUVIO_BUFFER_RW(float) fluvio_Turbulence;
    FLUVIO_BUFFER_RW(float4) fluvio_Vorticity;
    FLUVIO_BUFFER_RW(float) fluvio_Lifetime;
    FLUVIO_BUFFER_RW(float4) fluvio_Color;
    FLUVIO_BUFFER_SOLVER_RW(int) fluvio_NeighborCount;
    FLUVIO_BUFFER_SOLVER_RW(int) fluvio_Neighbors;
    FLUVIO_BUFFER_SOLVER_RW(int) fluvio_IndexGrid;
    int fluvio_IndexGridDepth;

    #ifdef FLUVIO_PLUGIN
        int fluvio_IncludeFluidGroup;
        int fluvio_PluginFluidID;
        FLUVIO_PLUGIN_DECL_0;
        FLUVIO_PLUGIN_DECL_1;
        FLUVIO_PLUGIN_DECL_2;
        FLUVIO_PLUGIN_DECL_3;
        FLUVIO_PLUGIN_DECL_4;
        FLUVIO_PLUGIN_DECL_5;
        FLUVIO_PLUGIN_DECL_6;
        FLUVIO_PLUGIN_DECL_7;
        FLUVIO_PLUGIN_DECL_8;
        FLUVIO_PLUGIN_DECL_9;
    #endif

    #define FLUVIO_KERNEL(kernelName) \
        [numthreads(FLUVIO_THREAD_GROUP_SIZE,1,1)]\
        void kernelName(int3 fluvio_DispatchThreadID : SV_DispatchThreadID)
    #define FLUVIO_KERNEL_INDEX_GRID(kernelName) FLUVIO_KERNEL(kernelName)
#endif

// ---------------------------------------------------------------------------------------
// Helper macros
// ---------------------------------------------------------------------------------------

// Get the plugin data at the specified index
#define FluvioGetPluginBuffer(index) fluvio_PluginData##index
#define FluvioGetPluginValue(index) fluvio_PluginData##index[0]

// Particle index
#ifdef FLUVIO_API_UNITYNATIVE
    #define get_global_id(i) fluvio_DispatchThreadID[(i)]
#endif

// Macro to iterate through particle neighbors
#define EachNeighbor(particleIndex, neighborIndex) int fluvio_neighborIndexP = particleIndex*fluvio_Stride, neighborIndex = fluvio_Neighbors[fluvio_neighborIndexP]; fluvio_neighborIndexP < particleIndex*fluvio_Stride + fluvio_NeighborCount[particleIndex]; ++fluvio_neighborIndexP, neighborIndex = fluvio_Neighbors[fluvio_neighborIndexP]

// Returns true if the current particle is alive
#define FluvioParticleIsAlive(particleIndex) (fluvio_Lifetime[(particleIndex)] >= 0 && (particleIndex) < fluvio_DynamicParticleCount)

// Returns true if the plugin should be updated for this particle
#define FluvioShouldUpdatePlugin(particleIndex) ((particleIndex) < fluvio_DynamicParticleCount && FluvioParticleIsAlive((particleIndex)) && particleIndex < fluvio_DynamicParticleCount && (fluvio_IncludeFluidGroup == 1 || fluvio_PluginFluidID == fluvio_FluidID[(particleIndex)]))

// Returns true if the plugin should be updated for this particle neighbor
#define FluvioShouldUpdatePluginNeighbor(neighborIndex) (FluvioParticleIsAlive((neighborIndex)) && (fluvio_IncludeFluidGroup == 1 || fluvio_PluginFluidID == fluvio_FluidID[(neighborIndex)]))

// ---------------------------------------------------------------------------------------
// Helper functions
// ---------------------------------------------------------------------------------------

// Gets the neighbor index at the specified offset (from 0 to stride)
inline int FluvioGetNeighborIndex(FLUVIO_BUFFER_SOLVER_RW(int) neighbors, int particleIndex, int stride, int offset)
{
    return neighbors[particleIndex * stride + offset];
}

#define FluvioPositive(x) 0.5f*(x + max(x,-x))
#define FluvioRenorm(x, upper, lower) 1.0f-FluvioPositive(1.0f-FluvioPositive((x-lower)/(upper-lower)))

// Performs a mathematical branch:
//      if (x > y) return a;
//      else return b;
// Assumes y is greater than 0.001.
inline float FluvioRenormBranch(float x, float y, float a, float b)
{
    return lerp(b, a, FluvioRenorm(x, y + 0.001f, y - 0.001f));
}

// Clamp length
inline float3 clamp_len(float3 v, float len)
{
    return (dot(v, v) > (len * len)) ? (normalize(v) * len) : v;
}

// Inverse lerp
inline float invlerp(float from, float to, float value)
{
    if (from < to)
    {
        if (value < from)
            return 0.0f;
        if (value > to)
            return 1.0f;
        value -= from;
        value /= to - from;
        return value;
    }
    else
    {
        if (from <= to)
            return 0.0f;
        if (value < to)
            return 1.0f;
        if (value > from)
            return 0.0f;
        else
            return 1.0 - (value - to) / (from - to);
    }
}

// Random numbers
inline uint WangHash(uint seed)
{
    seed = (seed ^ 61) ^ (seed >> 16);
    seed *= 9;
    seed = seed ^ (seed >> 4);
    seed *= 0x27d4eb2d;
    seed = seed ^ (seed >> 15);
    return seed;
}

inline float RandomFloat(uint seed)
{
    float f = (float)WangHash(seed);
    return f * (1.0f / 4294967295.0f); // 2^32 - 1
}


// Animation curve evaluation
inline float DoEvaluateCurve(float t, Keyframe keyframe0, Keyframe keyframe1)
{
    float dt = keyframe1.time - keyframe0.time;

    float m0 = keyframe0.outTangent * dt;
    float m1 = keyframe1.inTangent * dt;

    float t2 = t * t;
    float t3 = t2 * t;

    float a = 2 * t3 - 3 * t2 + 1;
    float b = t3 - 2 * t2 + t;
    float c = t3 - t2;
    float d = -2 * t3 + 3 * t2;

    return a * keyframe0.value + b * m0 + c * m1 + d * keyframe1.value;
}

inline float EvaluateCurve(FLUVIO_BUFFER(Keyframe) curve, int startIndex, int endIndex, float time)
{
    Keyframe start, end;

    for(int i = startIndex; i < endIndex; ++i)
    {
        start = curve[i];
        end = curve[i+1];

        if (time < end.time)
            break;
    }
    
    float t = invlerp(start.time, end.time, time);
    return DoEvaluateCurve(t, start, end);
}

inline float EvaluateMinMaxCurve(FLUVIO_BUFFER(Keyframe) curve, float time, uint seed)
{
    Keyframe curveInfo = curve[0];

    int maxCurveStartIndex = 1;
    int maxCurveEndIndex = (int)curveInfo.time;
    int minCurveStartIndex = (int)curveInfo.value;
    int minCurveEndIndex = curveInfo.inTangent;

    float scalar = curveInfo.outTangent;

    float maxCurve = EvaluateCurve(curve, maxCurveStartIndex, maxCurveEndIndex, time);
    float minCurve = EvaluateCurve(curve, minCurveStartIndex, minCurveEndIndex, time);

    return scalar * lerp(minCurve, maxCurve, RandomFloat(seed));
}

// Gradient evaluation
inline float4 DoEvaluateGradient(float cT, float aT, GradientColorKey colorKey0, GradientColorKey colorKey1, GradientAlphaKey alphaKey0, GradientAlphaKey alphaKey1)
{
    float3 col0 = FLUVIO_INITIALIZE(float3);
    float3 col1 = FLUVIO_INITIALIZE(float3);

    col0.x = colorKey0.cR;
    col0.y = colorKey0.cG;
    col0.z = colorKey0.cB;

    col1.x = colorKey1.cR;
    col1.y = colorKey1.cG;
    col1.z = colorKey1.cB;

    float4 c = FLUVIO_INITIALIZE(float4);
    c.xyz = lerp3(col0, col1, cT);
    c.w = lerp(alphaKey0.alpha, alphaKey1.alpha, aT);
    return c;
}

inline float4 EvaluateGradient(FLUVIO_BUFFER(GradientColorKey) colorKeys, FLUVIO_BUFFER(GradientAlphaKey) alphaKeys, int colorKeyStartIndex, int colorKeyEndIndex, int alphaKeyStartIndex, int alphaKeyEndIndex, float time)
{
    GradientColorKey startColor, endColor;
    GradientAlphaKey startAlpha, endAlpha;

    for(int i = colorKeyStartIndex; i < colorKeyEndIndex; ++i)
    {
        startColor = colorKeys[i];
        endColor = colorKeys[i+1];

        if (time < endColor.time)
            break;
    }
    
    if (alphaKeyEndIndex - alphaKeyStartIndex == 1)
    {
        startAlpha = alphaKeys[alphaKeyStartIndex];
        endAlpha = alphaKeys[alphaKeyStartIndex+1];
    }
    else
    {
        for(int j = alphaKeyStartIndex; j < alphaKeyEndIndex; ++j)
        {
            startAlpha = alphaKeys[j];
            endAlpha = alphaKeys[j+1];

           if (time < endAlpha.time)
               break;
        }
    }

    float cT = invlerp(startColor.time, endColor.time, time);
    float aT = invlerp(startAlpha.time, endAlpha.time, time);

    return DoEvaluateGradient(cT, aT, startColor, endColor, startAlpha, endAlpha);
}

inline float4 EvaluateMinMaxGradient(FLUVIO_BUFFER(GradientColorKey) colorKeys, FLUVIO_BUFFER(GradientAlphaKey) alphaKeys, float time, uint seed)
{
    GradientColorKey colorInfo = colorKeys[0];
    GradientAlphaKey alphaInfo = alphaKeys[0];

    int maxColorKeyStartIndex = 1;
    int maxColorKeyEndIndex = colorInfo.cR;
    int minColorKeyStartIndex = colorInfo.cG;
    int minColorKeyEndIndex = colorInfo.cB;

    int maxAlphaKeyStartIndex = 1;
    int maxAlphaKeyEndIndex = colorInfo.cA;
    int minAlphaKeyStartIndex = colorInfo.time;
    int minAlphaKeyEndIndex = alphaInfo.alpha;

    float4 maxGradient = EvaluateGradient(colorKeys, alphaKeys, maxColorKeyStartIndex, maxColorKeyEndIndex, maxAlphaKeyStartIndex, maxAlphaKeyEndIndex, time);
    float4 minGradient = EvaluateGradient(colorKeys, alphaKeys, minColorKeyStartIndex, minColorKeyEndIndex, minAlphaKeyStartIndex, minAlphaKeyEndIndex, time);

    return lerp4(minGradient, maxGradient, RandomFloat(seed));
}

// ---------------------------------------------------------------------------------------
// Solver data macros
// ---------------------------------------------------------------------------------------

// The following macros will only work within the main kernel function on OpenCL.

///     Adds the specified force to the particle, using an optional force mode.
///			ForceMode.Force: f0 += f1
///			ForceMode.Acceleration: f0 += f1 * invMass
///			ForceMode.Impulse: f0 += f1 / dt
///			ForceMode.VelocityChange: f0 += (f1 * invMass) / dt
#define solverData_AddForce(particleIndex, forceAmount, forceMode) \
{ \
    float deltaTime = fluvio_Time.x; \
    switch ((forceMode)) \
    { \
        case FLUVIO_FORCE_MODE_FORCE: \
            fluvio_Force[(particleIndex)] += (forceAmount); \
            break; \
        case FLUVIO_FORCE_MODE_ACCELERATION: \
            fluvio_Force[(particleIndex)] += (forceAmount) * (1.0f / fluvio_Fluid[fluvio_FluidID[(particleIndex)]].particleMass); \
            break; \
        case FLUVIO_FORCE_MODE_IMPULSE: \
            fluvio_Force[(particleIndex)] += (forceAmount) / deltaTime; \
            break; \
        case FLUVIO_FORCE_MODE_VELOCITY_CHANGE: \
            fluvio_Force[(particleIndex)] += ((forceAmount) * (1.0f / fluvio_Fluid[fluvio_FluidID[(particleIndex)]].particleMass)) / deltaTime; \
            break; \
    } \
}

///     Gets the color of the particle at the specified index in the solver data.
#define solverData_GetColor(particleIndex) (fluvio_Color[(particleIndex)])

///     Gets the fluid that is associated with the particle at the specified index in the solver data.
#define solverData_GetFluid(particleIndex) (fluvio_Fluid[fluvio_FluidID[(particleIndex)]])

///     Gets the fluid ID that is associated with the particle at the specified index in the solver data.
#define solverData_GetFluidID(particleIndex) (fluvio_FluidID[(particleIndex)])

///     Gets the density of the particle at the specified index in the solver data.
#define solverData_GetDensity(particleIndex) (fluvio_Density[(particleIndex)])

///     Gets the unapplied force of the particle at the specified index in the solver data.
#define solverData_GetForce(particleIndex) (fluvio_Force[(particleIndex)])

///     Gets the mass of the particle at the specified index in the solver data.
#define solverData_GetMass(particleIndex) (fluvio_Fluid[fluvio_FluidID[(particleIndex)]].particleMass)

///     Gets the lifetime of the particle at the specified index in the solver data.
#define solverData_GetLifetime(particleIndex) (fluvio_Lifetime[(particleIndex)])

///     Gets the position of the particle at the specified index in the solver data.
#define solverData_GetPosition(particleIndex) fluvio_Position[(particleIndex)]

///     Gets the pressure of the particle at the specified index in the solver data.
#define solverData_GetPressure(particleIndex) (fluvio_Pressure[(particleIndex)])

///     Gets the random seed of the particle at the specified index in the solver data.
#define solverData_GetRandomSeed(particleIndex) (fluvio_RandomSeed[(particleIndex)])

///     Gets the size of the particle at the specified index in the solver data.
#define solverData_GetSize(particleIndex) (fluvio_Velocity[(particleIndex)].w)

///     Gets the surface normal of the particle at the specified index in the solver data.
#define solverData_GetSurfaceNormal(particleIndex) (fluvio_Normal[(particleIndex)])

///     Gets the surface normal length of the particle at the specified index in the solver data.
#define solverData_GetSurfaceNormalLength(particleIndex) (fluvio_NormalLen[(particleIndex)])

///     Gets the turbulence probability of the particle at the specified index in the solver data.
#define solverData_GetTurbulence(particleIndex) (fluvio_Turbulence[(particleIndex)])

///     Gets the current velocity of the particle at the specified index in the solver data.
#define solverData_GetVelocity(particleIndex) (fluvio_Velocity[(particleIndex)])

///     Gets the vorticity of the particle at the specified index in the solver data.
#define solverData_GetVorticity(particleIndex) (fluvio_Vorticity[(particleIndex)])

///     Sets the color of the particle at the specified index in the solver data.
#define solverData_SetColor(particleIndex, color) (fluvio_Color[(particleIndex)] = (color))

///     Sets the unapplied force of the particle at the specified index in the solver data.
#define solverData_SetForce(particleIndex, force) (fluvio_Force[(particleIndex)] = (force))

///     Sets the lifetime of the particle at the specified index in the solver data.
#define solverData_SetLifetime(particleIndex, lifetime) (fluvio_Lifetime[(particleIndex)] = (lifetime))

///     Sets the size of the particle at the specified index in the solver data.
#define solverData_SetSize(particleIndex, size) (fluvio_Velocity[(particleIndex)].w = (size))

///     Sets the turbulence probability of the particle at the specified index in the solver data.
#define solverData_SetTurbulence(particleIndex, turbulence) (fluvio_Turbulence[(particleIndex)] = (turbulence))

///     Sets the vorticity of the particle at the specified index in the solver data.
#define solverData_SetVorticity(particleIndex, vorticity) (fluvio_Vorticity[(particleIndex)] = (vorticity))

#endif // FLUVIO_COMPUTE_INCLUDED
