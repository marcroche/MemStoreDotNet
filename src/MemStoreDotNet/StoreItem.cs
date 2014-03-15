namespace MemStoreDotNet
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    /// <summary>
    /// A storable object for storage system.
    /// </summary>
    internal class StoreItem<TValue> : IStoreItem
    {
        /// <summary>
        /// Gets or sets the duration of the store.
        /// </summary>
        /// <value>
        /// The duration of the store.
        /// </value>
        public int? StoreDuration { get; set; }

        /// <summary>
        /// Last accessed DateTime.
        /// </summary>
        public DateTime LastAccessed { get; private set; }

        /// <summary>
        /// The store item value.
        /// </summary>
        public TValue Value { get; private set; }

        /// <summary>
        /// Constructs a StoreItem instance.
        /// </summary>
        /// <param name="value"></param>
        public StoreItem(TValue value)
        {
            Value = value;
            LastAccessed = DateTime.UtcNow;
        }
    }
}
