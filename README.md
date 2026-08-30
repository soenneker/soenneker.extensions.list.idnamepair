[![](https://img.shields.io/nuget/v/soenneker.extensions.list.idnamepair.svg?style=for-the-badge)](https://www.nuget.org/packages/soenneker.extensions.list.idnamepair/)
[![](https://img.shields.io/github/actions/workflow/status/soenneker/soenneker.extensions.list.idnamepair/publish-package.yml?style=for-the-badge)](https://github.com/soenneker/soenneker.extensions.list.idnamepair/actions/workflows/publish-package.yml)
[![](https://img.shields.io/nuget/dt/soenneker.extensions.list.idnamepair.svg?style=for-the-badge)](https://www.nuget.org/packages/soenneker.extensions.list.idnamepair/)
[![](https://img.shields.io/github/actions/workflow/status/soenneker/soenneker.extensions.list.idnamepair/codeql.yml?label=CodeQL&style=for-the-badge)](https://github.com/soenneker/soenneker.extensions.list.idnamepair/actions/workflows/codeql.yml)

# ![](https://user-images.githubusercontent.com/4441470/224455560-91ed3ee7-f510-4041-a8d2-3fc093025112.png) Soenneker.Extensions.List.IdNamePair
ID lookup, projection, document-ID extraction, and deduplicated additions for lists of `IdNamePair` values.

## Installation

```bash
dotnet add package Soenneker.Extensions.List.IdNamePair
```

## Find an ID

```csharp
using Soenneker.Extensions.List.IdNamePair;

bool contains = pairs.ContainsId("partition:document");
```

`ContainsId()` compares the complete `Id` using ordinal, case-sensitive equality. A null/empty list or null/empty search ID returns `false`.

## Project IDs

```csharp
List<string> ids = pairs.ToListOfIds();
List<string> documentIds = pairs.ToListOfDocumentIds();
```

`ToListOfIds()` copies every complete ID in source order. `ToListOfDocumentIds()` extracts the portion after the last colon:

| Complete ID | Document ID |
|---|---|
| `partition:document` | `document` |
| `tenant:partition:document` | `document` |
| `document` | `document` |
| `:document` | `document` |
| `partition:` | empty string |

Both methods return a new empty list for a null or empty source. They preserve duplicates.

Use the enumerable form when the result should stay lazy:

```csharp
IEnumerable<string> documentIds = pairs.ToEnumerableOfDocumentIds();
```

`ToEnumerableOfDocumentIds()` performs extraction during enumeration and requires a non-null source. Do not structurally modify the source while enumerating it. Materialize with `ToListOfDocumentIds()` when a stable snapshot is needed.

## Add only new IDs

```csharp
pairs.AddIfNotExists(candidate);
pairs.AddRangeIfNotExists(candidates);
```

`AddIfNotExists()` appends the supplied object only when the destination has no exact, case-sensitive ID match. `AddRangeIfNotExists()` applies the same rule to each source item and also suppresses duplicate IDs within the incoming range. Existing destination duplicates are not removed, and an existing item is not replaced with the incoming object.

The add methods require non-null writable lists and non-null items. None of these methods trim, normalize, or parse IDs for equality; document-ID extraction is the only operation that treats `:` specially.
