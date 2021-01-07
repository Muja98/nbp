import { GroupStatistics } from './../../../Model/groupstatistics';
import { Component, Input, OnInit } from '@angular/core';
import URL from 'src/API/api'
import {HttpClient} from '@angular/common/http';
import { Router } from '@angular/router';

@Component({
    selector: 'group-info',
    templateUrl: './group-info.component.html'
})

export class GroupInfoComponent implements OnInit
{
    @Input()groupId: number;
    group: GroupStatistics;

    constructor(private http:HttpClient, private router:Router) { }

    getGroupStatistics()
    {
        return this.http.get<GroupStatistics>(URL + "/api/groups/" + this.groupId + "/statistics");
    }

    ngOnInit(): void {
        console.log("ngOnInit method");
        this.getGroupStatistics().subscribe(
            result => {
            debugger
            console.log(result);
            this.group =  result
              
            }
         )
    }
}