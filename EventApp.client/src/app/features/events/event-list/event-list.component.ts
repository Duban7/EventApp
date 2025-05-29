// components/event-list.component.ts
import { Component, OnInit } from '@angular/core';
import { EventService } from '../../../core/services/event.service';
import { Page } from '../../../core/models/page';
import { Event as _Event } from '../../../core/models/event';
import { FormBuilder, FormGroup, FormsModule } from '@angular/forms';
import { Router } from '@angular/router';

@Component({
  selector: 'app-event-list',
  standalone: false,
  templateUrl: './event-list.component.html',
  styleUrls: ['./event-list.component.scss'],
})
export class EventListComponent implements OnInit {
  eventsPage!: Page<_Event>;
  currentPage = 1;
  pageSize = 5;
  pageSizes = [5, 10, 20];
  isLoading = true;
  showFilters = false;
  isFiltered = false;
  filterForm: FormGroup;
  isAdminMode = false;
  showDeleteModal = false;
  eventToDelete: number | null = null;

  constructor(private eventService: EventService,
              private fb: FormBuilder,
              private router: Router,
  ) {
    this.filterForm = this.fb.group({
      name: [''],
      category: [''],
      eventPlace: [''],
      startDate: [null]
    });
  }

  ngOnInit(): void {
    this.checkAdminMode();
    this.loadEvents();
  }
  
  checkAdminMode(): void {
    this.isAdminMode = this.router.url.toString().includes('admin');
  }

  confirmDelete(eventId: number): void {
    this.eventToDelete = eventId;
    this.showDeleteModal = true;
  }

     cancelDelete(): void {
    this.showDeleteModal = false;
    this.eventToDelete = null;
  }

  deleteEvent(): void {
    if (this.eventToDelete) {
      this.eventService.deleteEvent(this.eventToDelete).subscribe({
        next: () => {
          this.showDeleteModal = false;
          this.loadEvents();
        },
        error: (err) => {
          console.error('Ошибка удаления:', err);
          this.showDeleteModal = false;
        }
      });
    }
  }

  toggleFilters(): void {
    this.showFilters = !this.showFilters;
    if (!this.showFilters && this.isFiltered) {
      this.resetFilters();
    }
  }
  
  applyFilters(): void {
    this.currentPage = 1;
    this.loadFilteredEvents();
    this.isFiltered = true;
  }

  resetFilters(): void {
    this.filterForm.reset();
    this.currentPage = 1;
    this.isFiltered = false;
    this.loadEvents();
  }

  loadEvents(): void {
    this.isLoading = true;
    this.eventService.getEvents(this.currentPage, this.pageSize)
      .subscribe(page => {
        this.eventsPage = page;
        this.currentPage = page.pageIndex;
        this.isLoading = false;
      });
  }

  loadFilteredEvents(): void {
    this.isLoading = true;
    this.eventService.getEventsFiltered(
      this.filterForm.value,
      this.currentPage,
      this.pageSize
    ).subscribe({
      next: (page) => {
        this.eventsPage = page;
        this.isLoading = false;
      },
      error: () => {
        this.isLoading = false;
      }
    });
  }

  goToPage(page: number): void {
    if (page < 1 || page > this.eventsPage.totalPages) return;
    this.currentPage = page;
    if (this.isFiltered) {
      this.loadFilteredEvents();
    } else {
      this.loadEvents();
    }
  }

  changePageSize(event: Event): void {
    this.pageSize = +(event.target as HTMLSelectElement).value;
    this.currentPage = 1;
    if (this.isFiltered) {
      this.loadFilteredEvents();
    } else {
      this.loadEvents();
    }
  }

  getVisiblePages(): number[] {
    if (!this.eventsPage) return [];
    
    const visiblePages = [];
    const maxVisible = 5;
    let start = Math.max(1, this.currentPage - 2);
    let end = Math.min(this.eventsPage.totalPages, start + maxVisible - 1);

    if (end - start + 1 < maxVisible) {
      start = Math.max(1, end - maxVisible + 1);
    }

    for (let i = start; i <= end; i++) {
      visiblePages.push(i);
    }

    return visiblePages;
  }

}