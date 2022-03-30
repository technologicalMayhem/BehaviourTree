using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using BehaviourTrees.Core.Blackboard.Accessors;

namespace BehaviourTrees.Core.Blackboard
{
    /// <summary>
    ///     Holds values for use in the behaviour tree and keeps track of what objects are using the data.
    /// </summary>
    public class BehaviourTreeBlackboard
    {
        /// <summary>
        ///     List of the values in the blackboard and their accessors.
        /// </summary>
        private readonly List<IValueHolder> _blackboardValues;

        //Todo: Consider replacing this with a different implementation as nested dictionaries are awkward to use
        /// <summary>
        ///     A nested dictionary containing information about which objects have access to what blackboard values.
        /// </summary>
        private readonly Dictionary<object, Dictionary<string, BlackboardAccess>> _registrationData;

        public BehaviourTreeBlackboard()
        {
            _blackboardValues = new List<IValueHolder>();
            _registrationData = new Dictionary<object, Dictionary<string, BlackboardAccess>>();
            Registrations = new ReadOnlyDictionary<object, Dictionary<string, BlackboardAccess>>(_registrationData);
        }

        /// <inheritdoc cref="_registrationData" />
        public IReadOnlyDictionary<object, Dictionary<string, BlackboardAccess>> Registrations { get; }

        /// <summary>
        ///     Returns a <see cref="IBlackboardAccessProvider" /> for the object. Please consider using
        ///     <see cref="ProvideBlackboardAccess" /> instead as using the IBlackboardAccessProvider should only
        ///     be used for testing or debugging purposes.
        /// </summary>
        /// <param name="registrar">The object the <see cref="IBlackboardAccessProvider" /> should be for.</param>
        /// <returns>A <see cref="IBlackboardAccessProvider" /> instance.</returns>
        public IBlackboardAccessProvider GetProviderForObject(object registrar)
        {
            return new BlackboardAccessProvider(registrar, this);
        }

        /// <summary>
        ///     Provides access to the blackboard to the object.
        /// </summary>
        /// <param name="registrar">The object that should be given access to.</param>
        public void ProvideBlackboardAccess(IRegistersToBlackboard registrar)
        {
            registrar.RegisterBlackboardAccess(GetProviderForObject(registrar));
        }

        /// <summary>
        ///     Obtains an accessor to a blackboard key with a defined access scope.
        /// </summary>
        /// <param name="requester">The object requesting an accessor.</param>
        /// <param name="key">The key of the value the accessor should refer to.</param>
        /// <param name="desiredBlackboardAccess">The type of access the accessor should permit.</param>
        /// <typeparam name="T">The type of the value.</typeparam>
        /// <returns>
        ///     An accessor with the requested access rights. This will be either a <see cref="Get{T}" />,
        ///     a <see cref="Set{T}" /> or a <see cref="GetSet{T}" />.
        /// </returns>
        /// <exception cref="ArgumentOutOfRangeException">
        ///     Gets thrown when the <paramref name="desiredBlackboardAccess" /> has
        ///     invalid flags set.
        /// </exception>
        /// <exception cref="ArgumentException">
        ///     Gets thrown when the <paramref name="key" /> already exists and is of
        ///     different type to <typeparamref name="T" />.
        /// </exception>
        internal object GetBlackboardAccessor<T>(object requester, string key,
            BlackboardAccess desiredBlackboardAccess) where T : class
        {
            if (_registrationData.ContainsKey(requester) && _registrationData[requester].ContainsKey(key))
                _registrationData[requester][key] |= desiredBlackboardAccess;
            else
                _registrationData[requester] = new Dictionary<string, BlackboardAccess>
                {
                    [key] = desiredBlackboardAccess
                };

            var valueHolder = _blackboardValues.FirstOrDefault(holder => holder.Key == key);

            if (valueHolder == null)
            {
                var holder = new ValueHolder<T>(key);
                _blackboardValues.Add(holder);
                valueHolder = holder;
            }

            if (valueHolder is ValueHolder<T> typedHolder)
                return desiredBlackboardAccess switch
                {
                    BlackboardAccess.Read | BlackboardAccess.Write => typedHolder.GetSet,
                    BlackboardAccess.Read => typedHolder.Get,
                    BlackboardAccess.Write => typedHolder.Set,
                    _ => throw
                        new ArgumentOutOfRangeException(nameof(desiredBlackboardAccess), desiredBlackboardAccess,
                            $"An invalid {nameof(BlackboardAccess)} has been passed in.")
                };

            var generic = valueHolder.GetType().GetGenericArguments()[0];
            throw new ArgumentException(
                $"Key {key} already exists but is of different type. The type expected is {typeof(T)} but the existing one is {generic}");
        }
    }
}