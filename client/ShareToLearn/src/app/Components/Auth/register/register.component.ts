import { StudentService } from './../../../Service/student.service';
import { Router } from '@angular/router';
import { Component, OnInit } from '@angular/core';

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.css']
})
export class RegisterComponent implements OnInit {

  constructor(private router:Router, private service: StudentService) { }

  ngOnInit(): void {
  }
  //SERVISI SE UGLAVNOM PISU U POSEBAN FOLDER ALI ZARAD TESTIRANJA OVDE CE BUDU PRIVREMENO

  public userName:string;
  public email:string;
  public password:string;
  public rPassword:string;

  handleLoginRedirect():void{
    this.router.navigate(['/login'])
  }

  handleDashboardRedirect():void
  {
    if(this.password===this.rPassword)
    {
      //Svi podaci kad uneses nesto u input se sistematski upisuju u promenljive
      //ovde se zove servise, a u folder Service se nalazi sama implementacija
      this.service.addNewStudent(this.userName,this.email,this.password);
    }
    else alert("Password don't match!")
   
    //this.router.navigate(['/dashboard/main'])
  }

}
