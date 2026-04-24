import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { RouterLink, Router } from '@angular/router';
import { TranslateModule, TranslateService } from '@ngx-translate/core';
import { AuthService } from '../../../core/services/auth.service';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterLink, TranslateModule],
  template: `
    <div class="auth-page">
      <div class="auth-card animate-scale-in">
        <div class="auth-header">
          <span class="auth-icon">👋</span>
          <h1>{{ 'AUTH.LOGIN_TITLE' | translate }}</h1>
        </div>
        <form (ngSubmit)="onLogin()" class="auth-form">
          <div class="form-group">
            <label>{{ 'AUTH.USERNAME' | translate }}</label>
            <input type="text" [(ngModel)]="form.userName" name="userName" required>
          </div>
          <div class="form-group">
            <label>{{ 'AUTH.PASSWORD' | translate }}</label>
            <input type="password" [(ngModel)]="form.password" name="password" required>
          </div>
          @if (error) { <div class="error-msg">{{ error }}</div> }
          <button type="submit" class="btn btn-primary btn-lg w-full" [disabled]="loading">
            {{ loading ? '...' : ('AUTH.LOGIN_BTN' | translate) }}
          </button>
        </form>
        <div class="auth-divider"><span>{{ 'AUTH.OR' | translate }}</span></div>
        <button class="btn btn-secondary w-full" routerLink="/auth/register">{{ 'AUTH.NO_ACCOUNT' | translate }}</button>
        <button class="btn btn-guest w-full" (click)="pinMode = !pinMode">{{ 'AUTH.PIN_LOGIN' | translate }}</button>

        @if (pinMode) {
          <form (ngSubmit)="onPinLogin()" class="pin-form animate-fade-in">
            <div class="form-group">
              <label>{{ 'AUTH.STUDENT_CODE' | translate }}</label>
              <input type="text" [(ngModel)]="pinForm.studentCode" name="studentCode" placeholder="EWD-1234">
            </div>
            <div class="form-group">
              <label>{{ 'AUTH.PIN' | translate }}</label>
              <input type="password" [(ngModel)]="pinForm.pin" name="pin" maxlength="4">
            </div>
            <button type="submit" class="btn btn-success w-full">{{ 'AUTH.LOGIN_BTN' | translate }}</button>
          </form>
        }
      </div>
    </div>
  `,
  styleUrl: './login.component.scss'
})
export class LoginComponent {
  form = { userName: '', password: '' };
  pinForm = { studentCode: '', pin: '' };
  error = '';
  loading = false;
  pinMode = false;

  constructor(private auth: AuthService, private router: Router, private translate: TranslateService) {}

  onLogin() {
    this.loading = true;
    this.error = '';
    this.auth.login(this.form).subscribe({
      next: res => { this.loading = false; if (res.success) this.router.navigate(['/']); else this.error = res.errors?.[0] || this.translate.instant('AUTH.ERROR'); },
      error: err => { this.loading = false; this.error = err.error?.errors?.[0] || this.translate.instant('AUTH.CONNECTION_ERROR'); }
    });
  }

  onPinLogin() {
    this.auth.studentPinLogin(this.pinForm).subscribe({
      next: res => { if (res.success) this.router.navigate(['/']); else this.error = res.errors?.[0] || this.translate.instant('AUTH.ERROR'); },
      error: err => { this.error = err.error?.errors?.[0] || this.translate.instant('AUTH.ERROR'); }
    });
  }
}
