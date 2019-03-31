using System;
using System.Collections.Generic;
using System.Linq;
using Crm.V2.Interfaces.Models;
using Orleans.Concurrency;
using ProtoBuf;

namespace Crm.V2.Interfaces
{
    [Serializable]
    //[Immutable]
    [ProtoContract]
    public class AccountState
    {
        public AccountState()
        {
            Transactions = new HashSet<Guid>();
            Changes = new List<DomainEvent<IDomainEventEntity>>();
        }
        [ProtoMember(1)]
        public List<DomainEvent<IDomainEventEntity>> Changes { get; set; }
        [ProtoMember(2)]
        public string Name { get; set; }
        [ProtoMember(3)]
        public string Iban { get; set; }
        [ProtoMember(4)]
        public string Bban { get; set; }
        [ProtoMember(5)]
        public HashSet<Guid> Transactions { get; set; }
        [ProtoMember(6)]
        public Guid AccountId { get; set; }

        public void Causes(DomainEvent<IDomainEventEntity> @event)
        {
            AddDomainEvent(@event);
            Apply(@event);
        }

        private void When(NewAccountEvent<NewAccount> @event)
        {
            if (@event.Event is NewAccount eventBody)
            {
                AccountId = eventBody.AccountId;
                Name = eventBody.Name;
                Iban = eventBody.Iban;
                Bban = eventBody.Bban;
            }
        }

        private void AddDomainEvent(DomainEvent<IDomainEventEntity> domainEvent)
        {
            Changes.Add(domainEvent);
        }

        private void Apply(DomainEvent<IDomainEventEntity> @event)
        {
            When((dynamic) @event);
        }

        public AccountDto ToDto()
        {
            return new AccountDto
            {
                Transactions = Transactions.ToList(),
                Iban = Iban,
                Bban = Bban,
                AccountId = AccountId,
                Name = Name
            };
        }
    }
}