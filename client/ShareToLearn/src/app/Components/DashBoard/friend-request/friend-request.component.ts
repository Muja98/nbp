import { request } from 'http';
import { StudentService } from 'src/app/Service/student.service';
import { FriendRequest } from 'src/app/Model/friendrequest';
import { Component, OnInit, Input, EventEmitter} from '@angular/core';
import { Output } from '@angular/core';

@Component({
  selector: 'app-friend-requests',
  templateUrl: './friend-request.component.html',
  styleUrls: ['./friend-request.component.css']
})

export class FriendRequestComponent implements OnInit {
  @Input() request:FriendRequest;
  @Input() userId:number;
  @Output() changed =  new EventEmitter<string> ();
  public imgSrc:string;
  constructor(private studentService:StudentService) { }

  ngOnInit(): void {
    console.log("OnInit method");
    if(this.request.request.profilePicturePath && !String(this.request.request.profilePicturePath).includes("data:image")
        && !String(this.request.request.profilePicturePath).includes("assets/profileDefault.png")) {
      this.imgSrc = 
        'data:image/png;base64,' + this.request.request.profilePicturePath;
    }
    else if(!this.request.request.profilePicturePath) {
      this.imgSrc = 
        "assets/profileDefault.png";
    }
  }

  handleAccept() {
    debugger
      this.studentService.acceptFriendRequest(this.request.request.id, this.userId, this.request.id)
      this.changed.emit(this.request.id);
  }
  
  handleDelete() {
    debugger
    this.studentService.deleteFriendRequest(this.userId, this.request.id)
    this.changed.emit(this.request.id);
  }
}