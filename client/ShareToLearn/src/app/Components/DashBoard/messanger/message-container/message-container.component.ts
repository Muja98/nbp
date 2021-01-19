import { StudentService } from './../../../../Service/student.service';
import { Message } from './../../../../Model/message';
import { HttpClient, HttpParams } from '@angular/common/http';
import { signalRService } from './../../../../Service/signalR.service';
import { Component, EventEmitter, Input, OnInit } from '@angular/core';
import * as signalR from '@aspnet/signalr';
import { mainModule } from 'process';
import { Student } from 'src/app/Model/student';
import { MessageService } from 'src/app/Service/message.service';

@Component({
  selector: 'app-message-container',
  templateUrl: './message-container.component.html',
  styleUrls: ['./message-container.component.css']
})
export class MessageContainerComponent implements OnInit {
  @Input() student:Student;
  @Input() changeStudentEvent: EventEmitter<Student>;
  @Input() newMessageEvent: EventEmitter<Message>;
  public message:Message;
  public messsageText: string = "";
  public messageArray: Array<Message> = [];
  public user:any;
  public userId:number = 0;
  public _hubConnection: signalR.HubConnection;
  public imgSrc:string;
  public loadedMessages:number = 0;
  public perLoadCount:number = 20;

  constructor(public signalRService: signalRService, private http: HttpClient, private userService: StudentService, private messageService:MessageService) { }

  handleAddMessage()
  {
    if(this.messsageText === "")return;
    this.message.content = this.messsageText;

    this.messageArray.push(JSON.parse(JSON.stringify(this.message)));
    this.http.post("https://localhost:44374/api/messages/send", {
      Sender:this.message.sender,
      SenderId: this.message.senderId,
      Receiver:this.message.receiver, 
      ReceiverId:this.message.receiverId,
      Content:this.messsageText
    }).subscribe(()=>{})
    this.messsageText = "";
  }

  
  // joinRoom()
  // {
  //   const biggerId = this.message.senderId > this.message.receiverId ? this.message.senderId : this.message.receiverId;
  //   const smallerId = this.message.senderId < this.message.receiverId ? this.message.senderId : this.message.receiverId;
  //   const channelName = "messages:" + this.message.senderId + ":chat";
  //   console.log(channelName);
  //   this._hubConnection.invoke("JoinRoom", channelName).catch((err)=>{
  //     console.log(err)
  //   })
  // }


  ngOnInit(): void {  
    // let pomuser = this.userService.getStudentFromStorage();
    if(this.changeStudentEvent) {
      this.changeStudentEvent.subscribe(data => {
        this.student = data;
        this.messageArray = []
        this.loadedMessages = 0;
        this.setMessageParams();
        this.getMessages(this.perLoadCount, '+', false);
      })
    }

    if(this.newMessageEvent) {
      this.newMessageEvent.subscribe(mess => {
        this.messageArray.push(mess);
      })
    }
    
    this.setMessageParams();
    
    if(this.student.student.profilePicturePath)
      this.imgSrc = 'data:image/png;base64,' + this.student.student.profilePicturePath;
    else
      this.imgSrc = "assets/profileDefault.png";

    // this._hubConnection = new signalR.HubConnectionBuilder()
    // .withUrl("https://localhost:44374/chat", )
    // .build()

    // this._hubConnection
    //   .start()
    //   .then(() => {
    //     console.log('Connection started! :)')
    //     this.joinRoom()
    //   })
    //   .catch(err => console.log('Error while establishing connection :('));

   
    // this._hubConnection.on('ReceiveMessage', (newMessage:any) => {
    //   console.log(newMessage)
    //   let nm:Message = new Message();
    //   nm.content = newMessage.content;
    //   nm.receiver = newMessage.receiver;
    //   nm.receiverId = newMessage.receiverId;
    //   nm.sender = newMessage.sender;
    //   nm.senderId = newMessage.senderId;
    //   this.messageArray.push(nm);
      
    // });
   
    this.getMessages(this.perLoadCount, '+', false);
  }

  public loadMore() {
    const fromId:string = this.messageArray[0].id;
    this.getMessages(this.perLoadCount + 1, fromId, true);
  }

  private getMessages(count:number, from:string, slice:boolean) {
    let params = new HttpParams()
      .set('senderId', String(this.message.senderId))
      .set('receiverId', String(this.message.receiverId))
      .set('from', encodeURIComponent(from)).set('count', String(count))
    this.messageService.getMessagePortion(params).subscribe(result => {
      let tempMessages = slice ? result.slice(1).reverse() : result.reverse()
      this.loadedMessages += tempMessages.length;
      this.messageArray = tempMessages.concat(this.messageArray);
    })
  }

  private setMessageParams() {
    this.user = this.userService.getStudentFromStorage();
    this.userId = parseInt(this.user.id);
    this.message = new Message();
    this.message.sender = this.user.firstName + " " + this.user.lastName;
    this.message.senderId = parseInt(this.user.id);
    this.message.receiver = this.student.student.firstName + " " + this.student.student.lastName;
    this.message.receiverId =  this.student.id;
  }


    //-----------------------------------------------------------
    // this._hubConnection = new signalR.HubConnectionBuilder()
    // .withUrl("https://localhost:44374/chat", )
    
    // .build()

    // this._hubConnection
    //   .start()
    //   .then(() => console.log('Connection started! :)'))
    //   .catch(err => console.log('Error while establishing connection :('));

   


  }






