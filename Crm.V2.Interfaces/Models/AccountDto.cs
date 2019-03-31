using System;
using System.Collections.Generic;

namespace Crm.V2.Interfaces.Models
{
    public class AccountDto
    {
        public string Name { get; set; }
        public string Iban { get; set; }
        public List<Guid> Transactions { get; set; }
        public Guid AccountId { get; set; }
    }
}