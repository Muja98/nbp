import { Router } from '@angular/router';
import { StudentService } from './../../../Service/student.service';
import { Student } from './../../../Model/student';
import { Component, OnInit } from '@angular/core';

@Component({
  selector: 'app-profile',
  templateUrl: './profile.component.html',
  styleUrls: ['./profile.component.css']
})
export class ProfileComponent implements OnInit {

  public student:Student;
  public pomStudent:Student;
  public studentChangeFlag:boolean = false;
  public tempStudent:any;

  constructor(private service:StudentService,private router:Router) {
    
   }
  
  handleSetStudent():void
  {
    this.tempStudent= this.service.getStudentFromStorage();
    this.student.id = this.tempStudent.id;
    this.student.firstName = this.tempStudent.firstName;
    if(this.student.firstName===this.student.lastName)
      this.student.lastName = "";
    else
      this.student.lastName = this.student.lastName
    this.student.email = this.tempStudent.email;
    this.student.dateOfBirth = this.tempStudent.dateOfBirth;
    this.student.userName = ""

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
      this.service.editStudent(this.student);
      this.service.logoutStudent();
      this.router.navigate(['/login'])
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
