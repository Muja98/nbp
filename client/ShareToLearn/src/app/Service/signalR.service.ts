import { Message } from './../Model/message';
import { Injectable } from '@angular/core';
import URL from '../../API/api';
import {HttpClient} from '@angular/common/http';
import * as signalR from '@aspnet/signalr';

@Injectable({
  providedIn: 'root'
})

export class signalRService {

  public hubConnection: signalR.HubConnection;
  
  constructor(private http:HttpClient) {

    this.startConnection();
   }
 
  public startConnection =()=>{
      this.hubConnection = new signalR.HubConnectionBuilder()
      .withUrl(URL+"/chat")
      .build()

      this.hubConnection
      .start()
      .then(()=> console.log('Connection started'))
      .catch(err => console.log('Error while starting connection: '+err))

      
  }

  public sendMessage(mess:Message): void {
    this.hubConnection
      .invoke('sendToAll', mess)
      .catch(err => console.error(err));
  }

  

}
