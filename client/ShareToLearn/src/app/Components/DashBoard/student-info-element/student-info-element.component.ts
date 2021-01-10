import { Component, OnInit, Input } from '@angular/core';
import { Student } from 'src/app/Model/student';

@Component({
  selector: 'app-student-info-element',
  templateUrl: './student-info-element.component.html',
  styleUrls: ['./student-info-element.component.css']
})
export class StudentInfoElementComponent implements OnInit {
  @Input() studentObject:Student;
  public student:any;

  constructor() { }

  ngOnInit(): void {
    debugger
    this.student = this.studentObject.student;
    if(this.student.profilePicturePath) {
      this.student.profilePicturePath = 'data:image/png;base64,' + this.student.profilePicturePath;
    }
    else {
      this.student.profilePicturePath = "assets/profileDefault.png"
    }
  }

}
