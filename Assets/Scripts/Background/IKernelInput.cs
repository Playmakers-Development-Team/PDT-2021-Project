namespace Background
{
    /// <summary>
    /// Interface defining the required functionality for structs that can be passed from a <see cref="Feature"/> into a compute buffer.
    /// </summary>
    public interface IKernelInput
    {
        /// <summary>
        /// Get the name of the buffer parameter in the shader.
        /// </summary>
        /// <returns>The name of the buffer parameter in the shader.</returns>
        string GetName();
        /// <summary>
        /// Get the size of the IKernelInput struct.
        /// </summary>
        /// <returns>The total size of the member variables, in bytes.</returns>
        int GetSize();
    }
}
