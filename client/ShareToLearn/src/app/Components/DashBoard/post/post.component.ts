import { Comment } from './../../../Model/comment';
import { PostService } from './../../../Service/post.service';
import { Post } from './../../../Model/post';
import { Student } from './../../../Model/student';
import { StudentService } from './../../../Service/student.service';
import { Component, Input, OnInit, Output } from '@angular/core';
import { textChangeRangeIsUnchanged } from 'typescript';

import { EventEmitter } from '@angular/core';
@Component({
  selector: 'app-post',
  templateUrl: './post.component.html',
  styleUrls: ['./post.component.css']
})
export class PostComponent implements OnInit {

  constructor(private authService: StudentService, private postService:PostService) { }
  public tempStudent:any;
  public student:Student = new Student();
  public postArray: Array<Post> = [];
  public commentArray: Array<Comment>= [];
  public comment:string;
  public profilePicture:string;
  @Input() post: Post;
  @Input() index: number;
  @Output() postID = new EventEmitter();
  
  ngOnInit(): void {
    this.tempStudent = this.authService.getStudentFromStorage();
    this.student.student.firstName = this.tempStudent.firstName;
    this.student.student.lastName  = this.tempStudent.lastName;

    this.handleGetComment();
  }

  handleReturnImage(string:any):any
  {
    if(string && string != "")
      return "data:image/png;base64,"+string;
    else 
      return "assets/profileDefault.png";
  }

  handleGetComment()
  {
    this.postService.getAllComment(this.post.id).subscribe((comment:Comment[])=>{
      this.commentArray = comment;
    })
  }

  handleAddNewComment()
  {
    if(this.comment === ""){return;}
    this.postService.createComment(this.post.id, this.tempStudent.id, this.comment).subscribe((newComment:Comment)=>{
      this.commentArray.reverse();
      this.commentArray.push(newComment);
      this.commentArray.reverse();
    })
    this.comment = "";
    
  }

  handleDeleteComment(commentId:any, index)
  {
    this.postService.deleteComment(commentId);
    this.commentArray.splice(index,1);
  }

  handleCheckStudent(cstudent)
  {
    if(
     cstudent.firstName === this.tempStudent.firstName &&
     cstudent.lastName === this.tempStudent.lastName &&
     cstudent.email === this.tempStudent.email 
     )
      return true;
    else return false;
  }

  handleEditComment(comment:any, id:any)
  {
    if(comment === "")return;
    this.postService.updateComment(id,comment)
  }

  handleEditPost(post:any,id:any)
  {
    if(post==="")return;
    this.postService.updatePost(id,post);
  }

  handleDeletePost(id:any)
  {
    this.postService.deletePost(id);
    this.postID.emit(this.index.toString())
  }

  povecaj(e)
  {
    e.target.style.height = "0px";
    e.target.style.height = (e.target.scrollHeight + 8)+"px";
  }

 



}
