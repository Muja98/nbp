import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router'

@Component({
  selector: 'app-dashboard',
  templateUrl: './dashboard.component.html',
  styleUrls: ['./dashboard.component.css']
})
export class DashboardComponent implements OnInit {
  public isCollapsed = true;
  public navLinks = ['/dashboard/profile', '/dashboard/main' /*, '/dashboard/my-groups'*/]

  constructor(private router:Router) { }

  ngOnInit(): void {
  }

  isActive(ind:number):object {
    return {
      'active': this.router.url == this.navLinks[ind]
    }
  }


}
