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
  @Input() owner: boolean;
  public isCollapsed = true;
  private studentId:number;


  constructor(private groupService:GroupService, private studentService:StudentService, private router:Router) { }

  ngOnInit(): void {
    this.studentId = this.studentService.getStudentFromStorage()['id'];
  }

  handleGroupJoin(): void {
    this.groupService.joinGroup(this.studentId, this.group.id).subscribe(
      result => {
        console.log(result);
        this.showMore = !this.showMore;
      }
    )
  }

  handleGroupLeave(): void {
    this.groupService.leaveGroup(this.studentId, this.group.id).subscribe(
      result => {
        console.log(result)
        this.showMore = !this.showMore;
      }
    )
  }

  handleGroupPageVisit(): void {
    this.router.navigate(["dashboard/group/" + this.group.id]);
  }

}
