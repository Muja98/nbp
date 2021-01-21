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
    this.service.loginStudent(this.email,this.password).subscribe(result => {
      if(result['body']['value'] == "Wrong password")
        alert("Wrong password!")
      else if (result['body']['value'] == "Non-existent email")
        alert("There is no account registered under the entered email!")
      else {
        var token:any = { accessToken: result['body']['value'] }
        this.service.geStudentFromToken(token)
        this.router.navigate(['/dashboard/main'])
      }
    });
  }

}
