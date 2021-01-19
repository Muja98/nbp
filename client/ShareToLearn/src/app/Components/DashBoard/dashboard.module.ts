import { FriendRequestComponent } from './friend-request/friend-request.component';
import { StudentContainerComponent } from './messanger/student-container/student-container.component';
import { MessageContainerComponent } from './messanger/message-container/message-container.component';
import { MyFriendsComponent } from './my-friends-page/my-friends.component';
import { GroupInfoComponent } from './group-info/group-info.component';
import { MyGroupsComponent } from './my-groups-page/my-groups-list.component';
import { PostComponent } from './post/post.component';
import { GroupComponent } from './group/group.component';
import { BrowserModule } from '@angular/platform-browser';
import { ProfileComponent } from './profile/profile.component';
import { DashboardComponent } from './dashboard.component';
import { MainPageComponent } from './main-page/main-page.component';
import { NgModule, Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { GroupElementComponent } from './group-element/group-element.component';
import { NgbModule } from '@ng-bootstrap/ng-bootstrap';
import { CreateGroupComponent } from './create-group/create-group.component';
import { GroupPageComponent } from './group-page/group-page.component';
import { GroupMembersComponent } from './group-members/group-members.component';
import { StudentInfoElementComponent } from './student-info-element/student-info-element.component';
import { SearchUsersComponent } from './search-users/search-users.component';
import { GroupDocumentComponent } from './group-document/group-document.component';
import { OtherUsersGroupsPageComponent } from './other-users-groups-page/other-users-groups-page.component';
import { ToastrModule } from 'ngx-toastr';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import {StoreModule} from '@ngrx/store';
import {reducer} from '../ngrx/reducers/friendRequest.reducer';


@NgModule({
  declarations: [
                 MainPageComponent,
                 DashboardComponent,
                 ProfileComponent,
                 GroupElementComponent,
                 MyGroupsComponent,
                 GroupComponent,
                 PostComponent,
                 CreateGroupComponent,
                 GroupPageComponent,
                 GroupMembersComponent,
                 StudentInfoElementComponent,
                 GroupInfoComponent,
                 SearchUsersComponent,
                 GroupDocumentComponent,
                 MyFriendsComponent,
                 OtherUsersGroupsPageComponent,
                 MessageContainerComponent,
                 StudentContainerComponent,
                 FriendRequestComponent
  ],

  imports: [
    CommonModule,
    FormsModule,
    BrowserModule,
    BrowserAnimationsModule,
    ToastrModule.forRoot({
      timeOut: 5000
    })
    StoreModule.forRoot({
      friendRequest: reducer
    }),
    RouterModule.forRoot([
      {path:'dashboard', component:DashboardComponent, children:[
        {path:'main',component:MainPageComponent},
        {path:'profile',component:ProfileComponent},
        {path: 'my-groups', component:MyGroupsComponent},
        {path:'group/:idGroup', component:GroupPageComponent},
        {path:'create-group',component:CreateGroupComponent},
        {path: 'group-info', component:GroupInfoComponent},
        {path:'search-users', component:SearchUsersComponent},
        {path:'group-component', component:GroupDocumentComponent},
        {path:'my-friends', component:MyFriendsComponent},
        {path:'profile/:studentId', component:ProfileComponent},
        {path:'student-groups/:studentId', component:OtherUsersGroupsPageComponent},
        {path:'messanger', component:StudentContainerComponent},
        {path:'friend-request', component:FriendRequestComponent}
      ]}]),
      NgbModule
  ]
})
export class DashboardModule { }