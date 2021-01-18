import { Injectable } from '@angular/core';
import URL from '../../API/api';
import {HttpClient} from '@angular/common/http';
import {JwtHelperService} from '@auth0/angular-jwt';
import { Router } from '@angular/router';
import { Group } from '../Model/group';
import { Student } from '../Model/student';

@Injectable({
  providedIn: 'root'
})

export class GroupService {

  constructor(private http:HttpClient, private router:Router) { }

  getFilteredGroups(params:any) {
    return this.http.get<Group[]>(URL + "/api/groups", {'params': params});
  }

  getFilteredGroupsCount(params:any) {
    return this.http.get(URL + "/api/groups/group-count", {'params': params});
  }

  createGroup(ownerId:string, name:string, field:string, description:string, groupPicturePath:string) {
    console.log(groupPicturePath)
    return this.http.post(URL + "/api/groups/" + ownerId, {Name: name, Field: field, Description: description, GroupPicturePath: groupPicturePath});
  }

  getGroupMembers(groupId:number, requesterId:number) {
    return this.http.get<Student[]>(URL + "/api/groups/" + groupId + "/members/requester/" + requesterId);
  }

  getGroupOwner(groupId:number, requesterId:number) {
    return this.http.get<Student>(URL + "/api/groups/" + groupId + "/owner/requester/" + requesterId);
  }

  joinGroup(studentId:number, groupId:number) {
    return this.http.post(URL + "/api/groups/student/" + studentId + "/group/" + groupId, null);
  }

  leaveGroup(studentId:number, groupId:number) {
    return this.http.delete(URL + "/api/groups/delete/student/" + studentId + "/group/" + groupId);
  }

  getStudentGroupRelationship(studentId:number, groupId:number) {
    return this.http.get<any>(URL + "/api/groups/relationship/student/" + studentId + "/group/" + groupId);
  }

  editGroup(groupId:number, group:any)
  {
    this.http.put(URL+"/api/groups/"+groupId, {
                                                Field: group.Field,
                                                Name: group.Name,
                                                Description: group.Description,
                                                GroupPicturePath: group.GroupPicturePath
                                              }).subscribe(()=>{});
  }

  getDocumentImage(groupId:number)
  {
    return this.http.get(URL+"/api/groups/"+groupId+"/groupImage")
  }
  getStudentGroups(studentId:number) {
    return this.http.get<Group[]>(URL + "/api/groups/student/" + studentId + "/groups");
  }
}
