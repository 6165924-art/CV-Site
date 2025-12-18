using CvSite.Service.Dtos;
using System;
using System.Collections.Generic;

namespace CvSite.Service
{
    public class PortfolioCacheEntry
    {
        public IEnumerable<RepositoryDto>? Portfolio { get; set; }
        public DateTimeOffset LastFetched { get; set; }
    }
}
