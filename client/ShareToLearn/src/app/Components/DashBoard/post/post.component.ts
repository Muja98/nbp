import { Component, OnInit } from '@angular/core';

@Component({
  selector: 'app-post',
  templateUrl: './post.component.html',
  styleUrls: ['./post.component.css']
})
export class PostComponent implements OnInit {

  constructor() { }

  ngOnInit(): void {
  }

  povecaj(e)
  {
    e.target.style.height = "0px";
  e.target.style.height = (e.target.scrollHeight + 25)+"px";
  }

}
