using System.IO;
using System.Net.Sockets;
using TMPro;
using UnityEngine;
using System;

public class Client : MonoBehaviour
{
    [SerializeField] Chat chat;
        
    [SerializeField] TMP_InputField nameField;
    [SerializeField] TMP_InputField ipField;
    [SerializeField] TMP_InputField portField;

    private TcpClient tcpClient;
    private NetworkStream stream;
    private StreamWriter writer;
    private StreamReader reader;

    [SerializeField]
    private string clientName;
    public string ClientName { get { return clientName; } }

    [SerializeField]
    private string ip;
    public string IP { get { return ip; } }

    [SerializeField]
    private int port; 
    public int Port { get { return port; } }

    [SerializeField]
    private bool isConnected;
    public bool IsConnected { get { return isConnected;} }

    private void Update()
    {
        #region Exception
        // 연결되어있지 않은 경우
        if (!isConnected) return;
        // 받을 데이터가 없는 경우
        if (!stream.DataAvailable) return;
        #endregion

        string text = reader.ReadLine();
        ReceiveChat(text);
    }

    public void Connect()
    {
        if (isConnected) return;

        clientName = nameField.text.Replace(" ", "");
        ip = ipField.text.Replace(" ", "");
        port = int.Parse(portField.text.Replace(" ", ""));

        try
        {
            // 클라이언트 생성
            tcpClient = new TcpClient(ip, port);
            Debug.Log("Connect success");
        }
        catch (Exception ex)
        {
            Debug.Log(ex.Message);
            return;
        }

        stream = tcpClient.GetStream();
        writer = new StreamWriter(stream);
        reader = new StreamReader(stream);
        isConnected = true;
    }

    public void DisConnect()
    {
        if (!isConnected) return;

        writer?.Close();
        writer = null;

        reader?.Close();
        reader = null;

        stream?.Close();
        stream = null;

        // TCP Client 만 Close 해주어도 자동적으로 stream, writer, reader도 Close됨.
        tcpClient?.Close();
        tcpClient = null;

        isConnected = false;
    }

    public void SendChat(string chatText)
    {
        if (!isConnected) return;

        try
        {
            writer.WriteLine($"{clientName} : {chatText}");
            // 버퍼를 지워줌.
            writer.Flush();
            Debug.Log("Send chat success");
        }
        catch(Exception ex)
        {
            Debug.Log(ex.Message);  
        }
    }

    public void ReceiveChat(string chatText)
    {
        if (!isConnected) return;

        chat.AddMessage(chatText);
    }
}
