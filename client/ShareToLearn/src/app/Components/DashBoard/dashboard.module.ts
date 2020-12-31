import { BrowserModule } from '@angular/platform-browser';
import { ProfileComponent } from './profile/profile.component';
import { DashboardComponent } from './dashboard.component';
import { MainPageComponent } from './main-page/main-page.component';
import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { GroupElementComponent } from './group-element/group-element.component';
import { NgbModule } from '@ng-bootstrap/ng-bootstrap';

@NgModule({
  declarations: [MainPageComponent,DashboardComponent, ProfileComponent, GroupElementComponent],
  imports: [
    CommonModule,
    FormsModule,
    BrowserModule,
    RouterModule.forRoot([
      {path:'dashboard', component:DashboardComponent, children:[
        {path:'main',component:MainPageComponent},
        {path:'profile',component:ProfileComponent}
      ]}]),
      NgbModule

   

  ]
})
export class DashboardModule { }