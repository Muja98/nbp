import { Injectable } from '@angular/core';
import URL from '../../API/api';
import {HttpClient} from '@angular/common/http';


@Injectable({
  providedIn: 'root'
})


export class StudentService {

  constructor(private http:HttpClient){}

  //slobodno menjaj imena funkcijama
  addNewStudent(userName:string, email:string, password:string)
  {
    //TODO IMPLEMENT addNewStudent
    //this.http.post(URL+'/habbit',{userName:userName, email:email, password:password} ).subscribe( response=>{} )
  }

  loginStudent(email:string, password:string)
  {
    //TODO IMPLEMENT loginStudent
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