namespace MemStoreDotNet
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Threading;

    /// <summary>
    /// Cleans the store based on a time policy.
    /// </summary>
    internal class StorePruner
    {
        private static Timer timer;
        private static object lockObject = new object();
        private static Dictionary<Type, IDictionary> _store;

        /// <summary>
        /// Initializes the specified store.
        /// </summary>
        /// <param name="store">The store.</param>
        internal static void Initialize(Dictionary<Type, IDictionary> store)
        {
            _store = store;
            timer = new Timer(PruneExpiredItems, null, 0, 60000);
        }

        /// <summary>
        /// Prunes the expired items.
        /// </summary>
        /// <param name="state">The state.</param>
        internal static void PruneExpiredItems(object state)
        {
            if (Monitor.TryEnter(lockObject))
            {
                try
                {
                    foreach (KeyValuePair<Type, IDictionary> kvp in _store)
                    {
                        foreach (KeyValuePair<Type, IDictionary> item in (Dictionary<Type, IDictionary>)kvp.Value)
                        {
                            List<object> list = new List<object>();
                            foreach (DictionaryEntry innerItem in item.Value)
                            {
                                IStoreItem storeItem = innerItem.Value as IStoreItem;

                                if (storeItem != null && storeItem.StoreDuration != null)
                                {
                                    if (storeItem.LastAccessed.AddMinutes(storeItem.StoreDuration.Value) < DateTime.UtcNow)
                                    {
                                        list.Add(innerItem.Key);
                                    }
                                }
                            }

                            list.ForEach(x => ((IDictionary)item.Value).Remove(x));
                        }
                    }
                }
                finally
                {
                    Monitor.Exit(lockObject);
                }
            }
        }
    }
}
