import { Component, OnInit, Input } from '@angular/core';
import { Router } from '@angular/router';
import { GroupService } from 'src/app/Service/group.service';
import { StudentService } from 'src/app/Service/student.service';
import { Group } from '../../../Model/group';

@Component({
  selector: 'app-group-element',
  templateUrl: './group-element.component.html',
  styleUrls: ['./group-element.component.css']
})
export class GroupElementComponent implements OnInit {
  @Input() group:Group  
  @Input() showMore: boolean;
  @Input() canJoin: boolean;
  @Input() canLeave: boolean;
  @Input() canDelete: boolean;

  public isCollapsed = true;
  private studentId:number;
  public groupImage = "";

  constructor(private groupService:GroupService, private studentService:StudentService, private router:Router) { }

  ngOnInit(): void {
    this.studentId = this.studentService.getStudentFromStorage()['id'];
    if(this.group.group.groupPicturePath !== null && this.group.group.groupPicturePath !== undefined && this.group.group.groupPicturePath!=="")
      this.groupImage = 'data:image/png;base64,'+ this.group.group.groupPicturePath;
    console.log(this.group)
  }

  handleDeleteGroup():void{
    this.groupService.deleteGroup(this.group.id)
    window.location.reload();
  }

  handleGroupJoin(): void {
    this.groupService.joinGroup(this.studentId, this.group.id).subscribe(
      result => {
        console.log(result);
        this.showMore = false;
        this.canJoin = false;
        this.canLeave = true;
      }
    )
  }

  handleGroupLeave(): void {
    this.groupService.leaveGroup(this.studentId, this.group.id).subscribe(
      result => {
        console.log(result)
        this.showMore = true;
        this.canJoin = true;
        this.canLeave = false;
      }
    )
  }

  handleGroupPageVisit(): void {
    this.router.navigate(["dashboard/group/" + this.group.id]);
  }

}
