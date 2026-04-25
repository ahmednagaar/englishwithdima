import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { RouterLink, Router, ActivatedRoute } from '@angular/router';
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
          <p class="auth-subtitle">سجّل دخولك للمتابعة</p>
        </div>
        <form (ngSubmit)="onLogin()" class="auth-form">
          <div class="form-group" [class.error]="fieldErrors.userName">
            <label for="login-username">{{ 'AUTH.USERNAME' | translate }}</label>
            <input type="text" [(ngModel)]="form.userName" name="userName" required
              id="login-username" autocomplete="username" inputmode="text"
              [placeholder]="'اكتب اسم المستخدم'" autofocus>
            @if (fieldErrors.userName) {
              <span class="error-msg">{{ fieldErrors.userName }}</span>
            }
          </div>
          <div class="form-group" [class.error]="fieldErrors.password">
            <label for="login-password">{{ 'AUTH.PASSWORD' | translate }}</label>
            <div class="password-wrapper">
              <input [type]="showPassword ? 'text' : 'password'" [(ngModel)]="form.password" name="password" required
                id="login-password" autocomplete="current-password"
                [placeholder]="'اكتب كلمة المرور'">
              <button type="button" class="password-toggle" (click)="showPassword = !showPassword"
                [attr.aria-label]="showPassword ? 'إخفاء كلمة المرور' : 'إظهار كلمة المرور'">
                <i [class]="showPassword ? 'fas fa-eye-slash' : 'fas fa-eye'"></i>
              </button>
            </div>
            @if (fieldErrors.password) {
              <span class="error-msg">{{ fieldErrors.password }}</span>
            }
          </div>

          @if (error) {
            <div class="error-banner">
              <i class="fas fa-exclamation-circle"></i>
              {{ error }}
            </div>
          }

          <button type="submit" class="btn btn-primary btn-lg w-full" [disabled]="loading">
            @if (loading) {
              <span class="btn-spinner"></span>
            } @else {
              {{ 'AUTH.LOGIN_BTN' | translate }}
            }
          </button>
        </form>

        <div class="auth-divider"><span>{{ 'AUTH.OR' | translate }}</span></div>
        <button class="btn btn-secondary w-full" routerLink="/auth/register">{{ 'AUTH.NO_ACCOUNT' | translate }}</button>

        <button class="btn btn-guest w-full" (click)="pinMode = !pinMode">
          <i class="fas fa-key"></i>
          {{ 'AUTH.PIN_LOGIN' | translate }}
        </button>

        @if (pinMode) {
          <form (ngSubmit)="onPinLogin()" class="pin-form animate-fade-in">
            <div class="form-group">
              <label for="student-code">{{ 'AUTH.STUDENT_CODE' | translate }}</label>
              <input type="text" [(ngModel)]="pinForm.studentCode" name="studentCode"
                placeholder="EWD-1234" id="student-code">
            </div>
            <div class="form-group">
              <label for="student-pin">{{ 'AUTH.PIN' | translate }}</label>
              <input type="password" [(ngModel)]="pinForm.pin" name="pin"
                maxlength="4" id="student-pin" inputmode="numeric">
            </div>
            <button type="submit" class="btn btn-success w-full">{{ 'AUTH.LOGIN_BTN' | translate }}</button>
          </form>
        }

        <div class="auth-footer">
          <a routerLink="/" class="guest-link">أو تابع بدون حساب ←</a>
        </div>
      </div>
    </div>
  `,
  styleUrl: './login.component.scss'
})
export class LoginComponent {
  form = { userName: '', password: '' };
  pinForm = { studentCode: '', pin: '' };
  error = '';
  fieldErrors = { userName: '', password: '' };
  loading = false;
  pinMode = false;
  showPassword = false;

  constructor(
    private auth: AuthService,
    private router: Router,
    private route: ActivatedRoute,
    private translate: TranslateService
  ) {}

  onLogin() {
    this.fieldErrors = { userName: '', password: '' };
    this.error = '';

    // Client-side validation
    if (!this.form.userName.trim()) {
      this.fieldErrors['userName'] = 'اكتب اسم المستخدم';
      return;
    }
    if (!this.form.password) {
      this.fieldErrors['password'] = 'اكتب كلمة المرور';
      return;
    }

    this.loading = true;
    this.auth.login(this.form).subscribe({
      next: res => {
        this.loading = false;
        if (res.success) {
          // Redirect to returnUrl or role-based default
          const returnUrl = this.route.snapshot.queryParams['returnUrl'];
          if (returnUrl) {
            this.router.navigateByUrl(returnUrl);
          } else {
            const user = this.auth.currentUser();
            if (user?.role === 'Admin' || user?.role === 'Teacher') {
              this.router.navigate(['/admin']);
            } else {
              this.router.navigate(['/']);
            }
          }
        } else {
          this.error = res.errors?.[0] || this.translate.instant('AUTH.ERROR');
        }
      },
      error: err => {
        this.loading = false;
        this.error = err?.friendlyMessage || err.error?.errors?.[0] || this.translate.instant('AUTH.CONNECTION_ERROR');
      }
    });
  }

  onPinLogin() {
    this.auth.studentPinLogin(this.pinForm).subscribe({
      next: res => { if (res.success) this.router.navigate(['/']); else this.error = res.errors?.[0] || this.translate.instant('AUTH.ERROR'); },
      error: err => { this.error = err?.friendlyMessage || err.error?.errors?.[0] || this.translate.instant('AUTH.ERROR'); }
    });
  }
}
