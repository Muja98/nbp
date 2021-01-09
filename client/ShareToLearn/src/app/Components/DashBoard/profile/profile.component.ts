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
  public imgSrcPom:string;

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
    this.imgSrc = 'data:image/png;base64,' + this.tempStudent.profilePicturePath;
    this.student.student.dateOfBirth = this.tempStudent.dateOfBirth;
    this.dateOfBirth = new Date(this.student.student.dateOfBirth);
    
    //TODO:Get student from API
  }

  base64textString = [];

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
    this.imgSrc=this.base64textString[0];
    this.imgSrcPom = pomNiz[0];
    this.student.student.profilePicturePath = pomNiz[0];
  }
 
  handleCheckStudent():void
  {
  
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
      this.student.id = this.tempStudent.id;
      let pom = this.dateOfBirth.toString().split(" ");

      if(this.dateOfBirth.toString().length>10)
      {
        let array = ['Jan','Feb','Mar','Apr','May','Jun','Jul','Aug','Sep','Oct','Nov','Dec']
       
        let mesec = 0;
        for(let i=0; i<array.length; i++)
        {
          if(array[i]===pom[1])
          {
            mesec = i+1;
          }
        }
        var mes = "";
        if(mesec<10)
          mes = "0";
        mes = mes+mesec;
        this.student.student.dateOfBirth = pom[3]+"-"+mes+"-"+pom[2];
      }
      else
        this.student.student.dateOfBirth = this.dateOfBirth
     
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
