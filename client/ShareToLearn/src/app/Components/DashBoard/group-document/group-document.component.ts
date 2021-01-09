import { DocumentService } from './../../../Service/document.service';
import { StudentService } from 'src/app/Service/student.service';
import { Document } from './../../../Model/document';
import { Component, Input, OnInit } from '@angular/core';

@Component({
  selector: 'app-group-document',
  templateUrl: './group-document.component.html',
  styleUrls: ['./group-document.component.css']
})
export class GroupDocumentComponent implements OnInit {

  constructor(private userService: StudentService, private documentService: DocumentService) { }
  //https://youtu.be/62D0vX9QeLg?t=112
  public levelArray: Array<number> = [1,2,3,4,5];
  public currentLevel: string = "";
  public searchString: string = "";
  public documentArray: Array<Document> = [];
  public newDocument={
    name:         "",
    level:         "",
    description:  "",
    documentPath:  "",
  }
  @Input()groupId: number;

  public fullNumberOfUsers:number;
  public page:number;
  public perPage:number;

  handlePageChange():void {
    //IMPLEMENT FUNCTION
  }

  handleGetLevel(level:string):void
  {
    this.currentLevel = level;
  }
  handleGetLevelFromInput(level:string)
  {
      this.newDocument.level = level;
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
    //this.base64textString.push('data:application/pdf;base64,' + btoa(e.target.result));
    this.base64textString.push(btoa(e.target.result));
    this.newDocument.documentPath= this.base64textString.toString();
  }

  handleAddNewDocument()
  {
    if(this.newDocument.name==="" || this.newDocument.level==="" || this.newDocument.description ==="" || this.base64textString.length==0)
      return;
    let studentPom = this.userService.getStudentFromStorage();

    let document ={
      Name: this.newDocument.name,
      Level: this.newDocument.level,
      Description: this.newDocument.description,
      DocumentPath: this.newDocument.documentPath
    }
    this.documentService.createDocument(this.groupId, studentPom.id, document);
    
   window.location.reload();
  }

  handleClickDocument(documentId:string)
  {
    let pdfFile;
    this.documentService.getDocument(documentId).subscribe((pdf:string)=>{
    
      alert(pdf)
    }); 
   
  }

  handleClickSearch()
  {
    if(this.searchString==="" || this.currentLevel == "")
    return;
    //U searchString se nalazi string za search
    //U curentLevel se nalazi izabrani nivo sa leve strane
    //TODO: dodati servis za pretragu
    this.searchString = "";
    this.currentLevel = "";
  } 

  ngOnInit(): void {
      this.documentService.getDocuments(this.groupId, this.currentLevel, this.searchString).subscribe((documents:any)=>{
          this.documentArray = documents;
      })
  }

}
