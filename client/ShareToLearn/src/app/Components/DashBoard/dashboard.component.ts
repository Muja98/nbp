import { StudentService } from './../../Service/student.service';
import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router'
import { FriendRequest } from 'src/app/Model/friendrequest';
import { signalRService } from './../../Service/signalR.service';
import * as signalR from '@aspnet/signalr';
import { ToastrService } from 'ngx-toastr';

@Component({
  selector: 'app-dashboard',
  templateUrl: './dashboard.component.html',
  styleUrls: ['./dashboard.component.css']
})

export class DashboardComponent implements OnInit {
  public isCollapsed = true;
  public _hubConnection: signalR.HubConnection;
  public linkRoot = "/dashboard";
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
    },
    {
      link:"/my-friends",
      text:"My friends"
    },
    {
      link:"/messanger",
      text:"Messanger"
    }
  ]
  userId:number;
  friend_requests:FriendRequest[];


  constructor(
    private toastr:ToastrService,
    public signalRService: signalRService, 
    private router:Router, 
    private userService:StudentService, 
    private studentService:StudentService) { }

  handleLogOut()
  {
    this.userService.logoutStudent();
    this.router.navigate(['/login']);
  }

  ngOnInit(): void {
    if(!this.userService.getStudentFromStorage)
    {
      this.router.navigate(['/login']);
    }

    this.userId =  JSON.parse(localStorage.getItem('user'))['id'];
    this.getFriendRequests();

    this._hubConnection = new signalR.HubConnectionBuilder()
    .withUrl("https://localhost:44374/chat", )
    .build()

    this._hubConnection
      .start()
      .then(() => {
        console.log('Connection started! :)')
        const channelName = "channel:" + this.userId;
        this._hubConnection.invoke("JoinRoom", channelName).catch((err)=>{
          console.log(err)
        })
        console.log(channelName);
      })
      .catch(err => console.log('Error while establishing connection :('));
   
    this._hubConnection.on('ReceiveMessage', (newMessage:any) => {
    
      if(this.router.url != "/dashboard/messanger") {
        this.toastr.info(newMessage['content'], "New message from " + newMessage['sender']);    
      }
    });

    this._hubConnection.on('ReceiveFriendRequests', (newRequest:any) => {
      console.log(newRequest);
      debugger
      var request:FriendRequest = newRequest['requestDTO'];
      this.friend_requests.push(request);
      this.toastr.info("You have a new friend request!"); 
    });

    
  }

  isActive(ind:number):object {
    return {
      'active': this.router.url == this.linkRoot + this.navLinks[ind].link
    }
  }

  getFriendRequests() {
    debugger
    this.studentService.getFriendRequests(this.userId).subscribe(
      result => { this.friend_requests = result }
    )
  }

  removeRequestFromList(id:string) {
    this.friend_requests = this.friend_requests.filter(obj => obj.id != id);
  }

}
