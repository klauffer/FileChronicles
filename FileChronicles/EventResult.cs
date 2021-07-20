using System;

namespace FileChronicles
{
    /// <summary>
    /// Results of registering an event with the chronicle via chonicler
    /// </summary>
    public abstract class EventResult<SuccessType, ErrorType>
    {
        /// <summary>
        /// keep external classes from inheriting this
        /// </summary>
        private EventResult() { }

        /// <summary>
        /// Exhaustively match on the <see cref="EventResult{SuccessType, ErrorType}"/>.
        /// </summary>
        /// <typeparam name="T">The type to unify all cases of the <see cref="EventResult{SuccessType, ErrorType}"/> to.</typeparam>
        /// <param name="success">What to do if the <see cref="EventResult{SuccessType, ErrorType}"/> was a success.</param>
        /// <param name="error">What to do if the <see cref="EventResult{SuccessType, ErrorType}"/> was a error.</param>
        /// <returns>The result of handling each case.</returns>
        public abstract T Match<T>(Func<SuccessType, T> success, Func<ErrorType, T> error);

        /// <summary>
        /// a Successful Event
        /// </summary>
        public sealed class Success : EventResult<SuccessType, ErrorType>
        {
            private readonly SuccessType _successInfo;

            /// <summary>
            /// Require Additional information for a Success State
            /// </summary>
            /// <param name="successInfo"></param>
            public Success(SuccessType successInfo)
            {
                _successInfo = successInfo;
            }

            /// <inheritdoc/>
            public override T Match<T>(Func<SuccessType, T> success, Func<ErrorType, T> error) => success(_successInfo);
        }

        /// <summary>
        /// a errored Event
        /// </summary>
        public sealed class Error : EventResult<SuccessType, ErrorType>
        {
            /// <summary>
            /// Errored additional information
            /// </summary>
            public ErrorType _errorInfo;

            /// <summary>
            /// require additional information with an errored state
            /// </summary>
            /// <param name="errorInfo"></param>
            public Error(ErrorType errorInfo)
            {
                _errorInfo = errorInfo;
            }
            /// <inheritdoc/>
            public override T Match<T>(Func<SuccessType, T> success, Func<ErrorType, T> error) => error(_errorInfo);
        }
    }
}
