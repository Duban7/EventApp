import { Injectable } from "@angular/core";
import { HttpClient, HttpParams } from "@angular/common/http";
import { User } from "../models/user";
import { AuthResponse } from "../models/authResponse";
import { BehaviorSubject, Observable, throwError } from 'rxjs';
import { catchError, map, tap } from 'rxjs/operators';
import { Router } from "@angular/router";
import { Page } from "../models/page";
import { Event } from "../models/event";
import { Participant } from "../models/participant";

@Injectable({
    providedIn: 'root'
})
export class EventService{
    private apiUrl = 'EventApi/event';
    private userApiUrl = 'EventApi/user';

    constructor(private http: HttpClient){

    }

    getEvents(pageNumber: number = 1, pageSize: number = 10) : Observable<Page<Event>>{
        const url = this.apiUrl+'/get-all/'+pageNumber.toString()+'/'+pageSize.toString();
        return this.http.get<Page<Event>>(url);
    }

    getEventsFiltered(filter: {
        name: string | null,
        category: string | null,
        startDate: string | null,
        eventPlace: string| null
    }, pageNumber: number = 1, pageSize: number = 10) : Observable<Page<Event>>{
       
        const url = this.apiUrl+'/get-filtered/'+pageNumber.toString()+'/'+pageSize.toString();
        let params = new HttpParams();
        if(filter){
            console.log(filter);
            if(filter.name)  params = params.set('Name', filter.name)
                console.log(typeof(filter.startDate));
            if(filter.category) params = params.set('Category', filter.category)
            if(filter.startDate) params = params.set('StartDate', filter.startDate)
            if(filter.eventPlace) params = params.set('EventPlace', filter.eventPlace)
        }
    console.log(params);
        return this.http.get<Page<Event>>(url, {params});
    }

    getEventDetails(eventId:number): Observable<Event>{
        return this.http.get<Event>(this.apiUrl+'/'+eventId.toString());
    }
    
    getEventParticipants(eventId:number) : Observable<Participant[]>{
        return this.http.get<Participant[]>(this.userApiUrl+'/get-event-participants/'+eventId.toString());
    }

    registerOnEvent(eventId: number): Observable<any>{
        return this.http.post(this.userApiUrl+'/reg-event/'+eventId.toString(),{});
    }

    unregisterFromEvent(eventId: number): Observable<any>{
        return this.http.post(this.userApiUrl+'/unreg-event/'+eventId.toString(),{});
    }
}