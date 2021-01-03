import { Component, OnInit, Input } from '@angular/core';
import { GroupService } from 'src/app/Service/group.service';

@Component({
  selector: 'app-group-members',
  templateUrl: './group-members.component.html',
  styleUrls: ['./group-members.component.css']
})
export class GroupMembersComponent implements OnInit {
  @Input() groupId:number;
  public students:any;
  public owner:any;

  constructor(private service:GroupService) { }

  ngOnInit(): void {
    this.service.getGroupMembers(this.groupId).subscribe(
      result => {
        debugger
        this.students = result;
      }
    )

    this.service.getGroupOwner(this.groupId).subscribe(
      result => {
        debugger
        this.owner = result;
      }
    )
  }

}
