import { StudentService } from './../../../../Service/student.service';
import { Message } from './../../../../Model/message';
import { HttpClient } from '@angular/common/http';
import { signalRService } from './../../../../Service/signalR.service';
import { Component, OnInit } from '@angular/core';
import * as signalR from '@aspnet/signalr';
import { mainModule } from 'process';

@Component({
  selector: 'app-message-container',
  templateUrl: './message-container.component.html',
  styleUrls: ['./message-container.component.css']
})
export class MessageContainerComponent implements OnInit {

  constructor(public signalRService: signalRService, private http: HttpClient, private userService: StudentService) { 
    this.user = this.userService.getStudentFromStorage();
    this.userId = parseInt(this.user.id);
    this.message = new Message();
    this.message.Sender = this.user.firstName + this.user.lastName;
    this.message.SenderId = parseInt(this.user.id);
    this.message.Receiver = "Petar Tebrica";
    this.message.ReceiverId =  1
  }
  //public message:string = "";
  public message:Message;
  public messsageText: string = "";
  public messageArray: Array<Message> = [];
  public user:any;
  public userId:number = 0;
  public _hubConnection: signalR.HubConnection;
  public sendMessage(): void {
    this._hubConnection
      .invoke('sendToAll', {
        Sender:this.message.Sender,
        SenderId: this.message.SenderId,
        Receiver:this.message.Receiver, 
        ReceiverId:this.message.ReceiverId,
        Content:this.messsageText
      })
      .then(() => this.messsageText = '')
      .catch(err => console.error(err));
  }

  handleAddMessage()
  {
    if(this.messsageText === "")return;
    this.message.Content = this.messsageText;
   
    this.sendMessage();
    //this.messageArray.push(this.message);
    this.messsageText = ""

  }
  ngOnInit(): void {
    
    this._hubConnection = new signalR.HubConnectionBuilder()
    .withUrl("https://localhost:44374/chat")
    .build()

    this._hubConnection
      .start()
      .then(() => console.log('Connection started! :)'))
      .catch(err => console.log('Error while establishing connection :('));

      this._hubConnection.on('sendToAll', (newMessage:any) => {
        let nm:Message = new Message();
        nm.Content = newMessage.content;
        nm.Receiver = newMessage.receiver;
        nm.ReceiverId = newMessage.receiverId;
        nm.Sender = newMessage.sender;
        nm.SenderId = newMessage.senderId;
        this.messageArray.push(nm);
        
      });


  }



}
