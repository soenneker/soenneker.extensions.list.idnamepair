[![](https://img.shields.io/nuget/v/soenneker.extensions.list.idnamepair.svg?style=for-the-badge)](https://www.nuget.org/packages/soenneker.extensions.list.idnamepair/)
[![](https://img.shields.io/github/actions/workflow/status/soenneker/soenneker.extensions.list.idnamepair/publish-package.yml?style=for-the-badge)](https://github.com/soenneker/soenneker.extensions.list.idnamepair/actions/workflows/publish-package.yml)
[![](https://img.shields.io/nuget/dt/soenneker.extensions.list.idnamepair.svg?style=for-the-badge)](https://www.nuget.org/packages/soenneker.extensions.list.idnamepair/)
[![](https://img.shields.io/github/actions/workflow/status/soenneker/soenneker.extensions.list.idnamepair/codeql.yml?label=CodeQL&style=for-the-badge)](https://github.com/soenneker/soenneker.extensions.list.idnamepair/actions/workflows/codeql.yml)

# ![](https://user-images.githubusercontent.com/4441470/224455560-91ed3ee7-f510-4041-a8d2-3fc093025112.png) Soenneker.Extensions.List.IdNamePair
A collection of helpful List{IdNamePair} extension methods.

## Installation

```bash
dotnet add package Soenneker.Extensions.List.IdNamePair
```

## Quick start

```csharp
using Soenneker.Extensions.List.IdNamePair;

// Given an existing IList<T>? named value:
var result = value.ContainsId(id);
```

## Common operations

- `ContainsId()` - Checks whether the list contains an object with the specified Id.
- `ToListOfIds()` - Converts the list to a new list containing all Id values.
- `ToListOfDocumentIds()` - Converts the list to a new list containing Document Ids derived from each element's Id. Returns a new list containing Document Ids in the same order as the source.
- `ToEnumerableOfDocumentIds()` - Lazily yields Document Ids derived from each element's Id. Returns an enumerable yielding Document Ids. This method uses an iterator block, which allocates an enumerator object per enumeration.
- `AddIfNotExists()` - Adds `toAdd` to the list if no existing element has the same Id.
- `AddRangeIfNotExists()` - Adds elements from `toAddRange` that do not already exist in `value` by Id. For small ranges, this uses linear scans (zero allocations).
