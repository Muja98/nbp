import { Post } from './../../../Model/post';
import { Component, OnInit } from '@angular/core';
import {formatDate } from '@angular/common';
@Component({
  selector: 'app-group',
  templateUrl: './group.component.html',
  styleUrls: ['./group.component.css']
})
export class GroupComponent implements OnInit {

  today= new Date();
  todaysDataTime = '';
  newPost = new Post();

  constructor() { 
    this.todaysDataTime = formatDate(this.today, 'dd-MM-yyyy hh:mm:ss a', 'en-US', '+0530');
  }

  handleAddNewPost()
  {
    if(this.newPost.data==="")return;
    this.newPost.date = this.todaysDataTime;
    console.log(this.newPost)
    this.newPost = new Post();
  }
  
  povecaj(e)
  {
    e.target.style.height = "0px";
    e.target.style.height = (e.target.scrollHeight + 10)+"px";
  }

  ngOnInit(): void {
  }

}
