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

  base64textString = [];
  groupImagePom:any;
  groupImage:string;

  onUploadChange(evt: any) {
    const file = evt.target.files[0];
  
    if (file) {
      this.base64textString = []
      const reader = new FileReader();
  
      reader.onload = this.handleReaderLoaded.bind(this);
      reader.readAsBinaryString(file);
    }
  }
  
  handleReaderLoaded(e) {
    let array =  [];

    array.push(btoa(e.target.result))
    this.groupImagePom = array[0];
    
    this.base64textString.push('data:image/png;base64,' + btoa(e.target.result));
    this.groupImage=this.base64textString[0];
  }

  ngOnInit(): void {
    this.userId = JSON.parse(localStorage.getItem('user'))['id'];
  }

  handleGroupCreate():void {
    this.service.createGroup(this.userId, this.groupName, this.groupField, this.groupDescription, this.groupImagePom).subscribe(
      result => {
        console.log("Created group");
        this.router.navigate(['/dashboard/my-groups'])
      }
    );
  }

  canCreateCheck():void {
    this.canCreate = !((this.groupName == "") || (this.groupField == "") || (this.groupDescription == "") || (this.groupImagePom== ""))
  }
}
