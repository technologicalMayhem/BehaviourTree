namespace BehaviourTrees.Core.Blackboard
{
    /// <summary>
    ///     A interface for the <see cref="ValueHolder{T}" />. The purpose of this interface is to allow
    ///     <see cref="BehaviourTreeBlackboard" />
    ///     to have a list of typed ValueHolder's.
    /// </summary>
    internal interface IValueHolder
    {
        /// <summary>
        ///     The key the value can be referred to.
        /// </summary>
        string Key { get; }
    }
}