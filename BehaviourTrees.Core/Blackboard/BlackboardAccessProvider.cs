namespace BehaviourTrees.Core.Blackboard
{
    /// <summary>
    ///     Provides access to blackboard variables by handing out accessors wrapping the value.
    /// </summary>
    public class BlackboardAccessProvider : IBlackboardAccessProvider
    {
        private readonly BehaviourTreeBlackboard _behaviourTreeBlackboard;
        private readonly object _object;

        internal BlackboardAccessProvider(object o, BehaviourTreeBlackboard behaviourTreeBlackboard)
        {
            _object = o;
            _behaviourTreeBlackboard = behaviourTreeBlackboard;
        }

        /// <inheritdoc />
        public IGetSet<T> ProvideGetSet<T>(string key) where T : class
        {
            return _behaviourTreeBlackboard.GetBlackboardAccessor<T>
                (_object, key, BlackboardAccess.Read | BlackboardAccess.Write) as IGetSet<T>;
        }

        /// <inheritdoc />
        public IGet<T> ProvideGet<T>(string key) where T : class
        {
            return _behaviourTreeBlackboard.GetBlackboardAccessor<T>
                (_object, key, BlackboardAccess.Read) as IGet<T>;
        }

        /// <inheritdoc />
        public ISet<T> ProvideSet<T>(string key) where T : class
        {
            return _behaviourTreeBlackboard.GetBlackboardAccessor<T>
                (_object, key, BlackboardAccess.Write) as ISet<T>;
        }
    }
}