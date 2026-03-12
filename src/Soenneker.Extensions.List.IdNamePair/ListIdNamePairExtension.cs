using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using Soenneker.Extensions.String;

namespace Soenneker.Extensions.List.IdNamePair;

/// <summary>
/// A collection of helpful <see cref="IList{T}"/> extension methods for working with lists of
/// <see cref="Dtos.IdNamePair.IdNamePair"/> objects.
/// </summary>
public static class ListIdNamePairExtension
{
    private const StringComparison _ord = StringComparison.Ordinal;

    /// <summary>
    /// Checks whether the list contains an object with the specified Id.
    /// </summary>
    /// <typeparam name="T">A type derived from <see cref="Dtos.IdNamePair.IdNamePair"/>.</typeparam>
    /// <param name="value">The list to search. If null or empty, returns false.</param>
    /// <param name="id">The Id to search for. If null or empty, returns false.</param>
    /// <returns><c>true</c> if any element's <c>Id</c> equals <paramref name="id"/> using ordinal comparison; otherwise <c>false</c>.</returns>
    /// <remarks>This method performs a linear search.</remarks>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool ContainsId<T>(this IList<T>? value, string? id) where T : Dtos.IdNamePair.IdNamePair
    {
        if (value is null)
            return false;

        int count = value.Count;
        if (count == 0)
            return false;

        if (id.IsNullOrEmpty())
            return false;

        for (int i = 0; i < count; i++)
        {
            if (string.Equals(value[i].Id, id, _ord))
                return true;
        }

        return false;
    }

    /// <summary>
    /// Converts the list to a new list containing all Id values.
    /// </summary>
    /// <typeparam name="T">A type derived from <see cref="Dtos.IdNamePair.IdNamePair"/>.</typeparam>
    /// <param name="value">The source list. If null or empty, returns an empty list.</param>
    /// <returns>A new list containing the Ids in the same order as the source.</returns>
    [Pure]
    public static List<string> ToListOfIds<T>(this IList<T>? value) where T : Dtos.IdNamePair.IdNamePair
    {
        if (value is null)
            return [];

        int count = value.Count;
        if (count == 0)
            return [];

        var ids = new List<string>(count);

        for (int i = 0; i < count; i++)
            ids.Add(value[i].Id);

        return ids;
    }

    /// <summary>
    /// Converts the list to a new list containing Document Ids derived from each element's Id.
    /// </summary>
    /// <typeparam name="T">A type derived from <see cref="Dtos.IdNamePair.IdNamePair"/>.</typeparam>
    /// <param name="value">The source list. If null or empty, returns an empty list.</param>
    /// <returns>A new list containing Document Ids in the same order as the source.</returns>
    /// <remarks>
    /// This method assumes each Id can be split into a Document Id using <see cref="ToSplitId"/>.
    /// </remarks>
    [Pure]
    public static List<string> ToListOfDocumentIds<T>(this IList<T>? value) where T : Dtos.IdNamePair.IdNamePair
    {
        if (value is null)
            return [];

        int count = value.Count;
        if (count == 0)
            return [];

        var documentIds = new List<string>(count);

        for (int i = 0; i < count; i++)
            documentIds.Add(value[i].Id.ToSplitId().DocumentId);

        return documentIds;
    }

    /// <summary>
    /// Lazily yields Document Ids derived from each element's Id.
    /// </summary>
    /// <typeparam name="T">A type derived from <see cref="Dtos.IdNamePair.IdNamePair"/>.</typeparam>
    /// <param name="value">The source list. Must not be null.</param>
    /// <returns>An enumerable yielding Document Ids.</returns>
    /// <remarks>
    /// This method uses an iterator block, which allocates an enumerator object per enumeration.
    /// Prefer <see cref="ToListOfDocumentIds{T}(IList{T}?)"/> or a copy-to pattern if you need zero allocations.
    /// </remarks>
    [Pure]
    public static IEnumerable<string> ToEnumerableOfDocumentIds<T>(this IList<T> value) where T : Dtos.IdNamePair.IdNamePair
    {
        if (value is null)
            throw new ArgumentNullException(nameof(value));

        for (int i = 0, count = value.Count; i < count; i++)
            yield return value[i].Id.ToSplitId().DocumentId;
    }

    /// <summary>
    /// Adds <paramref name="toAdd"/> to the list if no existing element has the same Id.
    /// </summary>
    /// <typeparam name="T">A type derived from <see cref="Dtos.IdNamePair.IdNamePair"/>.</typeparam>
    /// <param name="value">The target list. Must not be null.</param>
    /// <param name="toAdd">The element to add. Must not be null.</param>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="value"/> or <paramref name="toAdd"/> is null.</exception>
    public static void AddIfNotExists<T>(this IList<T> value, T toAdd) where T : Dtos.IdNamePair.IdNamePair
    {
        if (value is null)
            throw new ArgumentNullException(nameof(value));

        if (toAdd is null)
            throw new ArgumentNullException(nameof(toAdd));

        string newId = toAdd.Id;

        for (int i = 0, count = value.Count; i < count; i++)
        {
            if (string.Equals(value[i].Id, newId, _ord))
                return;
        }

        value.Add(toAdd);
    }

    /// <summary>
    /// Adds elements from <paramref name="toAddRange"/> that do not already exist in <paramref name="value"/> by Id.
    /// </summary>
    /// <typeparam name="T">A type derived from <see cref="Dtos.IdNamePair.IdNamePair"/>.</typeparam>
    /// <param name="value">The target list. Must not be null.</param>
    /// <param name="toAddRange">The source list. Must not be null.</param>
    /// <remarks>
    /// For small ranges, this uses linear scans (zero allocations).
    /// For larger ranges, it builds a <see cref="HashSet{T}"/> of existing Ids to reduce comparisons.
    /// </remarks>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="value"/> or <paramref name="toAddRange"/> is null.</exception>
    public static void AddRangeIfNotExists<T>(this IList<T> value, IList<T> toAddRange) where T : Dtos.IdNamePair.IdNamePair
    {
        if (value is null)
            throw new ArgumentNullException(nameof(value));

        if (toAddRange is null)
            throw new ArgumentNullException(nameof(toAddRange));

        int addCount = toAddRange.Count;
        if (addCount == 0)
            return;

        int existingCount = value.Count;

        // Heuristic: avoid allocating a HashSet for small inputs.
        const int hashSetThreshold = 16;

        if (existingCount + addCount <= hashSetThreshold)
        {
            for (int i = 0; i < addCount; i++)
            {
                T item = toAddRange[i];
                string id = item.Id;

                bool exists = false;

                for (int j = 0, count = value.Count; j < count; j++)
                {
                    if (string.Equals(value[j].Id, id, _ord))
                    {
                        exists = true;
                        break;
                    }
                }

                if (!exists)
                    value.Add(item);
            }

            return;
        }

        var existingIds = new HashSet<string>(existingCount + addCount, StringComparer.Ordinal);

        for (int i = 0; i < existingCount; i++)
            existingIds.Add(value[i].Id);

        for (int i = 0; i < addCount; i++)
        {
            T newItem = toAddRange[i];
            if (existingIds.Add(newItem.Id))
                value.Add(newItem);
        }
    }
}
