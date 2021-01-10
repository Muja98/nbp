import { HttpParams } from '@angular/common/http';
import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { Group } from '../../../Model/group';
import { GroupService } from '../../../Service/group.service';

@Component({
  selector: 'app-main-page',
  templateUrl: './main-page.component.html',
  styleUrls: ['./main-page.component.css']
})
export class MainPageComponent implements OnInit {
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
  private userId:string;

  constructor(private service: GroupService, private router:Router) {}

  ngOnInit(): void {
    this.userId = JSON.parse(localStorage.getItem('user'))['id'];
    this.page = 1;
    this.previousPage = 1;
    this.perPage = 5;
    this.pagesVisited = 1;
    let params = new HttpParams().set('user', this.userId);
    this.getGroupsCount(params);
    params = params.set('from', "0").set('to', String(this.perPage));
    this.getGroups(params, false);
  }

  handleSearch():void {
    let params = new HttpParams().set('user', this.userId).set('name', this.groupName).set('field', this.groupField);
    this.getGroupsCount(params);
    params = params
      .set('from', "0").set('to', String(this.perPage))
      .set('orderByName',  String(this.orderByName))
      .set('descending', String(this.orderDescending));
    this.getGroups(params, false);
  }

  handlePageChange():void {
    if(this.page > this.previousPage && this.page > this.pagesVisited) {
      const params = new HttpParams()
      .set('name', this.groupName).set('field', this.groupField)
      .set('from', String(this.previousPage * this.perPage)).set('to', String((this.page - this.previousPage) * this.perPage))
      .set('orderByName',  String(this.orderByName))
      .set('descending', String(this.orderDescending))
      .set('user', this.userId);
      this.getGroups(params, true)
    }
    else {
      const startInd = (this.page - 1) * this.perPage;
      this.groupsToShow = this.groups.slice(startInd, startInd + this.perPage);
    }
    this.previousPage = this.page;
    if(this.pagesVisited < this.page)
      this.pagesVisited = this.page;
  }

  private getGroups(params:any, append:boolean):void {
    this.service.getFilteredGroups(params).subscribe(
      result => {
        this.groups = append ? this.groups.concat(result['value']) : result['value']
        const startInd = (this.page - 1) * this.perPage;
        this.groupsToShow = this.groups.slice(startInd, startInd + this.perPage);
      }
    )
  }

  private getGroupsCount(params:any):void {
    this.service.getFilteredGroupsCount(params).subscribe(
      result => this.fullNumberOfGroups = result['value']
    );
  }
}
