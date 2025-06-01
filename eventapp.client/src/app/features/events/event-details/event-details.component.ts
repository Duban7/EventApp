import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { EventService } from '../../../core/services/event.service';
import { Event } from '../../../core/models/event';
import { AuthService } from '../../../core/services/auth.service';
import { Participant } from '../../../core/models/participant';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-event-details',
  templateUrl: './event-details.component.html',
  styleUrls: ['./event-details.component.scss'],
  imports: [CommonModule]
})
export class EventDetailsComponent implements OnInit {
  event!: Event;
  participants: Participant[] = [];
  isLoading = true;
  isRegistering = false;
  currentUserParticipant: Participant | null = null;

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private eventService: EventService,
    public authService: AuthService
  ) {}

  ngOnInit(): void {
    this.loadEvent();
  }

  loadEvent(): void {
    const id = +this.route.snapshot.paramMap.get('id')!;
    this.eventService.getEventDetails(id).subscribe({
      next: (event) => {
        this.event = event;
        this.isLoading = false;
        this.loadParticipants();
      },
      error: () => {
        this.router.navigate(['/events']);
      }
    });
  }

  loadParticipants(): void {
    this.eventService.getEventParticipants(this.event.id).subscribe({
      next: (participants) => {
        console.log(participants);
        this.participants = participants;
        this.checkCurrentUserParticipation();
        this.isLoading = false;
      },
      error: () => {
        this.isLoading = false;
      }
    });
  }

   checkCurrentUserParticipation(): void {
    if (!this.authService.isLoggedIn) {
      this.currentUserParticipant = null;
      return;
    }
    
    this.currentUserParticipant = this.participants.find(
      p => p.id === this.authService.userId
    ) || null;
  }

  toggleRegistration(): void {
    if (!this.authService.isLoggedIn) {
      this.router.navigate(['/login'], { 
        queryParams: { returnUrl: this.router.url } 
      });
      return;
    }

    this.isRegistering = true;
    
    if (this.currentUserParticipant) {
      this.eventService.unregisterFromEvent(this.event.id).subscribe({
        next: () => this.handleRegistrationSuccess(),
        error: (err) => this.handleRegistrationError(err),
        complete: () => this.isRegistering = false
      });
    } else {
      this.eventService.registerOnEvent(this.event.id).subscribe({
        next: () => this.handleRegistrationSuccess(),
        error: (err) => this.handleRegistrationError(err),
        complete: () => this.isRegistering = false
      });
    }
  }

  private handleRegistrationSuccess(): void {
    this.loadEvent();
  }

  private handleRegistrationError(err: any): void {
    console.error('Registration error', err);
  }
}