import { Component, EventEmitter, Input, OnInit } from '@angular/core';
import { Student } from 'src/app/Model/student';
import { MessageService } from 'src/app/Service/message.service';
import { StudentService } from 'src/app/Service/student.service';

@Component({
  selector: 'app-student-container',
  templateUrl: './student-container.component.html',
  styleUrls: ['./student-container.component.css']
})
export class StudentContainerComponent implements OnInit {
  @Input() students: Student[];
  public changeStudentEvent: EventEmitter<Student> = new EventEmitter();
  private studentId: number;
  public chosenStudent: Student;

  constructor(private messageService: MessageService, private studentService:StudentService) { }

  ngOnInit(): void {
    this.studentId = this.studentService.getStudentFromStorage()['id'];
    this.messageService.getStudentsInChatWith(this.studentId).subscribe(result => {
      this.students = result
      this.chosenStudent = result[0]
    })
  }

  public imgSrc(picturePath:string) {
    if(picturePath)
      return 'data:image/png;base64,' + picturePath;
    else
      return "assets/profileDefault.png";
  }

  public changeChatStudent(student:Student) {
    this.chosenStudent = student;
    this.changeStudentEvent.emit(this.chosenStudent);
  }

}
