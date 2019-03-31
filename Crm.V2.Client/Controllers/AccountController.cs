using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Crm.V2.Interfaces;
using Crm.V2.Interfaces.Models;
using Microsoft.AspNetCore.Mvc;
using Orleans;

namespace Crm.V2.Client.Controllers
{
    public class NewAccountRequest
    {
        public string Name { get; set; }
        public string Iban { get; set; }
        public Guid AccountId { get; set; }
    }

    [Produces("application/json")]
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IClusterClient _clusterClient;

        public AccountController(IClusterClient clusterClient)
        {
            _clusterClient = clusterClient;
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public Task<AccountDto> Get(Guid id)
        {
            var customerActor = _clusterClient.GetGrain<IAccountGrain>(id);
            return customerActor.GetAsync();
        }

        // POST api/values
        [HttpPost]
        public async Task Post([FromBody] NewAccountRequest value)
        {
            var accountActor = _clusterClient.GetGrain<IAccountGrain>(value.AccountId);
            var @event = new NewAccountEvent<NewAccount>(value.AccountId, "New Account", DateTime.UtcNow,
                new NewAccount(value.AccountId, value.Name, value.Iban));
            await accountActor.NewAsync(@event);
        }

        [HttpPost]
        [Route("CreateAccounts")]
        public async Task<List<string>> CreateAccounts(int number)
        {
            var accounts = new List<string>();
            for (int i = 0; i < number; i++)
            {
                var accountId = Guid.NewGuid();
                var accountActor = _clusterClient.GetGrain<IAccountGrain>(accountId);
                var @event = new NewAccountEvent<NewAccount>(accountId, "New Account", DateTime.UtcNow,
                    new NewAccount(accountId, Guid.NewGuid().ToString(), Guid.NewGuid().ToString()));
                await accountActor.NewAsync(@event);
                accounts.Add(accountId.ToString());
            }
            return accounts;
        }
    }
}