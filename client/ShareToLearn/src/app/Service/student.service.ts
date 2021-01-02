import { Student } from './../Model/student';
import { Injectable } from '@angular/core';
import URL from '../../API/api';
import {HttpClient} from '@angular/common/http';
import {JwtHelperService} from '@auth0/angular-jwt';
import { Router } from '@angular/router';


@Injectable({
  providedIn: 'root'
})


export class StudentService {

  constructor(private http:HttpClient, private router:Router){}

  addNewStudent(Student:Student, Password:string)
  {
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
    this.http.put(URL+ "/api/student/"+student.id, student ).subscribe((el:any)=>console.log(el))
  }



}