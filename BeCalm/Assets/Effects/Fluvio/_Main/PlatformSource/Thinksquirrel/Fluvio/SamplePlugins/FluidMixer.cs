// FluidMixer.cs
// Copyright (c) 2011-2014 Thinksquirrel Software, LLC.

using System;
using Thinksquirrel.Fluvio.Plugins;
using UnityEngine;

namespace Thinksquirrel.Fluvio.SamplePlugins
{
    /// <summary>
    ///     This is an advanced sample plugin that provides simple, non-physical mixing for different fluids.
    /// </summary>
    /// <remarks>
    ///     Three mixing configurations are supported:
    /// <list type="bullet">
    /// <item><description>Fluid A + Fluid B -> Fluid C + Fluid D</description></item>
    /// <item><description>Fluid A + Fluid B -> Fluid A + Fluid D (if Fluid C is unassigned)</description></item>
    /// <item><description>Fluid A + Fluid B -> Fluid C + Fluid C (if Fluid D is unassigned)</description></item>
    /// </list>
    /// </remarks>
    [AddComponentMenu("Fluvio/Example Project/Sample Plugins/Fluid Mixer")]
    public class FluidMixer : FluidParticlePairPlugin
    {
        /// <summary>
        ///     The second fluid to mix with. This must be in the same fluid group as the first fluid.
        /// </summary>
        public Fluid fluidB;
        /// <summary>
        ///     A particle system belonging to the optional mixed fluid to output, at Fluid A's position (and also Fluid B's position, if Fluid D is not assigned). If unassigned, Fluid A will not be destroyed.
        /// </summary>
        /// <remarks>
        ///     This particle system does not have to be a fluid.
        /// </remarks>
        public ParticleSystem fluidC;
        /// <summary>
        ///     An particle system belonging to the optional second mixed fluid to output, at Fluid B's position.
        /// </summary>
        /// <remarks>
        ///     This particle system does not have to be a fluid.
        /// </remarks>
        public ParticleSystem fluidD;
        /// <summary>
        ///     The multiplier applied to the smoothing distance that determines if two neighboring particles are close enough to mix.
        /// </summary>
        public float distanceMultiplier;

        int[] m_ParticleSystems;
        Vector4[] m_EmitPositions;
        Vector4[] m_EmitVelocities;
        int m_Count;
        int m_OldCount;
        int m_FluidAID;
        int m_FluidBID;
        int m_FluidCID;
        int m_FluidDID;
        bool m_FirstFrame;
        FluvioTimeStep m_TimeStep;
        Vector3 m_Gravity;
        float m_SimulationScale;
        bool m_MixingFluidsAreTheSame;
        float m_MixingDistanceSq;
     
