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

        public async Task<int> getInventory(Player player){
            Console.WriteLine("started bc connection");
            string playerAddress = "0x6b2fb2df3ae3b22b8a4842461231d7b0e556ffd3"; // player.address
            string contractAddress = "0xaafa19d6f354eee368e0bc6ed0a418cc8bf49763";
            string abi = ABI.artifactsAbi;

            // server account
            var privateKey = "25b53b659d2f2dbd91c61848381ff73898b94d67f4b7c395b4dc98f0c7ce8fd9";
            var account = new Nethereum.Web3.Accounts.Account(privateKey);

            Web3 web3 = new Web3(account);
            Console.WriteLine("created web3 obj");
            Contract contract = web3.Eth.GetContract(abi, contractAddress);
            Console.WriteLine("got contract");
            var getItemsAmount = contract.GetFunction("getArtifactsAmount");
            var amount = await getItemsAmount.CallAsync<int>();

            var getArtifactOwner = contract.GetFunction("artifactIndexToOwner");
            var getArtifact = contract.GetFunction("getArtifactTypeId");
            Console.WriteLine("started loop");
            for(int i = 0; i < amount; ++i){
                var artifactOwner = await getArtifactOwner.CallAsync<string>(i);
                if(artifactOwner == playerAddress){
                    var typeId = await getArtifact.CallAsync<int>(i);
                    artifacts.Add(new Artifact(typeId, i, true));
                }
            }

            return amount;
        }
    }
}