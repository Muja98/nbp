import { Component, OnInit, Input } from '@angular/core';
import { Router } from '@angular/router';
import { Student } from 'src/app/Model/student';
import { StudentService } from 'src/app/Service/student.service';
import { DomSanitizer } from '@angular/platform-browser'
import { MessageService } from 'src/app/Service/message.service';

@Component({
  selector: 'app-student-info-element',
  templateUrl: './student-info-element.component.html',
  styleUrls: ['./student-info-element.component.css']
})
export class StudentInfoElementComponent implements OnInit {
  @Input() studentObject:Student;
  @Input() canChatWith:boolean;
  public student:any;
  public safeImgUrl:string;
  public firstMessage:string;
  public imgSrc:string;

  constructor(private router:Router, private studentService:StudentService, private sanitizer:DomSanitizer, private messageService:MessageService) { }

  ngOnInit(): void {
    this.student = this.studentObject.student;
    if(this.student.profilePicturePath && !String(this.student.profilePicturePath).includes("data:image")
        && !String(this.student.profilePicturePath).includes("assets/profileDefault.png")) {
      this.imgSrc = 
        'data:image/png;base64,' + this.student.profilePicturePath;
    }
    else if(!this.student.profilePicturePath) {
      this.imgSrc = 
        "assets/profileDefault.png";
    }
  }

  handleViewStudentProfile(): void {
    let routePart;
    let storageStudentId = this.studentService.getStudentFromStorage()['id'];
    routePart = (storageStudentId == this.studentObject.id) ? "" : ("/" + this.studentObject.id)
    this.router.navigate(["dashboard/profile" + routePart])
  }

  handleStartChat(): void {
    console.log(this.firstMessage)
    const studentFromStorage = this.studentService.getStudentFromStorage();
    const sender = new Student();
    sender.id = parseInt(studentFromStorage['id']);
    sender.isFriend = true;
    sender.student = {
      firstName: studentFromStorage['firstName'],
      lastName: studentFromStorage['lastName'],
      dateOfBirth: new Date(studentFromStorage['dateOfBirth']),
      email: studentFromStorage['email'],
      profilePicturePath: String(studentFromStorage['profilePicturePath'])
    }
    
    const receiver = this.studentObject;
    this.messageService.startChat(this.firstMessage, {'sender': sender, 'receiver': receiver}).subscribe(result => {
      this.router.navigate(["/dashboard/messanger"])
    });
  }
}
