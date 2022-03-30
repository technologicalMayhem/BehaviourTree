namespace BehaviourTrees.Core.Blackboard
{
    public interface IBlackboardAccessProvider
    {
        /// <summary>
        ///     Provides Read/Write access to a value on the blackboard.
        /// </summary>
        /// <param name="key">The key of the value.</param>
        /// <typeparam name="T">The type of the value.</typeparam>
        /// <returns>A accessor to the value allowing both read and write.</returns>
        IGetSet<T> ProvideGetSet<T>(string key) where T : class;

        /// <summary>
        ///     Provides read access to a value on the blackboard.
        /// </summary>
        /// <param name="key">The key of the value.</param>
        /// <typeparam name="T">The type of the value.</typeparam>
        /// <returns>A accessor to the value allowing only read access.</returns>
        IGet<T> ProvideGet<T>(string key) where T : class;

        /// <summary>
        ///     Provides write access to a value on the blackboard.
        /// </summary>
        /// <param name="key">The key of the value.</param>
        /// <typeparam name="T">The type of the value.</typeparam>
        /// <returns>A accessor to the value allowing only write access.</returns>
        ISet<T> ProvideSet<T>(string key) where T : class;
    }

    public interface IGetSet<T>
    {
        public T Value { get; set; }
    }

    public interface IGet<out T>
    {
        public T Value { get; }
    }

    public interface ISet<in T>
    {
        public T Value { set; }
    }
}