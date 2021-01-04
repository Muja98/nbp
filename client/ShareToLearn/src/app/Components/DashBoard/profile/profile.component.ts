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
  public dateOfBirth:Date;
  public imgSrc:string;

  constructor(private service:StudentService,private router:Router) {
    
   }
  
  handleSetStudent():void
  {
    this.tempStudent = this.service.getStudentFromStorage();
    this.student.student.firstName = this.tempStudent.firstName;
    if(this.student.student.firstName === this.tempStudent.lastName)
      this.student.student.lastName = "";
    else
      this.student.student.lastName = this.tempStudent.lastName
    this.student.student.email = this.tempStudent.email;
    this.student.student.profilePicturePath = this.tempStudent.profilePicturePath;
    this.imgSrc = "data:image/jpeg;base64," + this.student.student.profilePicturePath;
    this.student.student.dateOfBirth = (new Date(this.tempStudent.dateOfBirth)).toDateString();
    this.dateOfBirth = new Date(this.student.student.dateOfBirth);

    //TODO:Get student from API
  }

 
  handleCheckStudent():void
  {
    this.student.student.dateOfBirth = (new Date(this.dateOfBirth)).toDateString();
    if(
        this.student.student.firstName === this.pomStudent.student.firstName     &&
        this.student.student.lastName === this.pomStudent.student.lastName       &&
        this.student.student.dateOfBirth === this.pomStudent.student.dateOfBirth &&
        this.student.student.email === this.pomStudent.student.email             
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
    
    this.pomStudent.student.firstName = this.student.student.firstName;
    this.pomStudent.student.lastName = this.student.student.lastName;
    this.pomStudent.student.email = this.student.student.email;
    this.pomStudent.student.dateOfBirth = this.student.student.dateOfBirth;
  }


  
}
