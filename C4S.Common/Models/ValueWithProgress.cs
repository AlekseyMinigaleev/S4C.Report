namespace C4S.Common.Models
{
    /// <summary>
    /// модель значения с прогрессом для сравнения.
    /// </summary>
    /// <typeparam name="T">Тип значения.</typeparam>
    public class ValueWithProgress<T> : IComparable<ValueWithProgress<T>>
    {
        /// <summary>
        /// Актуальное значение
        /// </summary>
        public T ActualValue { get; set; }

        /// <summary>
        /// Последнее добавленное значение к актуальному значению
        /// </summary>
        public T LastProgressValue { get; set; }

        /// <param name="actualValue">Актуальное значение.</param>
        /// <param name="lastProgressValue">Последнее добавленное значение к актуальному значению.</param>
        public ValueWithProgress(T actualValue, T lastProgressValue)
        {
            ActualValue = actualValue;
            LastProgressValue = lastProgressValue;
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public int CompareTo(ValueWithProgress<T>? obj)
        {
            if (obj is null)
                return 1;

            return Comparer<T>.Default.Compare(ActualValue, obj.ActualValue);
        }
    }
}