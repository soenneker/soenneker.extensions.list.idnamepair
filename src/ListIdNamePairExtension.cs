using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using Soenneker.Extensions.String;

namespace Soenneker.Extensions.List.IdNamePair;

/// <summary>
/// A collection of helpful <see cref="List{T}"/> extension methods for working with lists of <see cref="Dtos.IdNamePair.IdNamePair"/> objects.
/// </summary>
public static class ListIdNamePairExtension
{
    /// <summary>
    /// Checks if the list contains an object with the specified Id.
    /// </summary>
    /// <typeparam name="T">A type derived from <see cref="Dtos.IdNamePair.IdNamePair"/>.</typeparam>
    /// <param name="value">The list of <see cref="Dtos.IdNamePair.IdNamePair"/> objects. Must not be null.</param>
    /// <param name="id">The Id to search for within the list.</param>
    /// <returns>True if an object with the specified Id exists in the list; otherwise, false.</returns>
    /// <remarks>This method performs a linear search to locate an object with the given Id.</remarks>
    [Pure]
    public static bool ContainsId<T>(this IList<T> value, string id) where T : Dtos.IdNamePair.IdNamePair
    {
        if (value is null || value.Count == 0)
            return false;

        if (id.IsNullOrEmpty())
            return false;

        // Iterate through the list and check for the matching ID
        for (var i = 0; i < value.Count; i++)
        {
            if (string.Equals(value[i].Id, id, StringComparison.Ordinal))
            {
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// Converts the list to a list of Id strings.
    /// </summary>
    /// <typeparam name="T">A type derived from <see cref="Dtos.IdNamePair.IdNamePair"/>.</typeparam>
    /// <param name="value">The list of <see cref="Dtos.IdNamePair.IdNamePair"/> objects. Must not be null.</param>
    /// <returns>A new <see cref="List{string}"/> containing the Ids of all objects in the original list.</returns>
    /// <remarks>Each element in the resulting list represents the Id of the corresponding element in the original list.</remarks>
    [Pure]
    public static List<string> ToListOfIds<T>(this IList<T> value) where T : Dtos.IdNamePair.IdNamePair
    {
        // Early return for null or empty input
        if (value is null || value.Count == 0)
            return [];

        int count = value.Count;

        var ids = new List<string>(count);

        for (var i = 0; i < count; i++)
        {
            string id = value[i].Id;

            ids.Add(id);
        }

        return ids;
    }

    /// <summary>
    /// Converts the list to an enumerable collection of Id strings.
    /// </summary>
    /// <typeparam name="T">A type derived from <see cref="Dtos.IdNamePair.IdNamePair"/>.</typeparam>
    /// <param name="value">The list of <see cref="Dtos.IdNamePair.IdNamePair"/> objects. Must not be null.</param>
    /// <returns>An <see cref="IEnumerable{string}"/> that yields each Id from the objects in the list.</returns>
    /// <remarks>This method returns an iterator, allowing deferred execution for enumerating Ids.</remarks>
    [Pure]
    private static IEnumerable<string> ToEnumerableOfIds<T>(this IList<T> value) where T : Dtos.IdNamePair.IdNamePair
    {
        for (var i = 0; i < value.Count; i++)
        {
            yield return value[i].Id;
        }
    }

    /// <summary>
    /// Converts the list to a list of Document Ids derived from each object's Id.
    /// </summary>
    /// <typeparam name="T">A type derived from <see cref="Dtos.IdNamePair.IdNamePair"/>.</typeparam>
    /// <param name="value">The list of <see cref="Dtos.IdNamePair.IdNamePair"/> objects. Must not be null.</param>
    /// <returns>A new <see cref="List{string}"/> containing Document Ids for each Id in the original list.</returns>
    /// <remarks>This method assumes each Id can be split into a Document Id using <see cref="ToSplitId"/>.</remarks>
    [Pure]
    public static List<string> ToListOfDocumentIds<T>(this IList<T> value) where T : Dtos.IdNamePair.IdNamePair
    {
        if (value is null || value.Count == 0)
            return [];

        int count = value.Count;

        var documentIds = new List<string>(count);

        for (var i = 0; i < count; i++)
        {
            string documentId = value[i].Id.ToSplitId().DocumentId;

            documentIds.Add(documentId);
        }

        return documentIds;
    }

    /// <summary>
    /// Converts the list to an enumerable collection of Document Ids derived from each object's Id.
    /// </summary>
    /// <typeparam name="T">A type derived from <see cref="Dtos.IdNamePair.IdNamePair"/>.</typeparam>
    /// <param name="value">The list of <see cref="Dtos.IdNamePair.IdNamePair"/> objects. Must not be null.</param>
    /// <returns>An <see cref="IEnumerable{string}"/> that yields each Document Id from the list's objects.</returns>
    /// <remarks>This method allows for deferred execution, providing Document Ids one at a time as they are iterated.</remarks>
    [Pure]
    public static IEnumerable<string> ToEnumerableOfDocumentIds<T>(this IList<T> value) where T : Dtos.IdNamePair.IdNamePair
    {
        for (var i = 0; i < value.Count; i++)
        {
            yield return value[i].Id.ToSplitId().DocumentId;
        }
    }

    /// <summary>
    /// Adds an object to the list if no object with the same Id exists in the list.
    /// </summary>
    /// <typeparam name="T">A type derived from <see cref="Dtos.IdNamePair.IdNamePair"/>.</typeparam>
    /// <param name="value">The list of <see cref="Dtos.IdNamePair.IdNamePair"/> objects. Must not be null.</param>
    /// <param name="toAdd">The object to add if its Id is not already in the list. Must not be null.</param>
    /// <remarks>If an object with the same Id is found, the method returns without adding <paramref name="toAdd"/>.</remarks>
    public static void AddIfNotExists<T>(this IList<T> value, T toAdd) where T : Dtos.IdNamePair.IdNamePair
    {
        string newId = toAdd.Id;

        for (var i = 0; i < value.Count; i++)
        {
            if (string.Equals(value[i].Id, newId, StringComparison.Ordinal))
            {
                return;
            }
        }

        value.Add(toAdd);
    }

    /// <summary>
    /// Adds a range of objects to the list if objects with the same Ids do not already exist.
    /// </summary>
    /// <typeparam name="T">A type derived from <see cref="Dtos.IdNamePair.IdNamePair"/>.</typeparam>
    /// <param name="value">The list of <see cref="Dtos.IdNamePair.IdNamePair"/> objects. Must not be null.</param>
    /// <param name="toAddRange">A list of objects to add. Only objects with unique Ids will be added.</param>
    /// <remarks>
    /// This method first collects existing Ids into a set, then iterates through <paramref name="toAddRange"/> to add only those objects with unique Ids.
    /// </remarks>
    /// <exception cref="ArgumentNullException">Thrown if either <paramref name="value"/> or <paramref name="toAddRange"/> is null.</exception>
    public static void AddRangeIfNotExists<T>(this IList<T> value, IList<T> toAddRange) where T : Dtos.IdNamePair.IdNamePair
    {
        if (value is null)
            throw new ArgumentNullException(nameof(value));

        if (toAddRange is null)
            throw new ArgumentNullException(nameof(toAddRange));

        if (toAddRange.Count == 0)
            return;

        var existingIds = new HashSet<string>(value.Count + toAddRange.Count, StringComparer.Ordinal);

        for (var i = 0; i < value.Count; i++)
        {
            existingIds.Add(value[i].Id);
        }

        for (var i = 0; i < toAddRange.Count; i++)
        {
            var newItem = toAddRange[i];

            if (existingIds.Add(newItem.Id))
            {
                value.Add(newItem);
            }
        }
    }
}