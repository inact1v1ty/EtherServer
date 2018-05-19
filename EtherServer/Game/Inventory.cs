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
            string playerAddress = "0xb86d5A3D020762efb05588E10ce2DAC6172A2074"; // player.address
            string contractAddress = "0xb13012a5a708a642ae06e1c36dd759e095431fc7";
            string abi = ABI.artifactsAbi;
            string url = "127.0.0.1:8545";

            // server account
            var privateKey = "25b53b659d2f2dbd91c61848381ff73898b94d67f4b7c395b4dc98f0c7ce8fd9";
            var account = new Nethereum.Web3.Accounts.Account(privateKey);

            Web3 web3 = new Web3(account);
            Console.WriteLine("created web3 obj");
            Contract contract = web3.Eth.GetContract(abi, contractAddress);
            Console.WriteLine("got contract");
            var itemsAmount = contract.GetFunction("getArtifactsAmount");
            var amount = await itemsAmount.CallAsync<int>();
            return amount;
        }
    }
}