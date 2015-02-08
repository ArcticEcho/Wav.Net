namespace WavDotNet.Core
{
    public enum IoPriority
    {
        /// <summary>
        /// Priorities memory usage over IO speed (lower RAM, lower read/write speed).
        /// </summary>
        Memory,

        /// <summary>
        /// Priorities IO speed over memory usage (higher RAM, higher read/write speed).
        /// </summary>
        Speed
    }
}
