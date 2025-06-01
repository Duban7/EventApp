// events.module.ts
import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { RouterModule } from '@angular/router';

import { EventListComponent } from './event-list.component';
import { TruncatePipe } from '../../../shared/pipes/truncatePipe';

@NgModule({
  declarations: [
    EventListComponent
  ],
  imports: [
    TruncatePipe,
    CommonModule,
    FormsModule,
    ReactiveFormsModule,
    RouterModule.forChild([
      { path: '', component: EventListComponent }
    ])
  ]
})
export class EventsModule { }