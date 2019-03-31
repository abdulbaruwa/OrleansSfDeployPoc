using System;
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