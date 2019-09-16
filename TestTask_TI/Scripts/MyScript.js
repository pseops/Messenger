let currentChat = null;
let receiverUser = null;
let editMessageId = null;
let colors = createColorScheme(2);
function HandleDelete(e) {    
    let settingsChat = {
        "async": true,
        "crossDomain": false,
        "url": "/Home/DeleteMessage?id=" + e.getAttribute('data-messageid'),
        "method": "GET"
    };

    $.ajax(settingsChat).done(function (response) {
      
    });
}
function HandleEdit(msg,e) {
    $("#inputID").val(msg);
    editMessageId = e.getAttribute('data-messageid');
}
function SendMessage(msg) {
    let Id = currentChat;
    let editModeId = editMessageId;
    editMessageId = null;
    let settingsChat = {
        "async": true,
        "crossDomain": false,
        "data": {
            msg: msg,
            userId: window.UserId,
            id: editModeId ? editModeId:Id,
        },
        "url": `/Home/${editModeId ? "EditMessage" : "SendMessage"}/`,
        "method": "POST"
    };

    $.ajax(settingsChat).done(function (response) {
        $("#inputID").val("");
    });
}
function GetMessages(receiverId, senderId) {
    let settingsChat = {
        "async": true,
        "crossDomain": false,
        "url": "/Home/GetChat/" + '?receiverId=' + receiverId + '&senderId=' + senderId,
        "method": "GET"
    };

    $.ajax(settingsChat).done(function (response) {
        currentChat = Array.isArray(response) && response[0] ? response[0].Chats_Id : null;
        receiverUser = receiverId;
        
        let body = Array.isArray(response) && response[0] ? response.map(function (Message) {
            let color = Message.CreatorId == window.UserId ? colors[0] : colors[1];
            let colorRGB = hexToRgb(color);
            
            let messageIsUrl = Message.Message.indexOf("/files/") == 0;
            return `<div class="message-block ${Message.CreatorId == window.UserId ? "" : "reverse"}" style="background:rgba(${colorRGB.r},${colorRGB.g},${colorRGB.b},0.1" >
                        <div class='Author ' style='color:${color}' >${Message.Name}</div>
                        <div class='Message'>${messageIsUrl ? "<a target='_blank' href='" + Message.Message + "'>" + Message.Message.slice(7) + "</a>" : Message.Message}</div >
                        <i onclick="HandleDelete(this)" data-messageid="${Message.Id}" class="fas fa-times ${Message.CreatorId == window.UserId ? "" : "reverse"} ${(window.UserId != Message.CreatorId) || !Message.Permision ? "hidden" : ""}" id="handleDelete"></i>
                        <i onclick="HandleEdit('${Message.Message}',this)" data-messageid="${Message.Id}" class="fas fa-edit ${Message.CreatorId == window.UserId ? "" : "reverse"} ${(window.UserId != Message.CreatorId) || !Message.Permision ? "hidden" : ""}" id="handleEdit"></i>
                    </div>`;
            
        }) : "<div class='alert alert-info'>You don't have messages yet</div>";
        $(".message-block").remove();
        $(".alert").remove();
        $("#Messages").append(body);

    });
}
function hexToRgb(hex) {
    var result = /^#?([a-f\d]{2})([a-f\d]{2})([a-f\d]{2})$/i.exec(hex);
    return result ? {
        r: parseInt(result[1], 16),
        g: parseInt(result[2], 16),
        b: parseInt(result[3], 16)
    } : null;
}
function createColorScheme(value)
{
    const letters = '0123456789ABCDEF';
    let result =[];
    for (let v = 0; v < value; v++)
    {
        let resultColor = '#';
        for (var i = 0; i < 6; i++)
        {
            resultColor += letters[Math.floor(Math.random() * 12)];
        }
        result.push(resultColor);
    }
    return result;
}
$(document).ready(function () {
    $("#uploadBtn").click(function (e) {
        $("#fileInput").trigger("click");
    });
    $("#fileInput").change(function (e) {
        var data = new FormData();
        $.each($('#fileInput')[0].files, function (i, file) {
            data.append('file', file);
        });
        $.ajax({
            type: "POST",
            enctype: 'multipart/form-data',
            url: "/Home/Index",
            data: data,
            processData: false,
            contentType: false,
            cache: false,
            timeout: 600000,
            success: function (data) {
                SendMessage(data);
            }
        });
        e.target.value = "";
    });


    if (window.currentChatUser) {
        GetMessages(window.currentChatUser, window.UserId);
        $("#" + window.currentChatUser).css("background", "#d9edf7");
    }

    $(".contactButton").click(function (e) {
        let Id = e.target.id;
        $(".contactButton").each(function (elem) {
            $(this).css("background", "#eee");
        });
        $("#" + Id).css("background", "#d9edf7");
        GetMessages(Id, window.UserId);
    });

    $("#sendMessage").click(function (e) {

        SendMessage($("#inputID").val());
    });
    var chat = $.connection.chatHub;

    chat.client.addMessage = function (name) {
        // Добавление сообщений на веб-страницу 
        if (name == currentChat)
            GetMessages(receiverUser, window.UserId);
    };

    $.connection.hub.start().done(function () {
        console.log("connection Started");
        

        // обработка логина
        chat.server.connect(window.UserId);
    });


})