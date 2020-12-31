import { HttpParams } from '@angular/common/http';
import { Component, OnInit } from '@angular/core';
import { Group } from '../../../Model/group';
import { GroupService } from '../../../Service/group.service';

@Component({
  selector: 'app-main-page',
  templateUrl: './main-page.component.html',
  styleUrls: ['./main-page.component.css']
})
export class MainPageComponent implements OnInit {
  groups:Group[];
  public isCollapsed = true;

  constructor(private service: GroupService) {}

  ngOnInit(): void {
    const params = new HttpParams().set('name', "").set('field', "").set('from', "0").set('to', "5");
    this.service.getFilteredGroups(params).subscribe(
      result => {
        debugger
        this.groups = result['value']
      }
    )
  }

}
