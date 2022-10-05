using System;
using System.Collections.Generic;
using System.Text;
using UemgPergamumAPI.Domain.Entities;
using UemgPergamumAPI.Domain.Enum;

namespace UemgPergamumAPI.Domain.Services
{
    public interface IWebScraperService
    {
        public IEnumerable<LibraryItem> WebScrapingUemgPergamum(string searchTerm, SearchType searchType, string unity = "");
    }
}
