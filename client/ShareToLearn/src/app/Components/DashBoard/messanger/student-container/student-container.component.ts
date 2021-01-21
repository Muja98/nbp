import { Component, EventEmitter, Input, OnDestroy, OnInit } from '@angular/core';
import { Student } from 'src/app/Model/student';
import { MessageService } from 'src/app/Service/message.service';
import { StudentService } from 'src/app/Service/student.service';
import * as signalR from '@aspnet/signalr';
import { signalRService } from './../../../../Service/signalR.service';
import { ToastrService } from 'ngx-toastr';
import { Message } from 'src/app/Model/message';

@Component({
  selector: 'app-student-container',
  templateUrl: './student-container.component.html',
  styleUrls: ['./student-container.component.css']
})
export class StudentContainerComponent implements OnInit, OnDestroy {
  @Input() students: Student[];
  public changeStudentEvent: EventEmitter<Student> = new EventEmitter();
  public newMessageEvent: EventEmitter<Message> = new EventEmitter();
  private studentId: number;
  public chosenStudent: Student;
  private _hubConnection: signalR.HubConnection;

  constructor(private toastr:ToastrService, public signalRService: signalRService, private messageService: MessageService, private studentService:StudentService) { }
  
  ngOnDestroy(): void {
    this._hubConnection.stop().then(() => console.log("Connection stopped."))
  }


  ngOnInit(): void {
    this.studentId = this.studentService.getStudentFromStorage()['id'];
    this.messageService.getStudentsInChatWith(this.studentId).subscribe(result => {
      this.students = result
      this.chosenStudent = result[0]
    })

    this._hubConnection = new signalR.HubConnectionBuilder()
    .withUrl("https://localhost:44374/chat", )
    .build()

    this._hubConnection
      .start()
      .then(() => {
        console.log('Connection started! :)')
        this.joinRoom()
      })
      .catch(err => console.log('Error while establishing connection :('));

    this._hubConnection.on('ReceiveMessage', (newMessage:any) => {
      if(this.chosenStudent.id != newMessage.senderId)
        this.toastr.info(newMessage['content'], "New message from " + newMessage['sender']);
      else
        this.newMessageEvent.emit(newMessage);
    });
  }

  public imgSrc(picturePath:string):string {
    if(picturePath)
    {
      if(picturePath.includes('data:image/png;base64,') == true)
        return picturePath;
      else
        return 'data:image/png;base64,' + picturePath;
    }
    else
      return "assets/profileDefault.png";
  }

  public changeChatStudent(student:Student) {
    this.chosenStudent = student;
    this.changeStudentEvent.emit(this.chosenStudent);
  }

  joinRoom()
  {
    const channelName = "channel:" + this.studentId;
    console.log(channelName);
    this._hubConnection.invoke("JoinRoom", channelName).catch((err)=>{
      console.log(err)
    })
  }

}
