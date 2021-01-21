import { PostService } from './../../../Service/post.service';
import { StudentService } from './../../../Service/student.service';
import { Student } from './../../../Model/student';
import { Post } from './../../../Model/post';
import { Component, OnInit } from '@angular/core';
import { formatDate } from '@angular/common';
import { ActivatedRoute } from '@angular/router'
import { ThrowStmt } from '@angular/compiler';
import { parse } from 'path';
@Component({
  selector: 'app-group',
  templateUrl: './group.component.html',
  styleUrls: ['./group.component.css']
})

export class GroupComponent implements OnInit {

  today= new Date();
  todaysDataTime = '';
  postText:string="";
  student = new Student();
  tempStudent: any;
  postArray: Array<Post> = [];
  profileImage:string;
  constructor(private service:StudentService, private postService: PostService, private aroute:ActivatedRoute) { 
    this.todaysDataTime = formatDate(this.today, 'yyyy-MM-dd', 'en-SR', 'GMT+1');

  }

  handleAddNewPost()
  {
    if(this.postText === "")return;
    this.aroute.paramMap.subscribe(params=>{
      this.postService.createPost(params.get('idGroup'), this.tempStudent.id, this.postText, this.todaysDataTime).subscribe((newPost:Post)=>{
        this.postArray.reverse();
        this.postArray.push(newPost)
        this.postArray.reverse();
      });
    })
    this.postText = "";
    //window.location.reload();
  }


  handleDeletePostFromArray(index: string)
  {
    this.postArray.splice(parseInt(index),1);
  }

  povecaj(e)
  {
    e.target.style.height = "0px";
    e.target.style.height = (e.target.scrollHeight + 10)+"px";
  }

  ngOnInit(): void {
      this.tempStudent = this.service.getStudentFromStorage();
     
        this.profileImage ="data:image/png;base64,"+this.tempStudent.profilePicturePath;
      this.student.student.firstName = this.tempStudent.firstName;
      this.student.student.lastName = this.tempStudent.lastName;
      this.aroute.paramMap.subscribe(params=>{
        if(params.get('idGroup')===null){return}
        this.postService.getAllPosts(parseInt(params.get('idGroup'))).subscribe((posts:Post[])=>{
          this.postArray = posts;
        })})
      
  }

}
