import { Post } from './../../../Model/post';
import { Student } from './../../../Model/student';
import { StudentService } from './../../../Service/student.service';
import { Component, OnInit } from '@angular/core';

@Component({
  selector: 'app-post',
  templateUrl: './post.component.html',
  styleUrls: ['./post.component.css']
})
export class PostComponent implements OnInit {

  constructor(private authService: StudentService) { }
  public tempStudent:Student;
  public student:Student;
  public postArray: Array<Post> = [];

  ngOnInit(): void {
    this.student= this.authService.getStudentFromStorage();
  }

 

  povecaj(e)
  {
    e.target.style.height = "0px";
    e.target.style.height = (e.target.scrollHeight + 10)+"px";
  }

}
