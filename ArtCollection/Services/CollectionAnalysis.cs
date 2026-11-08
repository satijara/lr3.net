using ArtCollection.Models;
using System.Collections.Generic;
using System.Linq;

namespace ArtCollection.Services;

public static class CollectionAnalysis
{
    public static decimal TotalValueBefore1900(IEnumerable<Artwork> artworks)
        => artworks.Where(a => a.CreationYear < 1900).Sum(a => a.Evaluate());
}