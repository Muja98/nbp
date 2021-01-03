import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';

@Component({
  selector: 'app-group-page',
  templateUrl: './group-page.component.html',
  styleUrls: ['./group-page.component.css']
})
export class GroupPageComponent implements OnInit {
  public groupId:number;
  private sub:any;

  constructor(private route:ActivatedRoute) { }

  ngOnInit(): void {
    this.sub = this.route.params.subscribe(params => {
      this.groupId = +params['idGroup'];
    })
  }

  ngOnDestroy() {
    this.sub.unsubscribe();
  }

}
