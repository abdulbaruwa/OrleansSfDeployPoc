using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Crm.V2.Interfaces.Models;
using Orleans;
using Orleans.Concurrency;

namespace Crm.V2.Interfaces
{
    public interface IAccountGrain : IGrainWithGuidKey
    {
        Task NewAsync(NewAccountEvent<NewAccount> @event);
        Task<AccountDto> GetAsync();
    }

    public interface IDomainEventEntity
    {
    }

    [Serializable]
    [Immutable]
    public class DomainEvent<T>
    {
        public DomainEvent(Guid id, string name, DateTime whenUtc, T @event)
        {
            Id = id;
            Event = @event;
            WhenUtc = whenUtc;
            EventName = name;
        }

        public string EventName { get; set; }
        public Guid Id { get; set; }
        public DateTime WhenUtc { get; set; }
        public T Event { get; set; }
    }

    [Serializable]
    [Immutable]
    public class AccountState
    {
        public AccountState()
        {
            Transactions = new HashSet<Guid>();
            Changes = new List<DomainEvent<IDomainEventEntity>>();
        }

        public List<DomainEvent<IDomainEventEntity>> Changes { get; }
        public string Name { get; private set; }
        public string Iban { get; private set; }
        public string Bban { get; private set; }
        public HashSet<Guid> Transactions { get; }
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

    [Serializable]
    [Immutable]
    public class NewAccountEvent<T> : DomainEvent<IDomainEventEntity> where T : NewAccount
    {
        public NewAccountEvent(Guid id, string name, DateTime whenUtc, IDomainEventEntity @event) : base(id, name,
            whenUtc, @event)
        {
        }
    }

    [Serializable]
    [Immutable]
    public class NewAccount : IDomainEventEntity
    {
        public NewAccount(Guid accountId, string name, string iban, string bban)
        {
            Name = name;
            Iban = iban;
            Bban = bban;
            AccountId = accountId;
        }

        public string Name { get; }
        public string Iban { get; }
        public string Bban { get; }
        public Guid AccountId { get; }
    }
}