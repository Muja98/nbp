import { ActivatedRoute, Router } from '@angular/router';
import { StudentService } from './../../../Service/student.service';
import { Student } from './../../../Model/student';
import { Component, OnInit, Input, OnDestroy } from '@angular/core';
import { FriendRequest } from 'src/app/Model/friendrequest';
import { MessageService } from 'src/app/Service/message.service';

@Component({
  selector: 'app-profile',
  templateUrl: './profile.component.html',
  styleUrls: ['./profile.component.css']
})
export class ProfileComponent implements OnInit, OnDestroy {
  public studentId:number;
  public edit:boolean;
  public student:Student;
  public pomStudent:Student;
  public studentChangeFlag:boolean = false;
  public tempStudent:any;
  public dateOfBirth:Date;
  public imgSrc:string;
  public imgSrcPom:string;
  private sub:any;
  private inChatWith:number[];
  public firstMessage:string;
  public FriendRequestSendsArray:number[];
  public frinedRequestFlag:boolean = false;
  constructor(private studentService:StudentService,private router:Router, private route:ActivatedRoute, private messageService:MessageService) {
    
   }
  
  handleSetStudent():void
  {
    this.tempStudent = this.studentService.getStudentFromStorage();
    this.student.student.firstName = this.tempStudent.firstName;
    if(this.student.student.firstName === this.tempStudent.lastName)
      this.student.student.lastName = "";
    else
      this.student.student.lastName = this.tempStudent.lastName
    this.student.student.email = this.tempStudent.email;
    this.student.student.profilePicturePath = this.tempStudent.profilePicturePath;
    this.student.student.dateOfBirth = this.tempStudent.dateOfBirth;
    this.dateOfBirth = new Date(this.student.student.dateOfBirth);

    if(this.student.student.profilePicturePath && !String(this.student.student.profilePicturePath).includes("data:image")
        && !String(this.student.student.profilePicturePath).includes("assets/profileDefault.png")) {
      this.imgSrc = 
        'data:image/png;base64,' + this.student.student.profilePicturePath;
    }
    else if(!this.student.student.profilePicturePath) {
      this.imgSrc = 
        "assets/profileDefault.png";
    }
    
    //TODO:Get student from API
  }

  base64textString = [];

  onUploadChange(evt: any) {
    this.base64textString = [];
    const file = evt.target.files[0];
  
    if (file) {
      const reader = new FileReader();
  
      reader.onload = this.handleReaderLoaded.bind(this);
      reader.readAsBinaryString(file);
    }
  }
  
  handleReaderLoaded(e) {
    let pomNiz = [];
    pomNiz.push(btoa(e.target.result));
    this.base64textString.push('data:image/png;base64,' + btoa(e.target.result));
    this.imgSrc=this.base64textString[0];
    this.imgSrcPom = pomNiz[0];
    this.student.student.profilePicturePath = pomNiz[0];
  }
 
  handleCheckStudent():void
  {
  
    if(
        this.student.student.firstName === this.pomStudent.student.firstName     &&
        this.student.student.lastName === this.pomStudent.student.lastName       &&
        this.student.student.dateOfBirth === this.pomStudent.student.dateOfBirth &&
        this.student.student.email === this.pomStudent.student.email             
      )
    {
      this.studentChangeFlag = false
    }
 
    else {this.studentChangeFlag = true};
   
  }

  handleEditStudent()
  {
      this.student.id = this.tempStudent.id;
      let pom = this.dateOfBirth.toString().split(" ");

      if(this.dateOfBirth.toString().length>10)
      {
        let array = ['Jan','Feb','Mar','Apr','May','Jun','Jul','Aug','Sep','Oct','Nov','Dec']
       
        let mesec = 0;
        for(let i=0; i<array.length; i++)
        {
          if(array[i]===pom[1])
          {
            mesec = i+1;
          }
        }
        var mes = "";
        if(mesec<10)
          mes = "0";
        mes = mes+mesec;
        this.student.student.dateOfBirth = pom[3]+"-"+mes+"-"+pom[2];
      }
      else
        this.student.student.dateOfBirth = this.dateOfBirth
     
      this.studentService.editStudent(this.student);
      this.studentService.logoutStudent();
      this.router.navigate(['/login'])
  }

  handleViewStudentGroups(): void {
    this.router.navigate(['dashboard/student-groups/' + this.studentId]);
  }

  handleSendFriendRequest(event): void {
   event.target.disabled = true;
   event.target.innerText = "Friend request sent";

    let currentUser = this.studentService.getStudentFromStorage();

    var request = new FriendRequest();
    request.request.id = parseInt(currentUser.id);
    request.request.firstName = currentUser.firstName;
    request.request.lastName = currentUser.lastName;
    request.request.email = currentUser.email;
    request.request.profilePicturePath = currentUser.profilePicturePath;


    this.studentService.sendFriendRequest(currentUser.id, this.student.id, request);
  }

  handleStartChat(): void {
    console.log(this.firstMessage)
    const studentFromStorage = this.studentService.getStudentFromStorage();
    const sender = new Student();
    sender.id = parseInt(studentFromStorage['id']);
    sender.isFriend = true;
    sender.student = {
      firstName: studentFromStorage['firstName'],
      lastName: studentFromStorage['lastName'],
      dateOfBirth: new Date(studentFromStorage['dateOfBirth']),
      email: studentFromStorage['email'],
      profilePicturePath: String(studentFromStorage['profilePicturePath'])
    }
    
    const receiver = this.student;
    this.messageService.startChat({'sender': sender, 'receiver': receiver, 'firstMessage': this.firstMessage}).subscribe(result => {
      this.router.navigate(["/dashboard/messanger"])
    });
  }

  public isLoaded(): boolean {
    if(this.studentId) {
      if(this.student && this.inChatWith)
        return true
      return false
    }
    else {
      if(this.student)
        return true
      return false
    }
  }

  public canChatWith(): boolean {
    return this.inChatWith.includes(this.studentId);
  }

  private getIdsStudentsInChatWith() {
    this.messageService.getIdsStudentsInChatWith(parseInt(this.studentService.getStudentFromStorage()['id'])).subscribe(
      result => this.inChatWith = result
    )
  }

  
  ngOnInit(): void {
    this.sub = this.route.params.subscribe(params => {
      this.studentId = +params['studentId'];
      if(Number.isNaN(this.studentId)) {
        this.edit = true;
        this.student = new Student();
        this.pomStudent = new Student();
        this.handleSetStudent();
        
        this.pomStudent.student.firstName = this.student.student.firstName;
        this.pomStudent.student.lastName = this.student.student.lastName;
        this.pomStudent.student.email = this.student.student.email;
        this.pomStudent.student.dateOfBirth = this.student.student.dateOfBirth;
      }
      else {
        this.edit = false;
        let id = this.studentService.getStudentFromStorage().id;
        this.studentService.GetFriendRequestSends(id).subscribe((friendRequests:number[])=>{
            this.FriendRequestSendsArray = friendRequests;
        });
        this.studentService.getSpecificStudent(this.studentId, parseInt(this.studentService.getStudentFromStorage()['id'])).subscribe(
          result => {
            this.student = result;

            this.FriendRequestSendsArray.forEach((el:any)=>{
              if(el===result.id)
              {
                this.frinedRequestFlag = true;
              }
                
            })
            this.imgSrc = 'data:image/png;base64,' + this.student.student.profilePicturePath;
            this.dateOfBirth = new Date(this.student.student.dateOfBirth);
          }
        )

        this.getIdsStudentsInChatWith();
      }
    })
  }

  ngOnDestroy(): void {
    this.sub.unsubscribe();
  }
  
}
