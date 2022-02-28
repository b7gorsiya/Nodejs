using UnityEngine;
using WebSocketSharp;
using UnityEngine.UI;
using System.Collections;
using System;

[Serializable]
class WebsocketMesageFormat
{
    //indicate what kind of data to be sent
    public string datatype;

    //login Data
    public string username;
    public string password;

    public string roomName;

    public string chat_text;
    //for login
    public WebsocketMesageFormat(string _username, string _password)
    {
        username = _username;
        password = _password;
    }
    //Create room 
    public WebsocketMesageFormat(string _roomname)
    {
        roomName = _roomname;
    }

    public WebsocketMesageFormat(string _username,string data,int index)
    {
        username = _username;
        chat_text = data;
    }
}

[Serializable]
public class MessageData
{
    public string type;
    public string msgdata;
}
public class Client : MonoBehaviour
{
    [SerializeField]
    string roomName;

    [SerializeField]
    InputField username_input;
    [SerializeField]
    InputField password_input;

    WebSocket ws;
    WebsocketMesageFormat userdata ;

    [SerializeField]
    Button loginBTN;
    [SerializeField]
    Button createRoomBtn;
    [SerializeField]
    Button joinRoomBTn;
    [SerializeField]
    Button testChat;
    [SerializeField]
    Text messageTXt;


    [SerializeField]
    GameObject loginPanel;
    [SerializeField]
    GameObject room_Panel;

    MessageData _data;

    private void Start()
    {
        StartCoroutine(ConnectToserver());
        //add listen on button click
        loginBTN.onClick.AddListener(Login);
        createRoomBtn.onClick.AddListener(CreateRoom);
        joinRoomBTn.onClick.AddListener(JoinRoom);
        testChat.onClick.AddListener(Chat);
        room_Panel.SetActive(false);
    }
    // Connecting to server
    IEnumerator ConnectToserver()
    {
        //wait for stable connection
        yield return new WaitForSeconds(0.5f);
        // connecting to local server
        ws = new WebSocket("ws://localhost:8080");
        ws.Connect();
        ws.OnOpen += ConnectedToServer;
        ws.OnMessage += MessageReciveFromServer;
        //wait  connection response
        yield return new WaitForSeconds(2);
        
    }

    // Event Function for connection
    private void ConnectedToServer(object sender, EventArgs e)
    {
        Debug.Log(e.ToString());
    }

    // event Function For Message response
    private void MessageReciveFromServer(object sender, MessageEventArgs e)
    {

        _data=JsonUtility.FromJson<MessageData>(e.Data);

        CompareMessageData();

        throw new NotImplementedException();
    }

    // Compare each message for relative Actions
    void CompareMessageData()
    {
        switch (_data.type.ToString())
        {
            case "Login":
                Debug.Log("Welcome " + _data.msgdata);
                messageTXt.text = "Welcome " + _data.msgdata;
                break;
            case "RoomCreated":
                Debug.Log("Yes ! You Have Create a Room \n " + _data.msgdata);
               messageTXt.text = "Yes ! You Have Create a Room \n " + _data.msgdata;
                SetupRoom();
                break;
            case "JoinedRoom":
                Debug.Log(_data.msgdata + "  Joind The Room ");
                messageTXt.text = _data.msgdata + "  Joind The Room ";
                SetupRoom();
                break;
            case "ChatRecived":
                Debug.Log(_data.msgdata + ": sent test message ");
                messageTXt.text = messageTXt.text + "\n" + _data.msgdata + ": sent test message ";
                break;

        }
    }    
    // UI Init
    void SetupRoom()
    {
        createRoomBtn.gameObject.SetActive(false);
        joinRoomBTn.gameObject.SetActive(false);
        testChat.gameObject.SetActive(true);
    }
    // When User Login
    void Login()
    {
        userdata = new WebsocketMesageFormat(username_input.text, password_input.text);
        userdata.datatype = "Login";
        ws.Send(JsonUtility.ToJson(userdata));
        room_Panel.SetActive(true);
        loginPanel.SetActive(false);
    }

    // When User Create Room
    void CreateRoom()
    {
        userdata = new WebsocketMesageFormat(roomName);
        userdata.datatype = "CreateRoom";
        ws.Send(JsonUtility.ToJson(userdata));
    }
    // When Other Use Join The Room
    void JoinRoom()
    {
        userdata.datatype = "JoinRoom";
        ws.Send(JsonUtility.ToJson(userdata));
    }
    // Sending Test Message
    void Chat()
    {
        userdata.datatype = "Chat";
        userdata.username = username_input.text;
        ws.Send(JsonUtility.ToJson(userdata));
    }
}
