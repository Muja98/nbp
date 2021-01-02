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
    this.student.FirstName = this.tempStudent.FirstName;
    if(this.student.FirstName===this.student.LastName)
      this.student.LastName = "";
    else
      this.student.LastName = this.student.LastName
    this.student.Email = this.tempStudent.Email;
    this.student.DateOfBirth = this.tempStudent.DateOfBirth;

    //TODO:Get student from API
  }

 
  handleCheckStudent():void
  {
    if(
        this.student.FirstName === this.pomStudent.FirstName     &&
        this.student.LastName === this.pomStudent.LastName       &&
        this.student.DateOfBirth === this.pomStudent.DateOfBirth &&
        this.student.Email === this.pomStudent.Email             
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
    
    this.pomStudent.FirstName = this.student.FirstName;
    this.pomStudent.LastName = this.student.LastName;
    this.pomStudent.Email = this.student.Email;
    this.pomStudent.DateOfBirth = this.student.DateOfBirth;
  }


  
}
