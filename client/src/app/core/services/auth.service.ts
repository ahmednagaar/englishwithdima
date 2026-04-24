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
  private guestKey = 'ewd_guest';

  // Signals for reactive state
  private _currentUser = signal<AuthResponse | null>(this.loadUser());
  readonly currentUser = this._currentUser.asReadonly();
  readonly isLoggedIn = computed(() => !!this._currentUser());
  readonly userRole = computed(() => this._currentUser()?.role || 'Guest');
  readonly isAdmin = computed(() => ['Admin', 'SuperAdmin'].includes(this.userRole()));
  readonly isTeacher = computed(() => this.userRole() === 'Teacher' || this.isAdmin());

  // Guest state
  private _guestSession = signal<GuestSession | null>(this.loadGuest());
  readonly guestSession$ = this._guestSession.asReadonly();
  readonly isGuest = computed(() => !!this._guestSession());
  readonly guestName = computed(() => this._guestSession()?.displayName || '');

  // Combined: either authenticated or guest
  readonly isAuthenticated = computed(() => this.isLoggedIn() || this.isGuest());
  readonly displayName = computed(() => {
    const user = this._currentUser();
    if (user) return `${user.firstName} ${user.lastName}`;
    const guest = this._guestSession();
    if (guest) return guest.displayName;
    return '';
  });

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

  /** Store a guest session returned from the API */
  handleGuestSession(data: GuestSession): void {
    this.clearAuth(); // Clear any existing auth
    localStorage.setItem(this.guestKey, JSON.stringify(data));
    this._guestSession.set(data);
  }

  /** Create a local-only guest session (when API is unavailable) */
  setLocalGuest(displayName: string, gradeId: number): void {
    this.clearAuth(); // Clear any existing auth
    const localGuest: GuestSession = {
      sessionId: 'local-' + Date.now(),
      displayName,
      gradeId,
      sessionToken: '',
      expiresAt: new Date(Date.now() + 24 * 60 * 60 * 1000) // 24 hours
    };
    localStorage.setItem(this.guestKey, JSON.stringify(localGuest));
    this._guestSession.set(localGuest);
  }

  /** Clear guest session */
  clearGuest(): void {
    localStorage.removeItem(this.guestKey);
    this._guestSession.set(null);
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
    this.clearGuest();
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
    this.clearGuest(); // Clear guest when doing real auth
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

  private loadGuest(): GuestSession | null {
    try {
      const stored = localStorage.getItem(this.guestKey);
      if (!stored) return null;
      const guest: GuestSession = JSON.parse(stored);
      // Check expiry
      if (new Date(guest.expiresAt) <= new Date()) {
        localStorage.removeItem(this.guestKey);
        return null;
      }
      return guest;
    } catch { return null; }
  }
}
