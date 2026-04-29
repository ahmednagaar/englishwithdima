import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { RouterLink, Router } from '@angular/router';
import { TranslateModule, TranslateService } from '@ngx-translate/core';
import { AuthService } from '../../../core/services/auth.service';
import { ApiService } from '../../../core/services/api.service';
import { SeoService } from '../../../core/services/seo.service';
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
          <p class="auth-subtitle">أنشئ حسابك للبدء في التعلم</p>
        </div>

        <!-- Step Indicator -->
        <div class="steps">
          <div class="step" [class.active]="step === 1" [class.done]="step > 1">
            <span class="step-num">1</span>
            <span class="step-label">البيانات</span>
          </div>
          <div class="step-line" [class.active]="step > 1"></div>
          <div class="step" [class.active]="step === 2">
            <span class="step-num">2</span>
            <span class="step-label">كلمة المرور</span>
          </div>
        </div>

        <form (ngSubmit)="step === 1 ? nextStep() : onRegister()" class="auth-form">
          <!-- Step 1: Personal Info -->
          @if (step === 1) {
            <div class="form-row">
              <div class="form-group">
                <label for="reg-fname">{{ 'AUTH.FIRST_NAME' | translate }}</label>
                <input type="text" [(ngModel)]="form.firstName" name="firstName" required
                  id="reg-fname" placeholder="الاسم الأول" autofocus>
              </div>
              <div class="form-group">
                <label for="reg-lname">{{ 'AUTH.LAST_NAME' | translate }}</label>
                <input type="text" [(ngModel)]="form.lastName" name="lastName" required
                  id="reg-lname" placeholder="اسم العائلة">
              </div>
            </div>
            <div class="form-group">
              <label for="reg-uname">{{ 'AUTH.USERNAME' | translate }}</label>
              <input type="text" [(ngModel)]="form.userName" name="userName" required
                id="reg-uname" placeholder="اختر اسم مستخدم" autocomplete="username">
            </div>
            <div class="form-group">
              <label for="reg-email">{{ 'AUTH.EMAIL' | translate }}</label>
              <input type="email" [(ngModel)]="form.email" name="email"
                id="reg-email" placeholder="اختياري" inputmode="email" autocomplete="email">
            </div>
            <div class="form-group">
              <label>{{ 'AUTH.ROLE' | translate }}</label>
              <div class="role-cards">
                <button type="button" class="role-card" [class.active]="form.role === 'Student'" (click)="form.role = 'Student'">
                  <span>🎓</span> {{ 'AUTH.STUDENT' | translate }}
                </button>
                <button type="button" class="role-card" [class.active]="form.role === 'Parent'" (click)="form.role = 'Parent'">
                  <span>👨‍👩‍👧</span> {{ 'AUTH.PARENT' | translate }}
                </button>
              </div>
            </div>
            @if (form.role === 'Student') {
              <div class="form-group">
                <label for="reg-grade">{{ 'AUTH.GRADE' | translate }}</label>
                <select [(ngModel)]="form.gradeId" name="gradeId" id="reg-grade">
                  <option [ngValue]="null" disabled>اختر الصف الدراسي</option>
                  @for (grade of grades(); track grade.id) {
                    <option [ngValue]="grade.id">{{ grade.nameAr }}</option>
                  }
                </select>
              </div>
            }
            @if (stepError) { <div class="error-banner"><i class="fas fa-exclamation-circle"></i> {{ stepError }}</div> }
            <button type="submit" class="btn btn-primary btn-lg w-full">
              التالي <i class="fas fa-arrow-left" style="margin-inline-start:6px"></i>
            </button>
          }

          <!-- Step 2: Password -->
          @if (step === 2) {
            <div class="form-group">
              <label for="reg-pass">{{ 'AUTH.PASSWORD' | translate }}</label>
              <div class="password-wrapper">
                <input [type]="showPass ? 'text' : 'password'" [(ngModel)]="form.password" name="password" required
                  id="reg-pass" placeholder="6 أحرف على الأقل" autocomplete="new-password">
                <button type="button" class="password-toggle" (click)="showPass = !showPass">
                  <i [class]="showPass ? 'fas fa-eye-slash' : 'fas fa-eye'"></i>
                </button>
              </div>
              <!-- Password Strength -->
              @if (form.password) {
                <div class="pass-strength">
                  <div class="pass-bar" [style.width]="passStrength() + '%'" [class]="passStrengthClass()"></div>
                </div>
                <span class="pass-hint" [class]="passStrengthClass()">{{ passStrengthLabel() }}</span>
              }
            </div>
            <div class="form-group">
              <label for="reg-cpass">{{ 'AUTH.CONFIRM_PASSWORD' | translate }}</label>
              <input [type]="showPass ? 'text' : 'password'" [(ngModel)]="form.confirmPassword" name="confirmPassword" required
                id="reg-cpass" placeholder="أعد كتابة كلمة المرور" autocomplete="new-password">
            </div>
            @if (error) { <div class="error-banner"><i class="fas fa-exclamation-circle"></i> {{ error }}</div> }
            <button type="submit" class="btn btn-primary btn-lg w-full" [disabled]="loading">
              @if (loading) {
                <span class="btn-spinner"></span>
              } @else {
                {{ 'AUTH.REGISTER_BTN' | translate }}
              }
            </button>
            <button type="button" class="btn-back" (click)="step = 1">
              <i class="fas fa-arrow-right"></i> العودة
            </button>
          }
        </form>

        <div class="auth-divider"><span>{{ 'AUTH.OR' | translate }}</span></div>
        <a routerLink="/auth/login" class="btn btn-secondary w-full">{{ 'AUTH.HAS_ACCOUNT' | translate }}</a>
      </div>
    </div>
  `,
  styles: [`
    .auth-page { min-height:calc(100vh - 140px); display:flex; align-items:center; justify-content:center; padding:2rem; background:linear-gradient(135deg,#f5f3ff,#ede9fe); }
    .auth-card { background:white; border-radius:24px; padding:2.5rem; width:100%; max-width:520px; box-shadow:0 20px 60px rgba(0,0,0,.08);
      @media(max-width:480px) { padding:1.75rem; }
    }
    .auth-header { text-align:center; margin-bottom:1.5rem;
      .auth-icon { font-size:3rem; display:block; margin-bottom:.5rem; }
      h1 { font-size:1.6rem; font-weight:800; color:#1f2937; margin-bottom:.25rem; }
      .auth-subtitle { font-size:.9rem; color:#6b7280; }
    }

    /* Steps */
    .steps { display:flex; align-items:center; justify-content:center; gap:0; margin-bottom:2rem; }
    .step {
      display:flex; flex-direction:column; align-items:center; gap:.25rem;
      .step-num { width:32px; height:32px; border-radius:50%; background:#e5e7eb; color:#9ca3af;
        display:flex; align-items:center; justify-content:center; font-weight:700; font-size:.85rem; transition:all .3s; }
      .step-label { font-size:.75rem; color:#9ca3af; font-weight:500; }
      &.active .step-num { background:linear-gradient(135deg,#6366f1,#a855f7); color:white; }
      &.active .step-label { color:#6366f1; font-weight:600; }
      &.done .step-num { background:#10b981; color:white; }
      &.done .step-label { color:#10b981; }
    }
    .step-line { width:60px; height:2px; background:#e5e7eb; margin:0 .5rem; margin-bottom:1.25rem; transition:background .3s;
      &.active { background:linear-gradient(90deg,#10b981,#6366f1); }
    }

    /* Role Cards */
    .role-cards { display:grid; grid-template-columns:1fr 1fr; gap:.75rem; }
    .role-card {
      display:flex; align-items:center; justify-content:center; gap:.5rem;
      padding:.75rem; border:2px solid #e5e7eb; border-radius:12px;
      background:white; cursor:pointer; font-weight:600; font-size:.9rem;
      transition:all .2s; min-height:48px; color:#374151;
      span { font-size:1.3rem; }
      &:hover { border-color:#a5b4fc; }
      &.active { border-color:#6366f1; background:#eef2ff; color:#6366f1; }
    }

    /* Password */
    .password-wrapper { position:relative; input { padding-inline-end:48px; } }
    .password-toggle {
      position:absolute; top:50%; inset-inline-end:8px; transform:translateY(-50%);
      width:36px; height:36px; border-radius:8px; display:flex; align-items:center;
      justify-content:center; color:#9ca3af; min-height:36px;
      &:hover { color:#6366f1; }
    }
    .pass-strength { height:4px; background:#e5e7eb; border-radius:4px; margin-top:.5rem; overflow:hidden; }
    .pass-bar { height:100%; border-radius:4px; transition:all .3s;
      &.weak { background:#ef4444; }
      &.medium { background:#f59e0b; }
      &.strong { background:#10b981; }
    }
    .pass-hint { font-size:.75rem; font-weight:600; margin-top:.25rem; display:block;
      &.weak { color:#ef4444; }
      &.medium { color:#f59e0b; }
      &.strong { color:#10b981; }
    }

    .form-row { display:grid; grid-template-columns:1fr 1fr; gap:1rem; @media(max-width:480px){ grid-template-columns:1fr; } }
    .w-full { width:100%; }
    .error-banner { display:flex; align-items:center; gap:.5rem; background:#fef2f2; color:#dc2626; padding:.75rem 1rem; border-radius:12px; font-size:.9rem; margin-bottom:1rem; border:1px solid #fecaca; }
    .auth-divider { text-align:center; margin:1.25rem 0; position:relative;
      &::before { content:''; position:absolute; top:50%; inset-inline-start:0; inset-inline-end:0; height:1px; background:#e5e7eb; }
      span { background:white; padding:0 1rem; color:#9ca3af; font-size:.85rem; position:relative; }
    }
    .btn-back { display:block; width:100%; text-align:center; margin-top:.75rem; color:#6366f1; font-weight:600; font-size:.9rem; min-height:48px;
      i { margin-inline-end:4px; }
      &:hover { color:#4f46e5; }
    }
    .btn-spinner { display:inline-block; width:20px; height:20px; border:2.5px solid rgba(255,255,255,.3); border-top-color:white; border-radius:50%; animation:spin .7s linear infinite; }
    @keyframes spin { to { transform:rotate(360deg); } }
  `]
})
export class RegisterComponent implements OnInit {
  form: any = { firstName: '', lastName: '', userName: '', email: '', password: '', confirmPassword: '', role: 'Student', gradeId: null, preferredLanguage: 'ar' };
  grades = signal<Grade[]>([]);
  error = '';
  stepError = '';
  loading = false;
  step = 1;
  showPass = false;

  constructor(private auth: AuthService, private api: ApiService, private router: Router, private translate: TranslateService, private seo: SeoService) {}

  ngOnInit() {
    this.seo.setPage({
      titleDefault: 'إنشاء حساب',
      descriptionDefault: 'أنشئ حسابك في منصة الإنجليزية مع ديما — ابدأ رحلة تعلم الإنجليزية',
    });
    this.api.getGrades().subscribe(res => { if (res.success) this.grades.set(res.data); });
  }

  nextStep() {
    this.stepError = '';
    if (!this.form.firstName.trim() || !this.form.lastName.trim()) {
      this.stepError = 'اكتب الاسم الأول واسم العائلة';
      return;
    }
    if (!this.form.userName.trim()) {
      this.stepError = 'اختر اسم مستخدم';
      return;
    }
    if (this.form.role === 'Student' && !this.form.gradeId) {
      this.stepError = 'اختر الصف الدراسي';
      return;
    }
    this.step = 2;
  }

  passStrength(): number {
    const p = this.form.password;
    if (!p) return 0;
    let score = 0;
    if (p.length >= 6) score += 30;
    if (p.length >= 8) score += 20;
    if (/[A-Z]/.test(p)) score += 15;
    if (/[0-9]/.test(p)) score += 15;
    if (/[^A-Za-z0-9]/.test(p)) score += 20;
    return Math.min(score, 100);
  }

  passStrengthClass(): string {
    const s = this.passStrength();
    if (s < 40) return 'weak';
    if (s < 70) return 'medium';
    return 'strong';
  }

  passStrengthLabel(): string {
    const cls = this.passStrengthClass();
    if (cls === 'weak') return 'ضعيفة';
    if (cls === 'medium') return 'متوسطة';
    return 'قوية ✓';
  }

  onRegister() {
    if (this.form.password !== this.form.confirmPassword) {
      this.error = this.translate.instant('AUTH.PASSWORDS_MISMATCH');
      return;
    }
    if (this.form.password.length < 6) {
      this.error = 'كلمة المرور يجب أن تكون 6 أحرف على الأقل';
      return;
    }
    this.loading = true;
    this.error = '';
    this.auth.register(this.form).subscribe({
      next: res => {
        this.loading = false;
        if (res.success) this.router.navigate(['/']);
        else this.error = res.errors?.[0] || this.translate.instant('AUTH.ERROR');
      },
      error: err => {
        this.loading = false;
        this.error = err?.friendlyMessage || err.error?.errors?.[0] || this.translate.instant('AUTH.CONNECTION_ERROR');
      }
    });
  }
}
