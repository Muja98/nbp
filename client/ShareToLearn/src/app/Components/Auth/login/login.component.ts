import { Component, OnInit } from '@angular/core';
import {Router} from '@angular/router';
import { StudentService } from './../../../Service/student.service';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.css']
})
export class LoginComponent implements OnInit {

  constructor(private router:Router,private service: StudentService) { }

  public email:string;
  public password:string;

  ngOnInit(): void {
   
  }

  handleRegisterRedirect():void
  {
    this.router.navigate(['/register'])
  }

  handleDashboardRedirect():void
  {
    //Svi podaci kad uneses nesto u input se sistematski upisuju u promenljive
    //ovde se zove servise, a u folder Service se nalazi sama implementacija
    this.service.loginStudent(this.email,this.password);
    //this.router.navigate(['/dashboard/main'])
  }

}
