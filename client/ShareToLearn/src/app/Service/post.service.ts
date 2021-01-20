import { Comment } from './../Model/comment';
import { Post } from './../Model/post';
import { Injectable } from '@angular/core';
import URL from '../../API/api';
import {HttpClient} from '@angular/common/http';

@Injectable({
  providedIn: 'root'
})

export class PostService {

  constructor(private http:HttpClient) { }

  getAllPosts(groupID:any) {
    return this.http.get<Post[]>(URL + "/api/posts/"+groupID+"/posts");
  }

  createPost(groupID:any, studentId:any, content, date)
  {
    return this.http.post<Post>(URL + "/api/posts/"+groupID+"/student/"+studentId, {Content:content, DateOfPublishing:date});
  }

  getAllComment(postId:any)
  {
    return this.http.get<Comment[]>(URL + "/api/posts/"+postId+"/comments");
  }

  createComment(postId:any, studentId:any, content)
  {
    return this.http.post(URL + "/api/posts/"+ postId +"/student/"+studentId+"/newComment",{Content:content});
  }

  deleteComment(commentId:any)
  {
    this.http.delete(URL + "/api/posts/comment/"+commentId+"/delete").subscribe(()=>{});
  }

  updateComment(commentId:any, comment:any)
  {
    this.http.put(URL + "/api/posts/comment/"+commentId+"/edit",{Content:comment}).subscribe(()=>{});
  }

  updatePost(postId:any, postContent:any)
  {
    this.http.put(URL + "/api/posts/"+postId+"/edit",{Content:postContent, DateOfPublishing:"1998-05-10"}).subscribe(()=>{});
  }

  deletePost(postId:any)
  {
    this.http.delete(URL+"/api/posts/"+postId+"/delete").subscribe(()=>{});
  }

}
