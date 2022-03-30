namespace BehaviourTrees.Core.Blackboard.Accessors
{
    internal class GetSet<T> : IGetSet<T>
    {
        private readonly ValueHolder<T> _valueHolder;

        public GetSet(ValueHolder<T> valueHolder)
        {
            _valueHolder = valueHolder;
        }

        public T Value
        {
            get => _valueHolder.Value;
            set => _valueHolder.Value = value;
        }
    }
}