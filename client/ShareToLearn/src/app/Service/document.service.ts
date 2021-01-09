import { Document } from './../Model/document';
import { Injectable } from '@angular/core';
import URL from '../../API/api';
import {HttpClient} from '@angular/common/http';
import { Router } from '@angular/router';

@Injectable({
  providedIn: 'root'
})

export class DocumentService {

  constructor(private http:HttpClient, private router:Router) { }

  getDocuments(groupId:number) {
    return this.http.get<Document[]>(URL + "/api/documents/group/"+groupId);
  }

  getDocument(path:string)
  {
    //return this.http.get<string>(URL + "/api/documents/from_path", {'path':path});
    return this.http.get<string>(URL + "/api/student", {'path': path});
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
