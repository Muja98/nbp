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
  profileImage:string;
  constructor(private router:Router, private service: StudentService) {
    this.user = new Student();
   }

  ngOnInit(): void {
  }

  public base64textString = [];

  onUploadChange(evt: any) {
    this.base64textString = [];
    const file = evt.target.files[0];
  
    if (file) {
      const reader = new FileReader();
  
      reader.onload = this.handleReaderLoaded.bind(this);
      reader.readAsBinaryString(file);
    }
  }
  
  handleReaderLoaded(e) {
    let pomNiz = [];
    pomNiz.push(btoa(e.target.result));
    this.base64textString.push('data:image/png;base64,' + btoa(e.target.result));
    this.profileImage = this.base64textString[0];
    this.user.student.profilePicturePath = pomNiz[0];
  }




  handleLoginRedirect():void{
    this.router.navigate(['/login'])
  }

  handleDashboardRedirect():void
  {
    if(this.password===this.rpassword)
    {
      this.service.addNewStudent(this.user,this.password).subscribe(result => {
        if(result['value'] == "Email taken")
          alert("An account with this email already exists")
        else {
          var token:any = { accessToken: result['value'] }
          this.service.geStudentFromToken(token)
          this.router.navigate(['/dashboard/main'])
        }
      })
    }
    else alert("Password don't match!")
   
   
  }

}
