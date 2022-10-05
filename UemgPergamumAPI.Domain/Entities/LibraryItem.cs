using System;
using System.Collections.Generic;
using System.Text;

namespace UemgPergamumAPI.Domain.Entities
{
    public class LibraryItem
    {
        public string Title { get; set; }
        public string Author { get; set; }
        public string Publisher { get; set; }
        public IEnumerable<LibraryUnity> Unities { get; set; }

    }
}
