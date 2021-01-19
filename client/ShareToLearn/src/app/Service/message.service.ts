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

  startChat(firstMessage:string, conversationParticipants:any) {
    return this.http.post(URL + "/api/messages/add-conversation/temp", conversationParticipants);
  }

  getMessagePortion(params:any) {
    return this.http.get<Message[]>(URL + "/api/messages/receive", {'params': params})
  }

  deleteMessage(biggerId:number, smallerId:number)
  {
     this.http.delete(URL+"/api/messages/deleteConversation/user/"+biggerId+"/"+smallerId).subscribe(()=>{});
  }
}
