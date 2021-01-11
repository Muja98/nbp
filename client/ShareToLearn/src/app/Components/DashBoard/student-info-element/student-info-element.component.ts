import { Component, OnInit, Input } from '@angular/core';
import { Router } from '@angular/router';
import { Student } from 'src/app/Model/student';
import { StudentService } from 'src/app/Service/student.service';
import { DomSanitizer } from '@angular/platform-browser'

@Component({
  selector: 'app-student-info-element',
  templateUrl: './student-info-element.component.html',
  styleUrls: ['./student-info-element.component.css']
})
export class StudentInfoElementComponent implements OnInit {
  @Input() studentObject:Student;
  public student:any;
  public safeImgUrl:string;

  constructor(private router:Router, private service:StudentService, private sanitizer:DomSanitizer) { }

  ngOnInit(): void {
    this.student = this.studentObject.student;
    if(this.student.profilePicturePath && !String(this.student.profilePicturePath).includes("data:image")
        && !String(this.student.profilePicturePath).includes("assets/profileDefault.png")) {
      this.student.profilePicturePath = 
        this.sanitizer.bypassSecurityTrustResourceUrl('data:image/png;base64,' + this.student.profilePicturePath);
    }
    else if(!this.student.profilePicturePath) {
      this.student.profilePicturePath = 
        this.sanitizer.bypassSecurityTrustResourceUrl("assets/profileDefault.png");
    }
  }

  handleViewStudentProfile(): void {
    let routePart;
    let storageStudentId = this.service.getStudentFromStorage()['id'];
    routePart = (storageStudentId == this.studentObject.id) ? "" : ("/" + this.studentObject.id)
    this.router.navigate(["dashboard/profile" + routePart])
  }
}
