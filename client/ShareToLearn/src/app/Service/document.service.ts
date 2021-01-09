import { Injectable } from '@angular/core';
import URL from '../../API/api';
import {HttpClient} from '@angular/common/http';
import {JwtHelperService} from '@auth0/angular-jwt';
import { Router } from '@angular/router';
import { Group } from '../Model/group';

@Injectable({
  providedIn: 'root'
})

export class DocumentService {

  constructor(private http:HttpClient, private router:Router) { }

  getDocuments(groupId:number, level:string, substring:string ) {
    console.log("groupid "+groupId+ " level "+level+" substring "+substring)
    return this.http.get<Document[]>(URL + "/api/documents/group/"+groupId+"?level="+level+"&substring="+substring);
  }

  getDocument(documentId:any)
  {
    return this.http.get<any>(URL + "/api/documents/"+documentId);
  }

  createDocument(groupId:number, studentId:number, document:any) {

     this.http.post(URL + "/api/documents/creator/"+studentId+"/group/"+groupId, {
      Name: document.Name,
      Level: document.Level,
      Description: document.Description,
      DocumentPath:document.DocumentPath
     }).subscribe(()=>{});

  }





}
