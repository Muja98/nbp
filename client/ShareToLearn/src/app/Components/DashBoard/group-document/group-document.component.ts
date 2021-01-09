import { Document } from './../../../Model/document';
import { Component, Input, OnInit } from '@angular/core';

@Component({
  selector: 'app-group-document',
  templateUrl: './group-document.component.html',
  styleUrls: ['./group-document.component.css']
})
export class GroupDocumentComponent implements OnInit {

  constructor() { }
  //https://youtu.be/62D0vX9QeLg?t=112
  public levelArray: Array<number> = [1,2,3,4,5];
  public currentLevel: number = 0;
  public searchString: String;
  public documentArray: Array<Document> = [];
  public newDocument: Document;
  @Input()groupId: number;

  public fullNumberOfUsers:number;
  public page:number;
  public perPage:number;

  handlePageChange():void {
    //IMPLEMENT FUNCTION
  }

  handleGetLevel(level:number):void
  {
    this.currentLevel = level;
  }
  handleGetLevelFromInput(level:number)
  {
      this.newDocument.level = level;
  }

  base64textString = [];

  onUploadChange(evt: any) {
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
  }

  handleAddNewDocument()
  {
    if(this.newDocument.name==="" || this.newDocument.level===0 || this.newDocument.description ==="" || this.base64textString.length==0)
      return;
    //U newDocument se nalaze svi podaci sa inputa
    //U base64textString se nalazi sam pdf file
    //TODO: dodati servis za dodavanje novog dokumenta
    window.location.reload();
  }

  handleClickSearch()
  {
    if(this.searchString==="" || this.currentLevel == 0)
    return;
    //U searchString se nalazi string za search
    //U curentLevel se nalazi izabrani nivo sa leve strane
    //TODO: dodati servis za pretragu
    this.searchString = "";
    this.currentLevel = 0;
  } 

  ngOnInit(): void {
    this.newDocument = new Document();
    let p1 = new Document();
    p1.description = "neki opis";
    p1.name = "naziv1";
    p1.level = 3;
    this.documentArray.push(p1)
    let p2 = new Document();
    p2.description = "neki opis";
    p2.name = "naziv2";
    p2.level = 3;
    this.documentArray.push(p2)
  }

}
