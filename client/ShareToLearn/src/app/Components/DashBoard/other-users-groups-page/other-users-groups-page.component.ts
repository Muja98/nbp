import { HttpParams } from '@angular/common/http';
import { Component, OnDestroy, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { Group } from '../../../Model/group';
import { GroupService } from '../../../Service/group.service';

@Component({
  selector: 'other-users-groups-page',
  templateUrl: './other-users-groups-page.component.html',
  styleUrls: ['./other-users-groups-page.component.css']
})
export class OtherUsersGroupsPageComponent implements OnInit, OnDestroy {
  groups:Group[];
  groupsToShow:Group[];
  public isCollapsed = true;
  public groupName:string = "";
  public groupField:string = "";
  public orderByName:boolean = false;
  public orderDescending:boolean = false;
  public from:string;
  public to:string;
  public page:number;
  public previousPage:number;
  public fullNumberOfGroups:number;
  public perPage:number;
  public pagesVisited:number;
  private studentId:string;
  private sub:any;

  constructor(private service: GroupService, private router:Router, private route:ActivatedRoute) {}

  ngOnInit(): void {
    this.sub = this.route.params.subscribe(routeParams => {
      debugger
      this.studentId = String(+routeParams['studentId']);
      this.service.getStudentGroups(parseInt(this.studentId)).subscribe(result => {
        this.groups = result;
      })
    })
    
  }

  ngOnDestroy(): void {
    this.sub.unsubscribe();
  }
}
