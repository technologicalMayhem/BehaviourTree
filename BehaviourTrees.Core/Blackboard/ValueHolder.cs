using BehaviourTrees.Core.Blackboard.Accessors;

namespace BehaviourTrees.Core.Blackboard
{
    /// <summary>
    ///     Holds the reference to a value on the blackboard alongside accessors to it with different levels of access.
    /// </summary>
    /// <typeparam name="T">The type of the value.</typeparam>
    internal class ValueHolder<T> : IValueHolder where T : class
    {
        /// <summary>
        ///     Creates a new instance of a value holder.
        /// </summary>
        /// <param name="key">The key of the value.</param>
        public ValueHolder(string key)
        {
            Key = key;
            Value = default;

            Get = new Get<T>(this);
            Set = new Set<T>(this);
            GetSet = new GetSet<T>(this);
        }

        /// <summary>
        ///     The value.
        /// </summary>
        public T Value { get; set; }

        /// <summary>
        ///     A accessor to the value with read access.
        /// </summary>
        public IGet<T> Get { get; }

        /// <summary>
        ///     A accessor to the value with write access.
        /// </summary>
        public ISet<T> Set { get; }

        /// <summary>
        ///     A accessor to the value with read and write access.
        /// </summary>
        public IGetSet<T> GetSet { get; }

        /// <inheritdoc />
        public string Key { get; }
    }
}