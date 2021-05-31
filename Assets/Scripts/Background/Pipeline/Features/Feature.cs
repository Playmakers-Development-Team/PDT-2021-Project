using System;
using Managers;
using UnityEngine;

namespace Background.Pipeline.Features
{
    /// <summary>
    /// Base class for creating scriptable background renderer features.
    /// </summary>
    [Serializable]
    public abstract class Feature : ScriptableObject
    {
        [SerializeField, HideInInspector] private bool active = true;

        [SerializeField, HideInInspector] private string featureName;
        

        /// <summary>
        /// The number of threads used to dispatch a compute shader kernel. Must match the kernel corresponding to the feature.
        /// </summary>
        protected virtual Vector3Int Threads => new Vector3Int(8, 8, 1);
        /// <summary>
        /// <c>true</c> if the feature is enabled in its <see cref="Pipeline"/>.
        /// </summary>
        public bool IsActive => active;
        /// <summary>
        /// The compute shader used for rendering the background.
        /// </summary>
        public ComputeShader Shader => Settings.BackgroundCompute;
        
        
        /// <summary>
        /// Override to define what a feature does when executed as part of a <see cref="Pipeline"/>.
        /// </summary>
        public abstract void Execute();

        /// <summary>
        /// Override to return the correct compute kernel index for this feature.
        /// </summary>
        /// <returns>The index of the corresponding kernel in the background shader.</returns>
        protected abstract int GetKernelIndex();

        /// <summary>
        /// Send an <see cref="IKernelInput"/> struct as a buffer into the background shader, ensuring it is registered properly with the <see cref="BackgroundManager"/>.
        /// </summary>
        /// <param name="input">The input struct to send into a compute buffer.</param>
        /// <typeparam name="T">The input type.</typeparam>
        /// <returns>The compute buffer created.</returns>
        protected ComputeBuffer SetInput<T>(T input) where T : IKernelInput
        {
            ComputeBuffer buffer = new ComputeBuffer(1, input.GetSize());
            buffer.SetData(new[] {input});

            Shader.SetBuffer(GetKernelIndex(), input.GetName(), buffer);

            BackgroundManager.MarkToRelease(buffer);
            
            return buffer;
        }

        /// <summary>
        /// Send a texture to the background shader.
        /// </summary>
        /// <param name="textureName">The texture's name in the background shader.</param>
        /// <param name="texture">The texture to send into the background shader.</param>
        protected void SetTexture(string textureName, Texture texture)
        {
            Shader.SetTexture(GetKernelIndex(), textureName, texture);
        }

        /// <summary>
        /// Dispatch the shader kernel corresponding to the feature.
        /// </summary>
        /// <param name="xResolution">The size of the writable data in the X dimension.</param>
        /// <param name="yResolution">The size of the writable data in the Y dimension.</param>
        /// <param name="zResolution">The size of the writable data in the Z dimension. Default is <c>1</c>.</param>
        protected void Dispatch(int xResolution, int yResolution, int zResolution = 1)
        {
            Shader.Dispatch(GetKernelIndex(), xResolution / Threads.x, yResolution / Threads.y, zResolution / Threads.z);
        }
    }
}
