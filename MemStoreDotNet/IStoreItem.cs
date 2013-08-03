namespace MemStoreDotNet
{
    using System;

    internal interface IStoreItem
    {
        /// <summary>
        /// Gets or sets the duration of the store.
        /// </summary>
        /// <value>
        /// The duration of the store.
        /// </value>
        int? StoreDuration { get; set; }

        /// <summary>
        /// Gets the last accessed.
        /// </summary>
        /// <value>
        /// The last accessed.
        /// </value>
        DateTime LastAccessed { get; }
    }
}
