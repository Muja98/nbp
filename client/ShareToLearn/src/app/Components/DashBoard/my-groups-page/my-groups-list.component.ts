import { Component, OnInit } from '@angular/core';
import { Group } from '../../../Model/group';
import URL from 'src/API/api'
import {HttpClient} from '@angular/common/http';
import { Router } from '@angular/router';

@Component({
    selector: 'my-groups',
    templateUrl: './my-groups-list.component.html'
})

export class MyGroupsComponent implements OnInit
{
    memberships:Group[];
    ownerships:Group[];
   
    constructor(private http:HttpClient, private router:Router) { }

    getMemberships()
    {
        const userId = JSON.parse(localStorage.getItem('user'))['id']
        return this.http.get<Group[]>(URL + "/api/groups/student/" + userId + "/memberships");
    }

    getOwnerships()
    {
        const userId = JSON.parse(localStorage.getItem('user'))['id']
        return this.http.get<Group[]>(URL + "/api/groups/student/" + userId + "/ownerships");
    }

    ngOnInit(): void {
        console.log("ngOnInit method");
        debugger
        this.getMemberships().subscribe(
            result => {
              debugger
              console.log(result);
              this.memberships= result
              
            }
         )

        this.getOwnerships().subscribe(
            result => {
              debugger
              console.log(result);
              this.ownerships= result
            }
        )

    }


}

