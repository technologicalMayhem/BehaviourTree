namespace BehaviourTrees.Core.Blackboard
{
    /// <summary>
    ///     Allows behaviour tree nodes to register themselves with variables on the blackboard.
    /// </summary>
    public interface IBlackboardAccessProvider
    {
        /// <summary>
        ///     Provides Read/Write access to a value on the blackboard.
        /// </summary>
        /// <param name="key">The key of the value.</param>
        /// <typeparam name="T">The type of the value.</typeparam>
        /// <returns>A accessor to the value allowing both read and write.</returns>
        IGetSet<T> ProvideGetSet<T>(string key);

        /// <summary>
        ///     Provides read access to a value on the blackboard.
        /// </summary>
        /// <param name="key">The key of the value.</param>
        /// <typeparam name="T">The type of the value.</typeparam>
        /// <returns>A accessor to the value allowing only read access.</returns>
        IGet<T> ProvideGet<T>(string key);

        /// <summary>
        ///     Provides write access to a value on the blackboard.
        /// </summary>
        /// <param name="key">The key of the value.</param>
        /// <typeparam name="T">The type of the value.</typeparam>
        /// <returns>A accessor to the value allowing only write access.</returns>
        ISet<T> ProvideSet<T>(string key);
    }

    /// <summary>
    ///     Allows read/write access to a variable on the blackboard.
    /// </summary>
    /// <typeparam name="T">The type of the variable.</typeparam>
    public interface IGetSet<T>
    {
        /// <summary>
        ///     The value of the blackboard variable.
        /// </summary>
        public T Value { get; set; }
    }

    /// <summary>
    ///     Allows read access to a variable on the blackboard.
    /// </summary>
    /// <typeparam name="T">The type of the variable.</typeparam>
    public interface IGet<out T>
    {
        /// <summary>
        ///     The value of the blackboard variable.
        /// </summary>
        public T Value { get; }
    }

    /// <summary>
    ///     Allows write access to a variable on the blackboard.
    /// </summary>
    /// <typeparam name="T">The type of the variable.</typeparam>
    public interface ISet<in T>
    {
        /// <summary>
        ///     The value of the blackboard variable.
        /// </summary>
        public T Value { set; }
    }
}