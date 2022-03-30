namespace BehaviourTrees.Core.Blackboard.Accessors
{
    internal class Set<T> : ISet<T> where T : class
    {
        private readonly ValueHolder<T> _valueHolder;

        public Set(ValueHolder<T> valueHolder)
        {
            _valueHolder = valueHolder;
        }

        public T Value
        {
            set => _valueHolder.Value = value;
        }
    }
}