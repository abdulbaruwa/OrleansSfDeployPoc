using System;
using System.Threading.Tasks;
using Crm.V2.Interfaces;
using Crm.V2.Interfaces.Models;
using Orleans;
using Orleans.Providers;

namespace Crm.V2.Implementations
{
    [StorageProvider(ProviderName = "GloballySharedAzureAccount")]
    public class AccountGrain : Grain<AccountState>, IAccountGrain
    {
        public Task NewAsync(NewAccountEvent<NewAccount> @event)
        {
            if (State.AccountId == Guid.Empty)
            {
                State.Causes(@event);
                base.WriteStateAsync();
            }

            return Task.CompletedTask;
        }

        public Task<AccountDto> GetAsync()
        {
            return Task.FromResult(State.ToDto());
        }
    }
}