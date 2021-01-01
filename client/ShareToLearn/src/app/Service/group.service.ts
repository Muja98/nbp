import { Injectable } from '@angular/core';
import URL from '../../API/api';
import {HttpClient} from '@angular/common/http';
import {JwtHelperService} from '@auth0/angular-jwt';
import { Router } from '@angular/router';
import { Group } from '../Model/group';

@Injectable({
  providedIn: 'root'
})

export class GroupService {

  constructor(private http:HttpClient, private router:Router) { }

  getFilteredGroups(params:any) {
    return this.http.get<Group[]>(URL + "/api/groups", {'params': params});
  }

  getFilteredGroupsCount(params:any) {
    return this.http.get(URL + "/api/groups/group-count", {'params': params})
  }
}