        protected override void OnResetPlugin()
        {
            fluidB = null;
            fluidC = null;
            fluidD = null;
            distanceMultiplier = 0.5f;
        }
        protected override void OnEnablePlugin()
        {
            m_FirstFrame = true;

            SetComputeShader(FluvioComputeShader.Find("ComputeShaders/SamplePlugins/FluidMixer"), "OnUpdatePlugin");
        }
        protected override void OnSetComputeShaderVariables()
        {
            SetComputePluginValue(0, m_FluidBID);
            SetComputePluginValue(1, m_FluidCID);
            SetComputePluginValue(2, m_FluidDID);
            SetComputePluginValue(3, m_MixingFluidsAreTheSame ? 1 : 0);
            SetComputePluginValue(4, m_MixingDistanceSq);
            SetComputePluginBuffer(5, m_ParticleSystems, true);
            SetComputePluginBuffer(6, m_EmitPositions, true);
            SetComputePluginBuffer(7, m_EmitVelocities, true);
        }    
        protected override bool OnStartPluginFrame(ref FluvioTimeStep timeStep)
        {
            // Always process the full fluid group - this plugin requires sub-fluids to work properly.
            includeFluidGroup = true;

            // Skip the first frame
            if (m_FirstFrame)
            {
                m_FirstFrame = false;
                return false;
            }

            if (!fluidB || !fluidB.enabled)
                return false;

            // Create arrays
            var oldCount = m_Count;
            m_Count = fluid.GetTotalParticleCount();
            Array.Resize(ref m_ParticleSystems, m_Count);
            Array.Resize(ref m_EmitPositions, m_Count);
            Array.Resize(ref m_EmitVelocities, m_Count);

            for (var i = oldCount; i < m_Count; ++i)
            {
                m_ParticleSystems[i] = 0;
            }

            // Get timestep
            m_TimeStep = timeStep;
            // Get gravity
            m_Gravity = fluid.gravity;
            // Get simulation scale
            m_SimulationScale = fluid.simulationScale;
            // Set flag indicating whether or not the mixing fluids are the same
            m_MixingFluidsAreTheSame = fluid == fluidB;
            // Get squared mixing distance
            m_MixingDistanceSq = fluid.smoothingDistance*fluid.smoothingDistance*distanceMultiplier*distanceMultiplier;
            // Get fluid IDs
            m_FluidAID = fluid.GetFluidID();
            m_FluidBID = fluidB.GetFluidID();
            m_FluidCID = fluidC ? 1 : 0;
            m_FluidDID = fluidD ? 2 : 0;

            // Fluid B and (C or D) at least must be enabled
            return fluidC || fluidD;
        }
        protected override void OnUpdatePlugin(SolverData solverData, int particleIndex, int neighborIndex)
        {
            // Get fluid IDs
            var particleFluid = solverData.GetFluid(particleIndex).GetFluidID();
            var neighborFluid = solverData.GetFluid(neighborIndex).GetFluidID();

            var particleIsA = particleFluid == m_FluidAID;

            if ((m_MixingFluidsAreTheSame || particleFluid !=  neighborFluid) && // Fluids are not the same, unless A and B are the same
                (particleIsA || particleFluid == m_FluidBID) && // First fluid is A or B
                (neighborFluid == m_FluidAID || neighborFluid == m_FluidBID)) // Second fluid is A or B
            {
                var position = solverData.GetPosition(particleIndex);
                var neighborPosition = solverData.GetPosition(neighborIndex);
                
                // Make sure the particles are actually close
                if ((position - neighborPosition).sqrMagnitude > m_MixingDistanceSq)
                    return;

                var velocity = solverData.GetVelocity(particleIndex);
                var invMass = 1.0f/solverData.GetMass(particleIndex);

                var emitC = false;
                
                // Despawn the current particle, unless Fluid C is null and the particle is from Fluid A
                if (m_FluidCID != 0 || !particleIsA)
                {
                    emitC = true;
                    solverData.SetLifetime(particleIndex, -1);
                }

                // Emit a particle. We don't emit from the neighbor position, that gets handled in the opposite pair
                // We handle actual emission on the main thread.
                if (m_FluidDID == 0 || particleIsA)
                {
                    if (emitC)
                    {
                        // Set the system to Fluid C
                        m_ParticleSystems[particleIndex] = 1;
                    }
                    else
                    {
                        m_ParticleSystems[particleIndex] = 0;
                    }
                }
                else
                {
                    // Set the system to Fluid D
                    m_ParticleSystems[particleIndex] = 2;
                }

                // Set emit position and velocity. These must be manually integrated since they won't be applied until after the solver has run.
                // The below is a standard Euler explicit integrator.
                var acceleration = m_Gravity + solverData.GetForce(particleIndex)*invMass;

                for (var iter = 0; iter < m_TimeStep.solverIterations; ++iter)
                {
                    var t = (m_TimeStep.dtIter)*acceleration;

                    // Ignore very large velocity changes
                    if (Vector3.Dot(t, t) > (FluvioSettings.kMaxSqrVelocityChange * m_SimulationScale))
                    {
                       t *= 0;
                    }

                    velocity += t;
                }

                m_EmitVelocities[particleIndex] = velocity;
                m_EmitPositions[particleIndex] = position + velocity * m_TimeStep.deltaTime;
            }
        }
        protected override void OnReadComputeBuffers()
        {
            GetComputePluginBuffer(5, m_ParticleSystems);
            GetComputePluginBuffer(6, m_EmitPositions);
            GetComputePluginBuffer(7, m_EmitVelocities);
        }
        protected override void OnPluginPostSolve()
        {
            // We do actual emission after the solver has run
            for (var particleIndex = 0; particleIndex < m_Count; ++particleIndex)
            {
                var systemID = m_ParticleSystems[particleIndex];
                if (systemID == 0) continue;

                var system = systemID == 1 ? fluidC : fluidD;

                m_ParticleSystems[particleIndex] = 0;

                var toSystemSpace = system.simulationSpace == ParticleSystemSimulationSpace.World
                    ? Matrix4x4.identity
                    : Matrix4x4.TRS(system.transform.position, system.transform.rotation, Vector3.one).inverse;

                // Emit from particle systems
                system.Emit(
                    toSystemSpace.MultiplyPoint3x4(m_EmitPositions[particleIndex]), 
                    toSystemSpace.MultiplyVector(m_EmitVelocities[particleIndex]), 
                    system.startSize, 
                    system.startLifetime,
                    system.startColor);
            }
        }
    }
}
