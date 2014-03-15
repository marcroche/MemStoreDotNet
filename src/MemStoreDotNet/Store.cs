namespace MemStoreDotNet
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading;

    /// <summary>
    /// Genericized key/value storage system.
    /// </summary>
    public class Store
    {
        private static readonly Store _store = new Store();
        private static readonly Dictionary<Type, IDictionary> _dictionary = new Dictionary<Type, IDictionary>();

        /// <summary>
        /// Initializes the <see cref="Store" /> class.
        /// </summary>
        static Store()
        {
            StorePruner.Initialize(_dictionary);
        }

        /// <summary>
        /// Prevents a default instance of the <see cref="Store" /> class from being created.
        /// </summary>
        private Store() { }

        /// <summary>
        /// Instance of the store.
        /// </summary>
        public static Store Instance
        {
            get { return _store; }
        }

        /// <summary>
        /// Adds an item to the store. A null storeDuration means the item will stay in the store without expiring.
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="nonExpirable"></param>
        public void AddItem<TKey, TValue>(TKey key, TValue value, int? storeDuration = null)
        {
            StoreItem<TValue> item = new StoreItem<TValue>(value)
            {
                StoreDuration = storeDuration
            };

            AddStoreItem(key, item);
        }

        /// <summary>
        /// Adds the store item.
        /// </summary>
        /// <typeparam name="TKey">The type of the key.</typeparam>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        private void AddStoreItem<TKey, TValue>(TKey key, TValue value)
        {
            if (!_dictionary.ContainsKey(typeof(TKey)))
            {
                _dictionary.Add(typeof(TKey), new Dictionary<Type, IDictionary>());
            }

            if (!((Dictionary<Type, IDictionary>)_dictionary[typeof(TKey)]).ContainsKey(typeof(TValue)))
            {
                ((Dictionary<Type, IDictionary>)_dictionary[typeof(TKey)]).Add(typeof(TValue),
                    new Dictionary<TKey, TValue>());
            }

            ((Dictionary<TKey, TValue>)(_dictionary[typeof(TKey)][typeof(TValue)]))[key] = value;
        }

        /// <summary>
        /// Tests for the existence of a give key.
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool ContainsKey<TKey, TValue>(TKey key)
        {
            var dict = ((Dictionary<TKey, StoreItem<TValue>>)
                (_dictionary[typeof(TKey)][typeof(StoreItem<TValue>)]));

            if (dict.Keys.Contains(key))
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Gets an item from the store.
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        public TValue GetItem<TKey, TValue>(TKey key)
        {
            return ((Dictionary<TKey, StoreItem<TValue>>)
                (_dictionary[typeof(TKey)][typeof(StoreItem<TValue>)]))[key].Value;
        }

        /// <summary>
        /// Removes an item from the store.
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="key"></param>
        public void RemoveItem<TKey, TValue>(TKey key)
        {
            ((Dictionary<TKey, StoreItem<TValue>>)
                (_dictionary[typeof(TKey)][typeof(StoreItem<TValue>)])).Remove(key);
        }

        /// <summary>
        /// Tries to get the value from the collection. Returns true if successful, false if not.
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool TryGetValue<TKey, TValue>(TKey key, out TValue value)
        {
            if (!_dictionary.ContainsKey(typeof(TKey)))
            {
                value = default(TValue);
                return false;
            }

            if (!((Dictionary<Type, IDictionary>)
                _dictionary[typeof(TKey)]).ContainsKey(typeof(StoreItem<TValue>)))
            {
                value = default(TValue);
                return false;
            }

            if (!((Dictionary<TKey, StoreItem<TValue>>)
                (_dictionary[typeof(TKey)][typeof(StoreItem<TValue>)])).ContainsKey(key))
            {
                value = default(TValue);
                return false;
            }

            value = ((Dictionary<TKey, StoreItem<TValue>>)
                (_dictionary[typeof(TKey)][typeof(StoreItem<TValue>)]))[key].Value;

            return true;
        }
    }
}
