import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { RouterLink, Router } from '@angular/router';
import { TranslateModule, TranslateService } from '@ngx-translate/core';
import { AuthService } from '../../../core/services/auth.service';
import { ApiService } from '../../../core/services/api.service';
import { Grade } from '../../../core/models';

@Component({
  selector: 'app-register',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterLink, TranslateModule],
  template: `
    <div class="auth-page">
      <div class="auth-card animate-scale-in">
        <div class="auth-header">
          <span class="auth-icon">🎓</span>
          <h1>{{ 'AUTH.REGISTER_TITLE' | translate }}</h1>
        </div>
        <form (ngSubmit)="onRegister()" class="auth-form">
          <div class="form-row">
            <div class="form-group">
              <label>{{ 'AUTH.FIRST_NAME' | translate }}</label>
              <input type="text" [(ngModel)]="form.firstName" name="firstName" required>
            </div>
            <div class="form-group">
              <label>{{ 'AUTH.LAST_NAME' | translate }}</label>
              <input type="text" [(ngModel)]="form.lastName" name="lastName" required>
            </div>
          </div>
          <div class="form-group">
            <label>{{ 'AUTH.USERNAME' | translate }}</label>
            <input type="text" [(ngModel)]="form.userName" name="userName" required>
          </div>
          <div class="form-group">
            <label>{{ 'AUTH.EMAIL' | translate }}</label>
            <input type="email" [(ngModel)]="form.email" name="email">
          </div>
          <div class="form-group">
            <label>{{ 'AUTH.ROLE' | translate }}</label>
            <select [(ngModel)]="form.role" name="role">
              <option value="Student">{{ 'AUTH.STUDENT' | translate }}</option>
              <option value="Parent">{{ 'AUTH.PARENT' | translate }}</option>
            </select>
          </div>
          @if (form.role === 'Student') {
            <div class="form-group">
              <label>{{ 'AUTH.GRADE' | translate }}</label>
              <select [(ngModel)]="form.gradeId" name="gradeId">
                @for (grade of grades(); track grade.id) {
                  <option [ngValue]="grade.id">{{ grade.nameAr }}</option>
                }
              </select>
            </div>
          }
          <div class="form-row">
            <div class="form-group">
              <label>{{ 'AUTH.PASSWORD' | translate }}</label>
              <input type="password" [(ngModel)]="form.password" name="password" required>
            </div>
            <div class="form-group">
              <label>{{ 'AUTH.CONFIRM_PASSWORD' | translate }}</label>
              <input type="password" [(ngModel)]="form.confirmPassword" name="confirmPassword" required>
            </div>
          </div>
          @if (error) { <div class="error-msg">{{ error }}</div> }
          <button type="submit" class="btn btn-primary btn-lg w-full" [disabled]="loading">
            {{ loading ? '...' : ('AUTH.REGISTER_BTN' | translate) }}
          </button>
        </form>
        <div class="auth-divider"><span>{{ 'AUTH.OR' | translate }}</span></div>
        <a routerLink="/auth/login" class="btn btn-secondary w-full">{{ 'AUTH.HAS_ACCOUNT' | translate }}</a>
      </div>
    </div>
  `,
  styles: [`
    .auth-page { min-height:calc(100vh - 140px); display:flex; align-items:center; justify-content:center; padding:2rem; background:linear-gradient(135deg,#f5f3ff,#ede9fe); }
    .auth-card { background:white; border-radius:24px; padding:2.5rem; width:100%; max-width:520px; box-shadow:0 20px 60px rgba(0,0,0,.08); }
    .auth-header { text-align:center; margin-bottom:1.5rem; .auth-icon { font-size:3rem; display:block; margin-bottom:.5rem; } h1 { font-size:1.6rem; font-weight:800; color:#1f2937; } }
    .form-row { display:grid; grid-template-columns:1fr 1fr; gap:1rem; @media(max-width:480px){ grid-template-columns:1fr; } }
    .w-full { width:100%; }
    .auth-divider { text-align:center; margin:1.25rem 0; position:relative; &::before { content:''; position:absolute; top:50%; left:0; right:0; height:1px; background:#e5e7eb; } span { background:white; padding:0 1rem; color:#9ca3af; font-size:.85rem; position:relative; } }
    .error-msg { background:#fef2f2; color:#dc2626; padding:.7rem 1rem; border-radius:10px; font-size:.9rem; margin-bottom:1rem; text-align:center; }
  `]
})
export class RegisterComponent implements OnInit {
  form: any = { firstName: '', lastName: '', userName: '', email: '', password: '', confirmPassword: '', role: 'Student', gradeId: null, preferredLanguage: 'ar' };
  grades = signal<Grade[]>([]);
  error = '';
  loading = false;

  constructor(private auth: AuthService, private api: ApiService, private router: Router, private translate: TranslateService) {}

  ngOnInit() {
    this.api.getGrades().subscribe(res => { if (res.success) this.grades.set(res.data); });
  }

  onRegister() {
    if (this.form.password !== this.form.confirmPassword) { this.error = this.translate.instant('AUTH.PASSWORDS_MISMATCH'); return; }
    this.loading = true; this.error = '';
    this.auth.register(this.form).subscribe({
      next: res => { this.loading = false; if (res.success) this.router.navigate(['/']); else this.error = res.errors?.[0] || this.translate.instant('AUTH.ERROR'); },
      error: err => { this.loading = false; this.error = err.error?.errors?.[0] || this.translate.instant('AUTH.CONNECTION_ERROR'); }
    });
  }
}
