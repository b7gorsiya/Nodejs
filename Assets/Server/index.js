// JavaScript source code

const WebSocket = require('ws');
const wss = new WebSocket.Server({ port: 8080 }, () => {
    console.log('server started')
})

var player;
var chatroom;

wss.on('connection', function connection(ws) {
    console.log('client coonected');
   
    //send akg
    ws.send(' Connection Established')
    // Compare message for relative response to sent
    ws.on('message', (message) => {
        console.log('data received ' + message)

        let data;
        try
        {
            data = JSON.parse(message);
        }
        catch {
                return;
        }

        switch (data.datatype)
        {
            case 'Login': {
                player = data.username;
                 // handle login and save state somewhere
                const datatobesent = {
                    type: 'Login',
                    msgdata: 'Hello ! ' + data.username,
                };
                ws.send(JSON.stringify(datatobesent));
                    break;
                }
            case 'CreateRoom': {
                chatroom = data.roomName;
                    // Handle some other event
                const datatobesent = {
                    type: 'RoomCreated',
                    msgdata: chatroom,
                };
                ws.send(JSON.stringify(datatobesent));

                    break;
            }
            case 'JoinRoom': {
                // Handle some other event
                const datatobesent = {
                    type: 'JoinedRoom',
                    msgdata: player,
                };
                ws.send(JSON.stringify(datatobesent));

                break;
            }
            case 'Chat': {
                // Handle some other event
                const datatobesent = {
                    type: 'ChatRecived',
                    msgdata: player,
                };
                //for brodcasting ~~~ but not working for some reason
                wss.broadcast = function () {
                    wss.clients.forEach(client => client.send(JSON.stringify(datatobesent)));
                };
                    ws.send(JSON.stringify(datatobesent));
                break;
            }
        }
    })
})
wss.on('listening', () => {
    console.log('listening on 8080')
})