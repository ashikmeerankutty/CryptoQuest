using System.Threading.Tasks;
using Thirdweb;
using UnityEngine;

public class TokenClaimer : MonoBehaviour
{
    private ThirdwebSDK sdk;

    public GameObject balanceText;

    public GameObject claimButton;

    async void OnEnable()
    {
        sdk =
            new ThirdwebSDK("mumbai",new ThirdwebSDK.Options()
                {
                    gasless =
                        new ThirdwebSDK.GaslessOptions()
                        {
                            openzeppelin =
                                new ThirdwebSDK.OZDefenderOptions()
                                {
                                    relayerUrl =
                                        "https://api.defender.openzeppelin.com/autotasks/c692183c-fd51-4430-a422-c9bb2edce898/runs/webhook/0dfe2c75-0aeb-4f38-8647-581b8b035750/Xyc966ZL5PipusFysPRyCJ"
                                }
                        }
                });

        string wallet = GetWalletKey();
        if(string.Equals(wallet, "Coinbase")) {
            await Coinbase();
        } else {
            await Metamask();
        }
        CheckBalance();
    }

     public string GetWalletKey()
    {
        return PlayerPrefs.GetString("SelectedProvider");
    }

    public async void Claim()
    {
        // Update claim button text
        claimButton.GetComponentInChildren<TMPro.TextMeshProUGUI>().text =
            "Claiming...";

        await getTokenDrop().ERC20.Claim("25");

        // hide claim button
        claimButton.SetActive(false);

        CheckBalance();
    }

    private Contract getTokenDrop()
    {
        return sdk.GetContract("0x9D8CC066Db25FE2751277c299b78a434bA74C148");
    }

    private async void CheckBalance()
    {
        // Set text to user's balance
        var bal = await getTokenDrop().ERC20.Balance();

        balanceText.GetComponent<TMPro.TextMeshProUGUI>().text =
            bal.displayValue + " " + bal.symbol;
    }

    public async Task<string> WalletConnect()
    {
        string address =
            await sdk
                .wallet
                .Connect(new WalletConnection()
                {
                    provider = WalletProvider.WalletConnect,
                    chainId = 80001 // Switch the wallet Polygon network on connection
                });
        return address;
    }

    public async Task<string> Metamask()
    {
        string address =
            await sdk
                .wallet
                .Connect(new WalletConnection()
                {
                    provider = WalletProvider.MetaMask,
                    chainId = 80001 // Switch the wallet Polygon network on connection
                });
        return address;
    }

    public async Task<string> Coinbase()
    {
        string address =
            await sdk
                .wallet
                .Connect(new WalletConnection()
                {
                    provider = WalletProvider.CoinbaseWallet,
                    chainId = 80001 // Switch the wallet Polygon network on connection
                });
        return address;
    }
}