import { Component, OnDestroy, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { GroupService } from 'src/app/Service/group.service';
import { StudentService } from 'src/app/Service/student.service';

@Component({
  selector: 'app-group-page',
  templateUrl: './group-page.component.html',
  styleUrls: ['./group-page.component.css']
})
export class GroupPageComponent implements OnInit, OnDestroy {
  public groupId:number;
  private sub:any;
  public relationship:string;
  private studentId:number;

  constructor(private route:ActivatedRoute, private groupService:GroupService, private studentService:StudentService) { }

  ngOnInit(): void {
    this.studentId = this.studentService.getStudentFromStorage()['id'];
    this.relationship = null;
    this.sub = this.route.params.subscribe(params => {
      this.groupId = +params['idGroup'];
      this.groupService.getStudentGroupRelationship(this.studentId, this.groupId).subscribe(result => {
        debugger
        this.relationship = "NORELATIONSHIP"
        if(result['type'])
          this.relationship = result['type']
      })
    })
  }

  ngOnDestroy() {
    this.sub.unsubscribe();
  }

  handleGroupJoin(): void {
    this.groupService.joinGroup(this.studentId, this.groupId).subscribe(
      result => {
        console.log(result);
        this.relationship = "MEMBER"
      }
    )
  }

  handleGroupLeave(): void {
    this.groupService.leaveGroup(this.studentId, this.groupId).subscribe(
      result => {
        console.log(result)
        this.relationship = "NORELATIONSHIP"
      }
    )
  }
}
