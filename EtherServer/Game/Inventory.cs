using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Nethereum.ABI.FunctionEncoding.Attributes;
using Nethereum.Web3;
using Nethereum.Contracts;

namespace EtherServer.Game
{
    public class Inventory{
        public List<Artifact> artifacts;

        public List<Artifact> bag;

        public Inventory()
        {
            bag = new List<Artifact>();
        }

        public async Task<int> getInventory(Player player){
            Console.WriteLine("started blockchain connection");
            string playerAddress = player.address;
            string contractAddress = "0xaafa19d6f354eee368e0bc6ed0a418cc8bf49763";
            string abi = ABI.artifactsAbi;
            // server account
            var privateKey = "d1b10e2390ffd68062732bf5444494539abfabf4bd7442e753b7d4d7adb67256";
            var account = new Nethereum.Web3.Accounts.Account(privateKey);
            Web3 web3 = new Web3(account);
            Contract contract = web3.Eth.GetContract(abi, contractAddress);
            Console.WriteLine("got contract");
            var getItemsAmount = contract.GetFunction("getArtifactsAmount");
            var amount = await getItemsAmount.CallAsync<int>();

            var getArtifactOwner = contract.GetFunction("artifactIndexToOwner");
            var getArtifact = contract.GetFunction("getArtifactTypeId");
            for(int i = 0; i < amount; ++i){
                var artifactOwner = await getArtifactOwner.CallAsync<string>(i);
                if(artifactOwner == playerAddress){
                    var typeId = await getArtifact.CallAsync<int>(i);
                    artifacts.Add(Artifact.CreateFromBlockchain(typeId, i));
                }
            }
            Console.WriteLine("inventory loaded");
            return amount;
        }
    }
}