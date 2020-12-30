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

  addNewStudent(userName:string, email:string, password:string)
  {
    this.http.post(URL + '/api/student',{FirstName:userName, LastName:userName, Email:email, Password:password}, {observe: 'response'} ).subscribe( 
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
        //mozda prepraviti na backend-u da se vraca 200 i kad se unese nepostojeci mail ili pogresna sifra, samo sa nekom porukom o tome
        if(response['status'] == 200) {
          var token:any = { accessToken: response['body']['value'] }
          this.geStudentFromToken(token)
          this.router.navigate(['/dashboard/main'])
        }
      }
    )
  }

  geStudentFromToken(token:any)
  {
    const helper = new JwtHelperService()
    const decodedToken = helper.decodeToken(token['accessToken'])
    localStorage.setItem('user', JSON.stringify(decodedToken))
  }

//PRIMER POST-a
//   addnewHabbit(newHabbit)
//   { 
//       this.http.post(URL+'/habbit', newHabbit).subscribe( response=>{} )
//   }

//PRIMER GET-a
// getAllHabbitByUserId(id)
// {
//       return this.http.get(URL+"/habbit?userId="+id)
// }

// PRIMER PUT-a
// editHabbit(id,habbit)
// {
//   this.http.put(URL+'/habbit/'+id,habbit).subscribe()
// }

//PRIMER PATCH-a
// updateWeekInHabbit(id,week,date)
// {
//   if(date==="")
//   {
//     this.http.patch(URL+'/habbit/'+id,{Week: week}).subscribe(x=>{console.log(x)})
//   }
//   else
//   {
//     this.http.patch(URL+'/habbit/'+id,{Week: week, WeekDay: date}).subscribe(x=>{console.log(x)})
//   }
// }

//PRIMER DELETE-a
// deleteHabbit(id)
// {
//     this.http.delete(URL+'/habbit/'+id).subscribe(x=>{})
// }

}