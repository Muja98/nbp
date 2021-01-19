import { HttpParams } from '@angular/common/http';
import { Component, OnInit } from '@angular/core';
import { Observable } from 'rxjs';
import { Student } from 'src/app/Model/student';
import { MessageService } from 'src/app/Service/message.service';
import { StudentService } from 'src/app/Service/student.service';

@Component({
  selector: 'app-search-users',
  templateUrl: './search-users.component.html',
  styleUrls: ['./search-users.component.css']
})
export class SearchUsersComponent implements OnInit {
  users:Student[];
  usersToShow:Student[];
  public userFirstName:string = "";
  public userLastName:string = "";
  public orderByName:boolean = false;
  public orderDescending:boolean = false;
  public page:number;
  public previousPage:number;
  public fullNumberOfUsers:number;
  public perPage:number;
  public pagesVisited:number;
  private userId:string;
  private inChatWith:number[];

  friendRequests: Observable<number[]>; 

  constructor(private studentService:StudentService, private messageService:MessageService) {

   }




  ngOnInit(): void {
    this.userId = JSON.parse(localStorage.getItem('user'))['id'];
    this.page = 1;
    this.previousPage = 1;
    this.perPage = 5;
    this.pagesVisited = 1;
    let params = new HttpParams().set('user', this.userId);
    this.getUsersCount(params);
    params = params.set('from', "0").set('to', String(this.perPage));
    this.getUsers(params, false);
    this.getIdsStudentsInChatWith();
  }

  handleSearch():void {
    let params = new HttpParams().set('user', this.userId).set('firstName', this.userFirstName).set('lastName', this.userLastName);
    this.getUsersCount(params);
    params = params
      .set('from', "0").set('to', String(this.perPage))
      .set('orderByName',  String(this.orderByName))
      .set('descending', String(this.orderDescending));
    this.getUsers(params, false);
  }

  handlePageChange():void {
    if(this.page > this.previousPage && this.page > this.pagesVisited) {
      const params = new HttpParams()
      .set('firstName', this.userFirstName).set('lastName', this.userLastName)
      .set('from', String(this.previousPage * this.perPage)).set('to', String((this.page - this.previousPage) * this.perPage))
      .set('orderByName',  String(this.orderByName))
      .set('descending', String(this.orderDescending))
      .set('user', this.userId);
      this.getUsers(params, true)
    }
    else {
      const startInd = (this.page - 1) * this.perPage;
      this.usersToShow = this.users.slice(startInd, startInd + this.perPage);
    }
    this.previousPage = this.page;
    if(this.pagesVisited < this.page)
      this.pagesVisited = this.page;
  }

  isLoaded(): boolean {
    if((!this.usersToShow) || (!this.users) || (!this.inChatWith))
      return false;
    if(this.users.length < ((this.page - 1) * this.perPage + 1))
      return false;
    return true;
  }

  canChatWith(user:Student): boolean {
    if(this.inChatWith.includes(user.id))
      return false;
    return true;
  }

  private getUsers(params:any, append:boolean):void {
    this.studentService.getFilteredStudents(params).subscribe(
      result => {
        this.users = append ? this.users.concat(result) : result
        const startInd = (this.page - 1) * this.perPage;
        this.usersToShow = this.users.slice(startInd, startInd + this.perPage);
      }
    )
  }

  private getUsersCount(params:any):void {
    this.studentService.getFilteredStudentsCount(params).subscribe(
      result => this.fullNumberOfUsers = parseInt(String(result))
    );
  }

  private getIdsStudentsInChatWith() {
    this.messageService.getIdsStudentsInChatWith(parseInt(this.userId)).subscribe(
      result => this.inChatWith = result
    )
  }
}
