namespace BehaviourTrees.Core.Blackboard.Accessors
{
    internal class Get<T> : IGet<T>
    {
        private readonly ValueHolder<T> _valueHolder;

        public Get(ValueHolder<T> valueHolder)
        {
            _valueHolder = valueHolder;
        }

        public T Value => _valueHolder.Value;
    }
}