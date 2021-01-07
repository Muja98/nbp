import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router'

@Component({
  selector: 'app-dashboard',
  templateUrl: './dashboard.component.html',
  styleUrls: ['./dashboard.component.css']
})
export class DashboardComponent implements OnInit {
  public isCollapsed = true;
  public linkRoot = "/dashboard"
  public navLinks = [
    {
      link:"/profile", 
      text:"Profile"
    }, 
    {
      link:"/main", 
      text:"Search groups"
    }, 
    {
      link:"/create-group", 
      text:"Create group"
    }, 
    {
      link:"/my-groups", 
      text:"My groups"
    },
    {
      link:"/search-users",
      text:"Search users"
    }
  ]

  constructor(private router:Router) { }

  ngOnInit(): void {
  }

  isActive(ind:number):object {
    return {
      'active': this.router.url == this.linkRoot + this.navLinks[ind].link
    }
  }

}
