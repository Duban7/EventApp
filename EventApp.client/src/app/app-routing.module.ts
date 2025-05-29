import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { LoginComponent } from './features/auth/login/login.component';
import { RegisterComponent } from './features/auth/register/register.component';
import { ProfileComponent } from './features/user/user-profile.component';
import { AuthGuard } from './core/guards/AuthGuard';
import { AdminGuard } from './core/guards/AdminGuard';
import { EventDetailsComponent } from './features/events/event-details/event-details.component';
import { EventEditComponent } from './features/events/event-edit/event-edit.component';
import { EventListComponent } from './features/events/event-list/event-list.component';

const routes: Routes = [
  { path: 'login', component: LoginComponent },
  { path: 'register', component: RegisterComponent },
  { 
    path: 'profile', 
    component: ProfileComponent,
    canActivate: [AuthGuard] 
  },
  {
    path: 'admin',
    children: [
      {
        path: '',
        component: EventListComponent
      },
      {
        path: 'event/new',
        component: EventEditComponent
      },
      {
        path: 'event/edit/:id',
        component: EventEditComponent
      }
    ],
    canActivate: [AuthGuard, AdminGuard]
  },
  {
    path: '',
    loadChildren: ()=> import('./features/events/event-list/event-list.module').then(m=>m.EventsModule)
  },
  {
    path: 'event/:id',
    component: EventDetailsComponent,
    canActivate: [AuthGuard]
  }
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
