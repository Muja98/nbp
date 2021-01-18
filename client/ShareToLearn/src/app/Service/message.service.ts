import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import URL from 'src/API/api';
import { Message } from '../Model/message';
import { Student } from '../Model/student';

@Injectable({
  providedIn: 'root'
})
export class MessageService {

  constructor(private http:HttpClient) { }

  getIdsStudentsInChatWith(studentId:number) {
    return this.http.get<number[]>(URL + "/api/messages/chat-ids/student/" + studentId);
  }

  getStudentsInChatWith(studentId:number) {
    return this.http.get<Student[]>(URL + "/api/messages/chats/student/" + studentId);
  }

  startChat(conversation:any) {
    return this.http.post(URL + "/api/messages/add-conversation/temp", conversation);
  }

  getMessagePortion(params:any) {
    return this.http.get<Message[]>(URL + "/api/messages/receive", {'params': params})
  }
}
