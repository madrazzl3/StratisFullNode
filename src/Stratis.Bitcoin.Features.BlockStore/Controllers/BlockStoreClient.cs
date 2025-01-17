﻿using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Stratis.Bitcoin.Controllers;
using Stratis.Bitcoin.Controllers.Models;

namespace Stratis.Bitcoin.Features.BlockStore.Controllers
{
    /// <summary>Rest client for <see cref="BlockStoreController"/>.</summary>
    public interface IBlockStoreClient : IRestApiClientBase
    {
        /// <summary><see cref="BlockStoreController.GetAddressesBalances"/></summary>
        Task<AddressBalancesResult> GetAddressBalancesAsync(IEnumerable<string> addresses, int minConfirmations, CancellationToken cancellation = default);

        /// <summary><see cref="BlockStoreController.GetVerboseAddressesBalancesData"/></summary>
        Task<VerboseAddressBalancesResult> GetVerboseAddressesBalancesDataAsync(IEnumerable<string> addresses, CancellationToken cancellation = default);

        /// <summary><see cref="BlockStoreController.VerboseAddressesBalancesData"/></summary>
        Task<VerboseAddressBalancesResult> VerboseAddressesBalancesDataAsync(IEnumerable<string> addresses, CancellationToken cancellation = default);
    }

    /// <inheritdoc cref="IBlockStoreClient"/>
    public class BlockStoreClient : RestApiClientBase, IBlockStoreClient
    {
        /// <summary>
        /// Currently the <paramref name="url"/> is required as it needs to be configurable for testing.
        /// <para>
        /// In a production/live scenario the sidechain and mainnet federation nodes should run on the same machine.
        /// </para>
        /// </summary>
        public BlockStoreClient(IHttpClientFactory httpClientFactory, string url, int port)
            : base(httpClientFactory, port, "BlockStore", url)
        {
        }

        /// <inheritdoc />
        public Task<AddressBalancesResult> GetAddressBalancesAsync(IEnumerable<string> addresses, int minConfirmations, CancellationToken cancellation = default)
        {
            string addrString = string.Join(",", addresses);

            string arguments = $"{nameof(addresses)}={addrString}&{nameof(minConfirmations)}={minConfirmations}";

            return this.SendGetRequestAsync<AddressBalancesResult>(BlockStoreRouteEndPoint.GetAddressesBalances, arguments, cancellation);
        }

        /// <inheritdoc />
        public Task<VerboseAddressBalancesResult> GetVerboseAddressesBalancesDataAsync(IEnumerable<string> addresses, CancellationToken cancellation = default)
        {
            string addrString = string.Join(",", addresses);

            string arguments = $"{nameof(addresses)}={addrString}";

            return this.SendGetRequestAsync<VerboseAddressBalancesResult>(BlockStoreRouteEndPoint.GetVerboseAddressesBalances, arguments, cancellation);
        }

        /// <inheritdoc />
        public Task<VerboseAddressBalancesResult> VerboseAddressesBalancesDataAsync(IEnumerable<string> addresses, CancellationToken cancellation = default)
        {
            string addrString = string.Join(",", addresses);

            return this.SendPostRequestAsync<string, VerboseAddressBalancesResult>(addrString, BlockStoreRouteEndPoint.VerboseAddressesBalances, cancellation);
        }
    }
}