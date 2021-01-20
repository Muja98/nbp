import { Component, OnInit, Input } from '@angular/core';
import { Student } from 'src/app/Model/student';
import { GroupService } from 'src/app/Service/group.service';
import { MessageService } from 'src/app/Service/message.service';
import { StudentService } from 'src/app/Service/student.service';

@Component({
  selector: 'app-group-members',
  templateUrl: './group-members.component.html',
  styleUrls: ['./group-members.component.css']
})
export class GroupMembersComponent implements OnInit {
  @Input() groupId:number;
  public students:Student[];
  public owner:Student;
  private inChatWith:number[];
  private userId: string;
  public friendRequestSendArray:number[];

  constructor(private groupService:GroupService, private messageService: MessageService, private studentService:StudentService) { }

  ngOnInit(): void {
    this.userId = this.studentService.getStudentFromStorage()['id']
    
    this.studentService.GetFriendRequestSends(parseInt(this.userId)).subscribe((friendRequests:number[])=>{
      this.friendRequestSendArray = friendRequests;
    })

    this.groupService.getGroupMembers(this.groupId, parseInt(this.userId)).subscribe(
      result => {
        this.students = result;
    
      }
    )

    

    this.groupService.getGroupOwner(this.groupId, parseInt(this.userId)).subscribe(
      result => {
        this.owner = result;
      }
    )

    this.messageService.getIdsStudentsInChatWith(parseInt(this.userId)).subscribe(
      result => this.inChatWith = result
    )
  }

  checkIfIsFreind(id:number):boolean
  {
    let pom:boolean = false;
    this.friendRequestSendArray.forEach((el:number)=>{
      if(el === id)
      {
        pom =  true;
      }
    })
    return pom;
  }

  isLoaded(): boolean {
    if((!this.students) || (!this.owner) || (!this.inChatWith))
      return false;
    return true;
  }

  canChatWith(user:Student): boolean {
    if(this.inChatWith.includes(user.id))
      return false;
    return true;
  }

}
