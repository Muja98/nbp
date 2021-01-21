import { HttpParams } from '@angular/common/http';
import { Component, OnInit } from '@angular/core';
import { Student } from 'src/app/Model/student';
import { MessageService } from 'src/app/Service/message.service';
import { StudentService } from 'src/app/Service/student.service';

@Component({
    selector: 'my-friends',
    templateUrl: './my-friends.component.html',
  })

export class MyFriendsComponent implements OnInit {
    friends:Student[];
    friendsToShow:Student[];
    filterFirstName:string = "";
    filterLastName:string = "";
    public orderByName:boolean = false;
    public orderDescending:boolean = false;
    public page:number;
    public previousPage:number;
    public pagesVisited:number;
    public perPage:number;
    private userId:string;
    public fullNumberOfFriends:number;
    private inChatWith:number[];

    constructor(private studentService:StudentService, private messageService:MessageService) { }

    ngOnInit(): void {
        console.log("onInit method");

        this.userId = JSON.parse(localStorage.getItem('user'))['id'];
        this.page = 1;
        this.previousPage = 1;
        this.perPage = 10;
        this.pagesVisited = 1;

        //vracanje liste svih prijatelja
        let params = new HttpParams().set('user', this.userId);
        this.getFriendsCount(params);
        params = params.set('from', "0").set('to', String(this.perPage));
        this.getFriends(params, false);
        this.getIdsStudentsInChatWith();
    }

    public getFriends(params:any, append:boolean): void {
        debugger
        this.studentService.getFilteredFriends(params).subscribe(
            result => {
              this.friends = append ? this.friends.concat(result) : result
              const startInd = (this.page - 1) * this.perPage;
              this.friendsToShow = this.friends.slice(startInd, startInd + this.perPage);
            }
        )
    }

    public getFriendsCount(params:any):void {
        debugger
        this.studentService.getFilteredFriendsCount(params).subscribe(
          result => this.fullNumberOfFriends = parseInt(String(result))
        );
    }

    private getIdsStudentsInChatWith() {
      this.messageService.getIdsStudentsInChatWith(parseInt(this.userId)).subscribe(
        result => this.inChatWith = result
      )
    }

    isLoaded(): boolean {
      if((!this.friendsToShow) || (!this.friends) || (!this.inChatWith))
        return false;
      if(this.friends.length < ((this.page - 1) * this.perPage + 1) && (this.friends.length > 0))
        return false;
      return true;
    }

    canChatWith(user:Student): boolean {
      if(this.inChatWith.includes(user.id))
        return false;
      return true;
    }

    handlePageChange():void {
        debugger
        if(this.page > this.previousPage && this.page > this.pagesVisited) {
          const params = new HttpParams()
          .set('firstName', this.filterFirstName).set('lastName', this.filterLastName)
          .set('from', String(this.previousPage * this.perPage)).set('to', String((this.page - this.previousPage) * this.perPage))
          .set('orderByName',  String(this.orderByName))
          .set('descending', String(this.orderDescending))
          .set('user', this.userId);
          this.getFriends(params, true)
        }
        else {
          const startInd = (this.page - 1) * this.perPage;
          this.friendsToShow = this.friends.slice(startInd, startInd + this.perPage);
        }

        this.previousPage = this.page;
        if(this.pagesVisited < this.page)
          this.pagesVisited = this.page;
    }

    handleSearch():void {
        debugger
        let params = new HttpParams().set('user', this.userId).set('firstName', this.filterFirstName).set('lastName', this.filterLastName);
        this.getFriendsCount(params);
        params = params
          .set('from', "0").set('to', String(this.perPage))
          .set('orderByName',  String(this.orderByName))
          .set('descending', String(this.orderDescending));
        this.getFriends(params, false);
      }
}
