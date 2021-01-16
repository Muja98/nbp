import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import URL from 'src/API/api';

@Injectable({
  providedIn: 'root'
})
export class MessageService {

  constructor(private http:HttpClient) { }

  getIdsStudentsInChatWith(studentId:number) {
    return this.http.get<number[]>(URL + "/api/messages/chat-ids/student/" + studentId);
  }
}
