import { GroupService } from './../../../Service/group.service';
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
   
    constructor(private http:HttpClient, private groupService:GroupService, private router:Router) { }

    getMemberships()
    {
        const userId = JSON.parse(localStorage.getItem('user'))['id']
        this.groupService.getMemberships(userId).subscribe(
            result => {
              debugger
              console.log(result);
              this.memberships= result
            }
         )
    }

    getOwnerships()
    {
        const userId = JSON.parse(localStorage.getItem('user'))['id']
        this.groupService.getOwnerships(userId).subscribe(
            result => {
              debugger
              console.log(result);
              this.ownerships= result
            }
        )
    }

    ngOnInit(): void {
        console.log("ngOnInit method");
        debugger
        this.getOwnerships();
        this.getMemberships();
    }
}
