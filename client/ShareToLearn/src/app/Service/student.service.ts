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

    this.http.post(URL + '/api/student',{Student, Password}, {observe: 'response'} ).subscribe( 
      response => {
        //mozda prepraviti na backend-u da se vraca 200 i kad se unese vec postojeci mail, samo sa nekom porukom o tome
        if(response['status'] == 200) {
          var token:any = { accessToken: response['body']['value'] }
          this.geStudentFromToken(token)
          this.router.navigate(['/dashboard/main'])
        }
      }
    )
  }

  loginStudent(email:string, password:string)
  {
    this.http.post(URL + '/api/student/login', {Email: email, Password: password}, {observe: 'response'}).subscribe(
      response => {
        console.log(response)
        //mozda prepraviti na backend-u da se vraca 200 i kad se unese nepostojeci mail ili pogresna sifra, samo sa nekom porukom o tome
        if(response['status'] == 200) {
          var token:any = { accessToken: response['body']['value'] }
          this.geStudentFromToken(token)
          this.router.navigate(['/dashboard/main'])
        }
      }
    )
  }

  logoutStudent()
  {
    localStorage.removeItem('user');
  }

  geStudentFromToken(token:any)
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

  getSpecificStudent(studentId:number) {
    return this.http.get<Student>(URL + "/api/student/student/" + studentId);
  }

  getFriendRequests(studentId:number) {
    return this.http.get<FriendRequest[]>(URL + "/api/student/friend_request/receiver/" + studentId);
  }
  acceptFriendRequest(senderId:number, receiverId:number, requestId:string) {
    this.http.post(URL + "/api/student/friendship/sender/" + senderId + "/receiver/"+ receiverId + "/request/" + requestId, {}).subscribe(()=>{});
  }
  deleteFriendRequest(receiverId:number, requestId:string) {
    this.http.delete(URL + "/api/student/friend_request/receiver/" + receiverId + "/request/" + requestId).subscribe(()=>{});
  }
}