import { Injectable, signal, computed } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Router } from '@angular/router';
import { Observable, tap, BehaviorSubject } from 'rxjs';
import { environment } from '../../../environments/environment';
import { ApiResponse, AuthResponse, LoginRequest, RegisterRequest, StudentPinLoginRequest, GuestCreateRequest, GuestSession, UserProfile } from '../models';

@Injectable({ providedIn: 'root' })
export class AuthService {
  private readonly apiUrl = `${environment.apiUrl}/auth`;
  private tokenKey = 'ewd_access_token';
  private refreshKey = 'ewd_refresh_token';
  private userKey = 'ewd_user';

  // Signals for reactive state
  private _currentUser = signal<AuthResponse | null>(this.loadUser());
  readonly currentUser = this._currentUser.asReadonly();
  readonly isLoggedIn = computed(() => !!this._currentUser());
  readonly userRole = computed(() => this._currentUser()?.role || 'Guest');
  readonly isAdmin = computed(() => ['Admin', 'SuperAdmin'].includes(this.userRole()));
  readonly isTeacher = computed(() => this.userRole() === 'Teacher' || this.isAdmin());

  constructor(private http: HttpClient, private router: Router) {}

  register(data: RegisterRequest): Observable<ApiResponse<AuthResponse>> {
    return this.http.post<ApiResponse<AuthResponse>>(`${this.apiUrl}/register`, data)
      .pipe(tap(res => { if (res.success) this.handleAuth(res.data); }));
  }

  login(data: LoginRequest): Observable<ApiResponse<AuthResponse>> {
    return this.http.post<ApiResponse<AuthResponse>>(`${this.apiUrl}/login`, data)
      .pipe(tap(res => { if (res.success) this.handleAuth(res.data); }));
  }

  studentPinLogin(data: StudentPinLoginRequest): Observable<ApiResponse<AuthResponse>> {
    return this.http.post<ApiResponse<AuthResponse>>(`${this.apiUrl}/student-login`, data)
      .pipe(tap(res => { if (res.success) this.handleAuth(res.data); }));
  }

  guestSession(data: GuestCreateRequest): Observable<ApiResponse<GuestSession>> {
    return this.http.post<ApiResponse<GuestSession>>(`${this.apiUrl}/guest`, data);
  }

  refreshToken(): Observable<ApiResponse<AuthResponse>> {
    const refreshToken = localStorage.getItem(this.refreshKey) || '';
    return this.http.post<ApiResponse<AuthResponse>>(`${this.apiUrl}/refresh`, { refreshToken })
      .pipe(tap(res => { if (res.success) this.handleAuth(res.data); }));
  }

  getProfile(): Observable<ApiResponse<UserProfile>> {
    return this.http.get<ApiResponse<UserProfile>>(`${this.apiUrl}/me`);
  }

  logout(): void {
    const refreshToken = localStorage.getItem(this.refreshKey);
    if (refreshToken) {
      this.http.post(`${this.apiUrl}/logout`, { refreshToken }).subscribe();
    }
    this.clearAuth();
    this.router.navigate(['/']);
  }

  getToken(): string | null {
    return localStorage.getItem(this.tokenKey);
  }

  isTokenExpired(): boolean {
    const user = this._currentUser();
    if (!user) return true;
    return new Date(user.accessTokenExpiry) <= new Date();
  }

  private handleAuth(data: AuthResponse): void {
    localStorage.setItem(this.tokenKey, data.accessToken);
    localStorage.setItem(this.refreshKey, data.refreshToken);
    localStorage.setItem(this.userKey, JSON.stringify(data));
    this._currentUser.set(data);
  }

  private clearAuth(): void {
    localStorage.removeItem(this.tokenKey);
    localStorage.removeItem(this.refreshKey);
    localStorage.removeItem(this.userKey);
    this._currentUser.set(null);
  }

  private loadUser(): AuthResponse | null {
    try {
      const stored = localStorage.getItem(this.userKey);
      return stored ? JSON.parse(stored) : null;
    } catch { return null; }
  }
}
