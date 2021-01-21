import { StudentService } from './../../../../Service/student.service';
import { Message } from './../../../../Model/message';
import { HttpClient, HttpParams } from '@angular/common/http';
import { signalRService } from './../../../../Service/signalR.service';
import { Component, EventEmitter, Input, OnInit } from '@angular/core';
import * as signalR from '@aspnet/signalr';
import { mainModule } from 'process';
import { Student } from 'src/app/Model/student';
import { MessageService } from 'src/app/Service/message.service';
import { Time } from '@angular/common';

@Component({
  selector: 'app-message-container',
  templateUrl: './message-container.component.html',
  styleUrls: ['./message-container.component.css']
})
export class MessageContainerComponent implements OnInit  {
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
  public timeLeft:number;
  private intervalHandler:any;

  constructor(public signalRService: signalRService, private http: HttpClient, private userService: StudentService, private messageService:MessageService  ) {

   }

  
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

  deleteConverasation()
  {
      let user =  this.userService.getStudentFromStorage().id;
      let chatFriend = this.student.id

      let bigger:number = 0;
      let smaller:number = 0;
      
      bigger = parseInt( user>chatFriend? user:chatFriend );
      smaller = parseInt( user<chatFriend? user:chatFriend );

      this.messageService.deleteMessage(bigger,smaller);
      this.messageArray = [];
      window.location.reload();
  }
  
  ngOnInit(): void {
    if(this.changeStudentEvent) {
      this.changeStudentEvent.subscribe(data => {
        clearInterval(this.intervalHandler);
        this.student = data;
        this.messageArray = []
        this.loadedMessages = 0;
        this.setMessageParams();
        this.getMessages(this.perLoadCount, '+', false);
        this.getTimeLeft();
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

    this.getMessages(this.perLoadCount, '+', false);
    this.getTimeLeft();
  }

  public loadMore() {
    const fromId:string = this.messageArray[0].id;
    this.getMessages(this.perLoadCount + 1, fromId, true);
  }

  public timeDisplayString() {
    let hours = Math.floor(this.timeLeft/ (60 * 60))
    let minutes = Math.floor((this.timeLeft % (60 * 60)) / 60);
    let seconds = Math.floor((this.timeLeft % 60));

    return String(hours) + ":" + String(minutes) + ":" + String(seconds);
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

  private getTimeLeft() {
    this.messageService.getConversationTimeLeft(this.userId, this.student.id).subscribe(result => {
      this.timeLeft = result - 2;
      this.intervalHandler = setInterval(() => {
        this.timeLeft -= 1
        if(this.timeLeft <= 0)
          clearInterval(this.intervalHandler)
      }, 1000)
    })
  }
}






