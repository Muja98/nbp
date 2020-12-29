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
    if(
        this.student.firstName === this.pomStudent.firstName     &&
        this.student.lastName === this.pomStudent.lastName       &&
        this.student.dateOfBirth === this.pomStudent.dateOfBirth &&
        this.student.email === this.pomStudent.email             &&
        this.student.userName === this.pomStudent.userName
      )
    {
      this.studentChangeFlag = false
    }
 
    else {this.studentChangeFlag = true};
   
  }

  handleEditStudent()
  {
    //TODO: implement handleEditStudent
  }

  
  ngOnInit(): void {
    this.student = new Student();
    this.pomStudent = new Student();
    this.handleSetStudent();
    
    this.pomStudent.firstName = this.student.firstName;
    this.pomStudent.lastName = this.student.lastName;
    this.pomStudent.email = this.student.email;
    this.pomStudent.dateOfBirth = this.student.dateOfBirth;
    this.pomStudent.email = this.student.email;
    this.pomStudent.userName = this.student.userName
  }


  
}
