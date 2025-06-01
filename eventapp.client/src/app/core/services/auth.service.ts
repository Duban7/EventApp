import { Injectable } from "@angular/core";
import { HttpClient } from "@angular/common/http";
import { User } from "../models/user";
import { AuthResponse } from "../models/authResponse";
import { BehaviorSubject, Observable, throwError } from 'rxjs';
import { catchError, map, tap } from 'rxjs/operators';
import { Router } from "@angular/router";

@Injectable({
    providedIn: "root"
})
export class AuthService{
    private readonly apiUrl = "https://localhost:8081/EventApi/user";
    private currentUserSubject: BehaviorSubject<User | null>;
     public currentUser: Observable<User | null>;

    constructor(private http: HttpClient,
                private router: Router) {
        const storedUser = localStorage.getItem('currentUser');
        this.currentUserSubject = new BehaviorSubject<User | null>(
          storedUser ? JSON.parse(storedUser) : null
        );
        this.currentUser = this.currentUserSubject.asObservable();
    }

    public get currentUserValue(): User | null {
        return this.currentUserSubject.value;
    }

    public get userId(): string | null {
        return this.currentUserValue?.id || null;
    }

     public get isLoggedIn(): boolean {
        const token = localStorage.getItem('accessToken');
        return !!token;
    }

    public get isAdmin(): boolean {
        if (!this.isLoggedIn) return false;
        const roles = localStorage.getItem('roles');
        return roles?.includes('Admin') || false;
    }

    
    logIn( email : string, password : string) : Observable<User>{
         const formData = new FormData();
        formData.append('email', email);
        formData.append('password', password);

        return this.http.post<AuthResponse>(this.apiUrl+"/log-in", formData)
         .pipe(
            tap(response => {
                this.storeAuthData(response);
                this.currentUserSubject.next(response.user);
            }),
            map(response => response.user),
            catchError(error => {
                this.clearAuthData();
                return throwError(() => error);
            })
      );
    }

    register(userData: {
            name: string;
            surname: string;
            email: string;
            password: string;
            dateOfBirth: Date;
    }): Observable<User> {
        const formData = new FormData();
        formData.append('name', userData.name);
        formData.append('surname', userData.surname);
        formData.append('email', userData.email);
        formData.append('password', userData.password);
        formData.append('birthDate', userData.dateOfBirth.toISOString());
        
        return this.http.post<AuthResponse>(`${this.apiUrl}/sign-up`, formData)
        .pipe(
            tap(response => {
                this.storeAuthData(response);
                this.currentUserSubject.next(response.user);
            }),
            map(response => response.user),
            catchError(error => {
                this.clearAuthData();
                return throwError(() => error);
            })
        );
    }
    
    logout(): void {
        this.clearAuthData();
        this.currentUserSubject.next(null);
        this.router.navigate(['/login']);
    }

    updateCurrentUser(user: User): void {
        this.currentUserSubject.next(user);
        localStorage.setItem('currentUser', JSON.stringify(user));
    }

    private storeAuthData(authResponse: AuthResponse): void {
        localStorage.setItem('currentUser', JSON.stringify(authResponse.user));
        localStorage.setItem('accessToken', authResponse.token);
        localStorage.setItem('roles', authResponse.roles.toString());
        localStorage.setItem('refreshToken', authResponse.user.refreshToken);
        localStorage.setItem('refreshTokenExpiresAt', authResponse.user.refreshExpires.toString());
    }

    private clearAuthData(): void {
        localStorage.removeItem('currentUser');
        localStorage.removeItem('accessToken');
        localStorage.removeItem('refreshToken');
        localStorage.removeItem('refreshTokenExpiresAt');
    }
}
