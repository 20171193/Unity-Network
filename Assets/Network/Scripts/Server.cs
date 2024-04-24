using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using TMPro;
using UnityEngine;
using System;
using System.IO;

public class Server : MonoBehaviour
{
    [SerializeField] RectTransform logContent;
    [SerializeField] TMP_Text logTextPrefab;
    [SerializeField] TMP_InputField ipField;
    [SerializeField] TMP_InputField portField;

    private TcpListener tcpListener;
    private List<TcpClient> clients = new List<TcpClient>();
    private List<TcpClient> disconnected = new List<TcpClient>();

    private IPAddress ip;
    private int port;

    private bool isOpened;
    public bool IsOpened { get { return isOpened; } }

    private void Awake()
    {
        // 호스트 찾기
        IPHostEntry host = Dns.GetHostEntry(Dns.GetHostName());
        ip = host.AddressList.FirstOrDefault(ip => ip.AddressFamily == AddressFamily.InterNetwork);
        ipField.text = ip.ToString();
    }

    private void OnDestroy()
    {
        if (isOpened)
            Close();
    }

    private void Update()
    {
        // 서버가 닫혀있는 경우
        if (!isOpened) return;

        // 접속중인 클라이언트 갱신
        //CheckClient();
        // 메세지 전송
        SendAll();
    }

    public void Open()
    {
        if (isOpened) return;

        AddLog("Try to Open");

        port = int.Parse(portField.text);
        // 127.0.0.1 - 루프백 ip :자가 회신 IP (테스트 시 많이 사용됨)
        try
        {
            tcpListener = new TcpListener(IPAddress.Any, port);
            Debug.Log("Open server success");
        }
        catch (Exception ex)
        {
            Debug.Log(ex.Message);
            return;
        }

        tcpListener.Start();
        isOpened = true;

        // 클라이언트 요청 대기
        tcpListener.BeginAcceptTcpClient(AcceptCallBack, tcpListener);
    }

    private void AcceptCallBack(IAsyncResult ar)
    {
        if (!isOpened) return;

        // 클라이언트 수용
        TcpClient client = tcpListener.EndAcceptTcpClient(ar);
        clients.Add(client);
        // 클라이언트 요청 대기
        tcpListener.BeginAcceptTcpClient(AcceptCallBack, tcpListener);
    }

    public void Close()
    {
        tcpListener?.Stop();
        tcpListener = null;

        isOpened = false;
    }

    private bool CheckClient(TcpClient client)
    {
        if (client == null || client.Client == null || !client.Connected)
            return false;

        bool check = client.Client.Poll(0, SelectMode.SelectRead);
        if (check && client.Client.Receive(new byte[1], SocketFlags.Peek) == 0)
            return false;
        
        return true;
    }

    private void SendAll()
    {
        foreach (TcpClient client in clients)
        {
            if (!CheckClient(client))
            {
                client.Close();
                disconnected.Add(client);
                continue;
            }

            NetworkStream stream = client.GetStream();
            // 받을 내용이 없다면 continue
            if (!stream.DataAvailable) continue;

            StreamReader reader = new StreamReader(stream);
            string text = reader.ReadLine();
            AddLog(text);
            // 모든 클라이언트에 전달
            Send(client, text);
        }

        if (disconnected.Count > 0)
        {
            foreach (TcpClient client in disconnected)
            {
                clients.Remove(client);
            }
            disconnected.Clear();
        }
    }

    private void Send(TcpClient client, string chat)
    {
        NetworkStream stream = client.GetStream();
        StreamWriter writer = new StreamWriter(stream);

        try
        {
            writer.WriteLine(chat);
            writer.Flush();
        }
        catch (Exception ex)
        {
            Debug.Log(ex.Message);
        }
    }

    private void AddLog(string message)
    {
        Debug.Log($"[Server] {message}");
        TMP_Text newLog = Instantiate(logTextPrefab, logContent);
        newLog.text = message;
    }

}
