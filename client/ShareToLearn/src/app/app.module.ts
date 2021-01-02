import { StudentService } from './Service/student.service';
import { DashboardModule } from './Components/DashBoard/dashboard.module';
import { RouterModule } from '@angular/router';
import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import {HttpClientModule} from '@angular/common/http'
import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { LoginComponent } from './Components/Auth/login/login.component';
import { RegisterComponent } from './Components/Auth/register/register.component';
import { FormsModule } from '@angular/forms';
import { NgbModule } from '@ng-bootstrap/ng-bootstrap';

@NgModule({
  declarations: [
    AppComponent,
    LoginComponent,
    RegisterComponent
  ],
  imports: [
    FormsModule,
    AppRoutingModule,
    HttpClientModule,
    DashboardModule,
    RouterModule.forRoot([
      {path: '', component: LoginComponent},
      {path: 'login', component: LoginComponent},
      {path: 'register', component: RegisterComponent}
    ]),
    NgbModule
  ],
  providers: [],
  bootstrap: [AppComponent]
})
export class AppModule { }
