namespace SqlBrokerToAzureAdapter.Transformations
{
    /// <summary>
    /// The differences of two objects.
    /// </summary>
    public sealed class Difference
    {
        /// <summary>
        /// Creates a new instance of a Difference
        /// </summary>
        /// <param name="memberPath">The path of the member of the difference.</param>
        /// <param name="oldValue">The old value of the difference.</param>
        /// <param name="newValue">The new value of the difference.</param>
        public Difference(string memberPath, string oldValue, string newValue)
        {
            MemberPath = memberPath;
            OldValue = oldValue;
            NewValue = newValue;
        }

        /// <summary>
        /// The member path of a difference.
        /// </summary>
        public string MemberPath { get; }

        /// <summary>
        /// The old value.
        /// </summary>
        public string OldValue { get; }

        /// <summary>
        /// The new value.
        /// </summary>
        public string NewValue { get; }
    }
}