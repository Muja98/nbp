import { GroupService } from 'src/app/Service/group.service';
import { GroupStatistics } from './../../../Model/groupstatistics';
import { Component, Input, OnInit, Output } from '@angular/core';
import URL from 'src/API/api'
import {HttpClient} from '@angular/common/http';
import { Router } from '@angular/router';
import { EventEmitter } from 'events';
import { StudentService } from 'src/app/Service/student.service';
import { Student } from 'src/app/Model/student';

@Component({
    selector: 'group-info',
    templateUrl: './group-info.component.html'
})

export class GroupInfoComponent implements OnInit
{
    @Input()groupId: number;

    group: GroupStatistics;
    groupImage:string = "";
    public groupOwner:Student;
    private storageStudentId:number;
    constructor(private http:HttpClient, private router:Router, private groupService:GroupService, private studentService:StudentService) { }

    base64textString = [];
    groupImagePom:any;
   
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

    getGroupStatistics()
    {
        return this.http.get<GroupStatistics>(URL + "/api/groups/" + this.groupId + "/statistics");
    }

    handleEditGroup(){
      let group = {
        Field: this.group.group.field,
        Name: this.group.group.name,
        Description: this.group.group.description,
        GroupPicturePath:this.groupImagePom
      }

      console.log(group)
      this.groupService.editGroup(this.groupId, group);
      window.location.reload();
    }

    canEditGroup(): boolean {
      return this.groupOwner.id == this.studentService.getStudentFromStorage()['id'];
    }

    ngOnInit(): void {
      this.storageStudentId = parseInt(this.studentService.getStudentFromStorage()['id'])
      this.getGroupStatistics().subscribe(
        result => {
          this.group =  result
          if(this.group.group.groupPicturePath !=="" && this.group.group.groupPicturePath !== undefined && this.group.group.groupPicturePath !== null)
            this.groupImage =  'data:image/png;base64,'+this.group.group.groupPicturePath 
          this.groupImagePom =  this.group.group.groupPicturePath;  
        }
      )

      this.groupService.getGroupOwner(this.groupId, this.storageStudentId).subscribe(result =>{
        this.groupOwner = result;
      })
    }
}