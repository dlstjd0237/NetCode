using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using Unity.Netcode;
using System.Net;
using System.Text.RegularExpressions;
using Unity.Netcode.Transports.UTP;
using UnityEngine.SceneManagement;

public class IPConnectPanel : MonoBehaviour
{
    [SerializeField] private Button _hostBtn, _clientBtn;
    [SerializeField] private TMP_InputField _ipText, _portText;

    private void Awake()
    {
        _hostBtn.onClick.AddListener(HandleHostBtnClick);
        _clientBtn.onClick.AddListener(HandleClientBtnClick);

        _ipText.text = FindIPAddress();
        _portText.text = "7777";

        if (NetworkManager.Singleton != null)
            NetworkManager.Singleton.OnClientDisconnectCallback += HandleClientDisconnect;
    }
    private void OnDestroy()
    {
        if (NetworkManager.Singleton == null) return;
            
        NetworkManager.Singleton.OnClientDisconnectCallback -= HandleClientDisconnect;
    }
    private void HandleClientDisconnect(ulong clientID)
    {
        Debug.Log(clientID + ", 오류 발생");
    }

    private string FindIPAddress()
    {
        IPHostEntry entry = Dns.GetHostEntry(Dns.GetHostName());

        foreach (var ip in entry.AddressList)
        {
            if (ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
            {
                return ip.ToString();
            }
        }
        return null;

    }

    private void HandleHostBtnClick()
    {
        bool checker = SettupNetworkPassport();

        if (checker == false) return;

        if (NetworkManager.Singleton.StartHost())
        {
            NetworkManager.Singleton.SceneManager.LoadScene(SceneNames.GameScene, LoadSceneMode.Single);
        }
        else
        {
            NetworkManager.Singleton.Shutdown();
        }
    }

    private void HandleClientBtnClick()
    {
        if (SettupNetworkPassport() == false) return;
        if (NetworkManager.Singleton.StartClient() == false)
        {
            NetworkManager.Singleton.Shutdown();
        }
    }

    private bool SettupNetworkPassport()
    {
        string ip = _ipText.text;
        string port = _portText.text;

        Regex portRegex = new Regex(@"[0-9]{3,5}");
        Regex ipRegex = new Regex(@"^[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}$");

        Match portMatch = portRegex.Match(port);
        Match ipMatch = ipRegex.Match(ip);

        if (portMatch.Success == false || ipMatch.Success == false)
        {
            Debug.Log("올바르지 못한 아이피 또는 포트 번호입니다.");
            return false;
        }

        UnityTransport transport = NetworkManager.Singleton.GetComponent<UnityTransport>();
        transport.SetConnectionData(ip, (ushort)int.Parse(port));

        return true;
    }

}
