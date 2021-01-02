import { Student } from './../../../Model/student';
import { StudentService } from './../../../Service/student.service';
import { Router } from '@angular/router';
import { Component, OnInit } from '@angular/core';

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.css']
})
export class RegisterComponent implements OnInit {

  user:Student;
  password:string;
  rpassword:string;
  constructor(private router:Router, private service: StudentService) {
    this.user = new Student();
   }

  ngOnInit(): void {
  }




  handleLoginRedirect():void{
    this.router.navigate(['/login'])
  }

  handleDashboardRedirect():void
  {
  if(this.password===this.rpassword)
  {
    this.service.addNewStudent(this.user,this.password);
    this.router.navigate(['/dashboard/main'])
  }
  else alert("Password don't match!")
   
   
  }

}
