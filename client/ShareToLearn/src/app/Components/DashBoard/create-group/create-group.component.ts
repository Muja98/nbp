import { Component, OnInit } from '@angular/core';
import { GroupService } from "../../../Service/group.service";
import { Router } from "@angular/router";

@Component({
  selector: 'app-create-group',
  templateUrl: './create-group.component.html',
  styleUrls: ['./create-group.component.css']
})
export class CreateGroupComponent implements OnInit {
  public groupName:string = "";
  public groupField:string = "";
  public groupDescription:string = "";
  public canCreate:boolean = false;
  private userId:string;
  
  constructor(private service:GroupService,  private router:Router) { }

  ngOnInit(): void {
    this.userId = JSON.parse(localStorage.getItem('user'))['id'];
  }

  handleGroupCreate():void {
    this.service.createGroup(this.userId, this.groupName, this.groupField, this.groupDescription).subscribe(
      result => {
        console.log("Created group");
        this.router.navigate(['/dashboard/my-groups'])
      }
    );
  }

  canCreateCheck():void {
    this.canCreate = !((this.groupName == "") || (this.groupField == "") || (this.groupDescription == ""))
  }
}
