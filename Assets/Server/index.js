// JavaScript source code

const WebSocket = require('ws');
const wss = new WebSocket.Server({ port: 8080 }, () => {
    console.log('server started')
})

var player;
var chatroom;
var clients = [2];
var clientsIndex = 0;

wss.on('connection', function connection(ws) {
    id = Math.random();
    console.log('connection is established : ' + id);
    clients[clientsIndex] = ws;
    clientsIndex++;
    clients.push(ws);
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
                    msgdata: data.username,
                };
                for (var j = 0; j < clients.length; j++) {
                    console.log('FOR : ', JSON.stringify(datatobesent));
                    clients[j].send(JSON.stringify(datatobesent));
                }
                break;
            }
        }
    })
})
wss.on('listening', () => {
    console.log('listening on 8080')
})