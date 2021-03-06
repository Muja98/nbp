import { FriendRequest } from 'src/app/Model/friendrequest';
import { Student } from './../Model/student';
import { Injectable } from '@angular/core';
import URL from '../../API/api';
import {HttpClient} from '@angular/common/http';
import {JwtHelperService} from '@auth0/angular-jwt';
import { Router } from '@angular/router';
import { getLocaleDateTimeFormat } from '@angular/common';
import { request } from 'http';


@Injectable({
  providedIn: 'root'
})


export class StudentService {

  constructor(private http:HttpClient, private router:Router){}
 
  addNewStudent(Studente:Student, Password:string)
  {
    let Student:any = {
      FirstName : Studente.student.firstName,
      LastName : Studente.student.lastName,
      DateOfBirth : Studente.student.dateOfBirth+"T00:00:00",
      Email : Studente.student.email,
      ProfilePicturePath : Studente.student.profilePicturePath
    }

    return this.http.post(URL + '/api/student',{Student, Password});
    // .subscribe( 
    //   response => {
    //     //mozda prepraviti na backend-u da se vraca 200 i kad se unese vec postojeci mail, samo sa nekom porukom o tome
    //     if(response['status'] == 200) {
    //       var token:any = { accessToken: response['body']['value'] }
    //       this.geStudentFromToken(token)
    //       this.router.navigate(['/dashboard/main'])
    //     }
    //   }
    // )
  }

  loginStudent(email:string, password:string)
  {
    return this.http.post(URL + '/api/student/login', {Email: email, Password: password}, {observe: 'response'})
    // .subscribe(
    //   response => {
    //     console.log(response)
    //     //mozda prepraviti na backend-u da se vraca 200 i kad se unese nepostojeci mail ili pogresna sifra, samo sa nekom porukom o tome
    //     if(response['status'] == 200) {
    //       var token:any = { accessToken: response['body']['value'] }
    //       this.geStudentFromToken(token)
    //       this.router.navigate(['/dashboard/main'])
    //     }
    //   }
    // )
  }

  logoutStudent()
  {
    localStorage.removeItem('user');
  }

  public geStudentFromToken(token:any)
  {
    const helper = new JwtHelperService()
    const decodedToken = helper.decodeToken(token['accessToken'])
    localStorage.setItem('user', JSON.stringify(decodedToken))
  }

  getStudentFromStorage()
  {
    const user = JSON.parse(localStorage.getItem('user'));
   
    if(user)
      return user;
  }


  editStudent(student:Student)
  {
   
    this.http.put(URL + "/api/student/"+student.id,{
      FirstName : student.student.firstName,
      LastName : student.student.lastName,
      DateOfBirth : student.student.dateOfBirth+"T00:00:00",
      Email : student.student.email,
      ProfilePicturePath : student.student.profilePicturePath
    }).subscribe(()=>{});
  }

  getFilteredStudents(params:any) {
    return this.http.get<Student[]>(URL + "/api/student", {'params': params});
  }

  getFilteredStudentsCount(params:any) {
    return this.http.get(URL + "/api/student/student-count", {'params': params});
  }

  getFilteredFriends(params:any) {
    return this.http.get<Student[]>(URL + "/api/student/friends", {'params': params});
  }

  getFilteredFriendsCount(params:any) {
    return this.http.get(URL + "/api/student/friend-count", {'params': params});
  }

  getSpecificStudent(studentId:number, requesterId:number) {
    return this.http.get<Student>(URL + "/api/student/" + studentId + "/requester/" + requesterId);
  }

  getFriendRequests(studentId:number) {
    return this.http.get<FriendRequest[]>(URL + "/api/student/friend_request/receiver/" + studentId);
  }
  acceptFriendRequest(senderId:number, receiverId:number, requestId:string) {
    this.http.post(URL + "/api/student/friendship/sender/" + senderId + "/receiver/"+ receiverId + "/request/" + requestId, {}).subscribe(()=>{});
  }
  deleteFriendRequest(receiverId:number, requestId:string, senderId:number) {
    this.http.delete(URL + "/api/student/friend_request/receiver/" + receiverId + "/request/" + requestId+"/sender/"+senderId).subscribe(()=>{});
  }
  sendFriendRequest(senderId:number, receiverId:number, request:FriendRequest) {
    this.http.post(URL + "/api/student/friend_request/sender/" + senderId + "/receiver/" + receiverId, 
    {id: request.request.id, firstName: request.request.firstName, lastName: request.request.lastName, email: request.request.email, profilePicturePath:request.request.profilePicturePath}).subscribe(()=>{});
  }

  GetFriendRequestSends(senderId:number)
  {
    return this.http.get<number[]>(URL+"/api/student/friend_request/"+senderId);
  }
}