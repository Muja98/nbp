import { Component, OnInit, Input } from '@angular/core';
import { Group } from '../../../Model/group';

@Component({
  selector: 'app-group-element',
  templateUrl: './group-element.component.html',
  styleUrls: ['./group-element.component.css']
})
export class GroupElementComponent implements OnInit {
  @Input() group:Group  
  @Input() showMore: boolean;
  public isCollapsed = true;


  constructor() { }

  ngOnInit(): void {
  }

}
