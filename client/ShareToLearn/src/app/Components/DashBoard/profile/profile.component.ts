import { Student } from './../../../Model/student';
import { Component, OnInit, OnChanges } from '@angular/core';

@Component({
  selector: 'app-profile',
  templateUrl: './profile.component.html',
  styleUrls: ['./profile.component.css']
})
export class ProfileComponent implements OnInit {

  public student:Student;
  public pomStudent:Student;
  public studentChangeFlag:boolean = false;

  constructor() {
    this.student = new Student();
    this.handleSetStudent();
    this.pomStudent = this.student;
   }
  
  handleSetStudent():void
  {
    this.student.firstName = "Stefan";
    this.student.lastName = "Stamenkovic";
    this.student.email = "stefaneli95@gmail.com";
    this.student.dateOfBirth = "1998-10-05";
    this.student.userName = "Muja98"

    //TODO:Get student from API
  }
 
  handleCheckStudent():void
  {
    if(this.student.firstName !== this.pomStudent.firstName) this.studentChangeFlag = true;
    else if(this.student.lastName !== this.pomStudent.lastName)  this.studentChangeFlag = true;
    else if(this.student.dateOfBirth !== this.pomStudent.dateOfBirth)  this.studentChangeFlag = true;
    else if(this.student.email !== this.pomStudent.email)  this.studentChangeFlag = true;
    else if(this.student.userName !== this.pomStudent.userName)  this.studentChangeFlag = true;
    else this.studentChangeFlag = true;
  }
  
  
  ngOnInit(): void {
   
  }

  OnChanges(){
    if(this.student.firstName !== this.pomStudent.firstName) this.studentChangeFlag = true;
    else if(this.student.lastName !== this.pomStudent.lastName)  this.studentChangeFlag = true;
    else if(this.student.dateOfBirth !== this.pomStudent.dateOfBirth)  this.studentChangeFlag = true;
    else if(this.student.email !== this.pomStudent.email)  this.studentChangeFlag = true;
    else if(this.student.userName !== this.pomStudent.userName)  this.studentChangeFlag = true;
    else this.studentChangeFlag = true;
  }



  
  

  klikni():void{
      alert(this.handleCheckStudent())
  }
}
