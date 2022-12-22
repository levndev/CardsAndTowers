using System;
using System.Collections.Generic;
using System.Linq;

public static class LinqExtensions
{
    /// <summary>
    /// Choose a weighted random element from the sequence.
    /// </summary>
    /// <typeparam name="TSource">The type of the elements of source.</typeparam>
    /// <param name="source">A sequence of elements to choose at random from.</param>
    /// <param name="selector">A function to calculate weights for each element.</param>
    /// <returns>A random element from the sequence.</returns>
    public static TSource ChooseRandom<TSource>(this IEnumerable<TSource> source, Func<TSource, float> selector)
    {
        var values = source.Select(value => (Value: value, Weight: selector(value)));
        var totalWeigth = values.Sum(value => value.Weight);
        var random = UnityEngine.Random.value;
        var totalProbability = 0f;

        foreach (var (value, weight) in values)
        {
            var probability = weight / totalWeigth;
            if (totalProbability < random && random < totalProbability + probability)
            {
                return value;
            }
            totalProbability += probability;
        }
        if (source.Any())
            return source.Last();
        else
            return default;
    }
}
