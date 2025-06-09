import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, FormsModule, ReactiveFormsModule, Validators } from '@angular/forms';
import { ActivatedRoute, Router, RouterModule } from '@angular/router';
import { EventService } from '../../../core/services/event.service';
import { Event } from '../../../core/models/event';
import { CommonModule } from '@angular/common';
import { EventsModule } from '../event-list/event-list.module';

@Component({
  selector: 'app-event-edit',
  templateUrl: './event-edit.component.html',
  styleUrls: ['./event-edit.component.scss'],
  imports: [ReactiveFormsModule, CommonModule, RouterModule]
})
export class EventEditComponent implements OnInit {
  eventForm: FormGroup;
  isEditMode = false;
  eventId?: number;
  selectedFile: File | null = null;
  currentImageUrl?: string;
  isLoading = false;
  errorMessage = '';
  tomorrow = new Date(new Date().setDate(new Date().getDate() + 1)).toISOString().split('T')[0];

  constructor(
    private fb: FormBuilder,
    private route: ActivatedRoute,
    private router: Router,
    private eventService: EventService
  ) {
    this.eventForm = this.fb.group({
      name: ['', [Validators.required,Validators.minLength(4), Validators.maxLength(40)]],
      description: ['', [Validators.required, Validators.maxLength(2000)]],
      startDate: [this.tomorrow, [Validators.required]],
      eventPlace: ['', [Validators.required, Validators.maxLength(100)]],
      category: ['', [Validators.required, Validators.maxLength(50)]],
      maxParticipantsCount: ['', [Validators.required, Validators.min(4), Validators.max(300)]]
    });
  }

  ngOnInit(): void {
    this.route.params.subscribe(params => {
      this.eventId = params['id'];
      this.isEditMode = !!this.eventId;
      
      if (this.isEditMode) {
        this.loadEventData();
      }
    });
  }

  loadEventData(): void {
    this.eventService.getEventDetails(this.eventId!).subscribe({
      next: (event) => {
        this.currentImageUrl = event.imagePath;
        this.eventForm.patchValue({
          name: event.name,
          description: event.description,
          startDate: event.startDate ? new Date(new Date().setDate(new Date(event.startDate).getDate())).toISOString().split('T')[0] : null,
          eventPlace: event.eventPlace,
          category: event.category,
          maxParticipantsCount: event.maxParticipantsCount
        });
      },
      error: () => {
        this.errorMessage = 'Ошибка загрузки данных события';
      }
    });
  }

  onFileSelected(event: any): void {
    this.errorMessage = '';
    this.selectedFile = event.target.files[0];
  }

  uploadImage(): void {
    if (!this.selectedFile || !this.eventId) return;

    this.isLoading = true;
    this.eventService.uploadEventImage(this.eventId, this.selectedFile).subscribe({
      next: () => {
        this.isLoading = false;
        this.selectedFile = null;
        this.loadEventData();
      },
      error: () => {
        this.isLoading = false;
        this.errorMessage = 'Ошибка загрузки изображения';
      }
    });
  }

  deleteImage(): void {
    if (!this.eventId) return;

    this.isLoading = true;
    this.eventService.deleteEventImage(this.eventId).subscribe({
      next: () => {
        this.isLoading = false;
        this.currentImageUrl = undefined;
      },
      error: () => {
        this.isLoading = false;
        this.errorMessage = 'Ошибка удаления изображения';
      }
    });
  }

  onSubmit(): void {
    if (this.eventForm.invalid) return;

    this.isLoading = true;
    const formData = this.eventForm.value;

    const eventData: Partial<Event> = {
      name: formData.name,
      description: formData.description,
      startDate: new Date(formData.startDate),
      eventPlace: formData.eventPlace,
      category: formData.category,
      maxParticipantsCount: formData.maxParticipantsCount
    };
    if(this.isEditMode) eventData.id = this.eventId!;
    const operation = this.isEditMode
      ? this.eventService.updateEvent(eventData)
      : this.eventService.createEvent(eventData);

    operation.subscribe({
      next: (event) => {
        this.router.navigate(['/event', event.id]);
      },
      error: (err) => {
        this.isLoading = false;
        let msg;
        if(err.status == 409) msg = "Событие с таким названием уже существует" ;
        if(err.status == 400) msg = "Событие не прошло валидацию" ;
        this.errorMessage = this.isEditMode 
          ? 'Ошибка обновления события: ' + msg
          : 'Ошибка создания события: ' + msg;
      }
    });
  }
}
