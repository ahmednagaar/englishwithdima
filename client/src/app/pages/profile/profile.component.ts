import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink, Router } from '@angular/router';
import { TranslateModule } from '@ngx-translate/core';
import { AuthService } from '../../core/services/auth.service';
import { ApiService } from '../../core/services/api.service';

@Component({
  selector: 'app-profile',
  standalone: true,
  imports: [CommonModule, RouterLink, TranslateModule],
  template: `
    <section class="profile-page">
      <div class="container">
        @if (user()) {
          <!-- Profile Hero -->
          <div class="profile-hero animate-scale-in">
            <div class="profile-avatar">{{ getInitial() }}</div>
            <h1>{{ user()!.firstName }} {{ user()!.lastName }}</h1>
            <p class="profile-role">{{ user()!.role === 'Student' ? '🎓 طالب' : user()!.role === 'Parent' ? '👨‍👩‍👧 ولي أمر' : '👨‍🏫 معلم' }}</p>
            @if (user()!.gradeName) {
              <span class="profile-grade">📚 {{ user()!.gradeName }}</span>
            }
          </div>

          <!-- Stats -->
          <div class="profile-stats animate-fade-in-up" style="animation-delay:.15s">
            <div class="pstat">
              <span class="pstat-num">{{ testsTaken() }}</span>
              <span class="pstat-label">اختبارات</span>
            </div>
            <div class="pstat">
              <span class="pstat-num">{{ totalPoints() }}</span>
              <span class="pstat-label">نقاط</span>
            </div>
            <div class="pstat">
              <span class="pstat-num">{{ avgScore() }}%</span>
              <span class="pstat-label">متوسط</span>
            </div>
            <div class="pstat">
              <span class="pstat-num">{{ streak() }}🔥</span>
              <span class="pstat-label">أيام متتالية</span>
            </div>
          </div>

          <!-- Quick Links -->
          <div class="profile-actions animate-fade-in-up" style="animation-delay:.25s">
            <a routerLink="/tests" class="profile-link">
              <span class="pl-icon">📝</span>
              <span>الاختبارات</span>
              <i class="fas fa-chevron-left"></i>
            </a>
            <a routerLink="/games" class="profile-link">
              <span class="pl-icon">🎮</span>
              <span>الألعاب التعليمية</span>
              <i class="fas fa-chevron-left"></i>
            </a>
            <a routerLink="/booking" class="profile-link">
              <span class="pl-icon">📅</span>
              <span>حجز حصة</span>
              <i class="fas fa-chevron-left"></i>
            </a>
            <a routerLink="/contact" class="profile-link">
              <span class="pl-icon">📞</span>
              <span>تواصل معنا</span>
              <i class="fas fa-chevron-left"></i>
            </a>
          </div>

          <!-- Logout -->
          <div class="profile-logout animate-fade-in-up" style="animation-delay:.35s">
            <button class="btn btn-secondary w-full" (click)="onLogout()">
              🚪 تسجيل الخروج
            </button>
          </div>
        } @else {
          <!-- Not logged in -->
          <div class="profile-empty">
            <span class="profile-empty-icon">🔑</span>
            <h2>سجّل دخولك أولاً</h2>
            <p>لعرض ملفك الشخصي ومتابعة تقدمك</p>
            <a routerLink="/auth/login" class="btn btn-primary btn-lg">تسجيل الدخول</a>
          </div>
        }
      </div>
    </section>
  `,
  styles: [`
    .profile-page { padding:2rem 0 5rem; }

    .profile-hero {
      text-align:center; padding:2.5rem; background:linear-gradient(135deg,#f5f3ff,#ede9fe);
      border-radius:24px; margin-bottom:1.5rem;
      h1 { font-size:1.8rem; font-weight:900; color:#1f2937; margin-top:1rem; }
    }
    .profile-avatar {
      width:80px; height:80px; border-radius:50%; margin:0 auto;
      background:linear-gradient(135deg,#6366f1,#a855f7); color:white;
      display:flex; align-items:center; justify-content:center;
      font-size:2rem; font-weight:900; box-shadow:0 8px 25px rgba(99,102,241,0.3);
    }
    .profile-role { color:#6b7280; margin:.25rem 0 .75rem; font-size:1rem; }
    .profile-grade {
      display:inline-block; padding:.35rem 1rem; background:white;
      border-radius:20px; font-weight:600; font-size:.9rem; color:#6366f1;
      box-shadow:0 2px 8px rgba(0,0,0,0.06);
    }

    .profile-stats {
      display:grid; grid-template-columns:repeat(4,1fr); gap:1rem; margin-bottom:1.5rem;
      @media(max-width:480px) { grid-template-columns:repeat(2,1fr); }
    }
    .pstat {
      background:white; border-radius:16px; padding:1.25rem; text-align:center;
      box-shadow:0 2px 10px rgba(0,0,0,0.04);
    }
    .pstat-num { display:block; font-size:1.5rem; font-weight:900; color:#6366f1; }
    .pstat-label { display:block; font-size:.8rem; color:#6b7280; margin-top:.25rem; }

    .profile-actions { display:flex; flex-direction:column; gap:.75rem; margin-bottom:1.5rem; }
    .profile-link {
      display:flex; align-items:center; gap:1rem; background:white;
      border-radius:14px; padding:1rem 1.25rem; text-decoration:none;
      box-shadow:0 2px 8px rgba(0,0,0,0.04); transition:all .2s;
      min-height:56px;
      span { flex:1; font-weight:600; color:#374151; }
      .pl-icon { font-size:1.3rem; flex:none; }
      i { color:#9ca3af; transition:transform .2s; }
      &:hover { transform:translateX(-4px); box-shadow:0 4px 16px rgba(0,0,0,0.08); i { color:#6366f1; } }
    }

    .profile-logout { max-width:400px; margin:0 auto; }
    .w-full { width:100%; }

    .profile-empty {
      text-align:center; padding:4rem 2rem; min-height:60vh;
      display:flex; flex-direction:column; align-items:center; justify-content:center;
      .profile-empty-icon { font-size:5rem; margin-bottom:1rem; }
      h2 { font-size:1.4rem; font-weight:700; margin-bottom:.5rem; }
      p { color:#6b7280; margin-bottom:1.5rem; }
    }
  `]
})
export class ProfileComponent implements OnInit {
  user = signal<any>(null);
  testsTaken = signal(0);
  totalPoints = signal(0);
  avgScore = signal(0);
  streak = signal(0);

  constructor(
    private auth: AuthService,
    private api: ApiService,
    private router: Router
  ) {}

  ngOnInit() {
    const currentUser = this.auth.currentUser();
    if (currentUser) {
      this.user.set(currentUser);
      this.loadStudentStats();
    }
  }

  getInitial(): string {
    return this.user()?.firstName?.charAt(0)?.toUpperCase() || '?';
  }

  onLogout() {
    this.auth.logout();
    this.router.navigate(['/']);
  }

  private loadStudentStats() {
    // Try to load test history for stats
    try {
      const history = JSON.parse(localStorage.getItem('ewd_test_history') || '[]');
      this.testsTaken.set(history.length);
      if (history.length > 0) {
        const total = history.reduce((s: number, h: any) => s + (h.score || 0), 0);
        const avg = history.reduce((s: number, h: any) => s + (h.percentage || 0), 0) / history.length;
        this.totalPoints.set(total);
        this.avgScore.set(Math.round(avg));
      }
    } catch {}

    // Calculate streak from login dates
    try {
      const dates = JSON.parse(localStorage.getItem('ewd_login_dates') || '[]');
      this.streak.set(this.calculateStreak(dates));
    } catch {}
  }

  private calculateStreak(dates: string[]): number {
    if (dates.length === 0) return 0;
    const today = new Date().toISOString().split('T')[0];
    const sorted = [...new Set(dates)].sort().reverse();
    let streak = 0;
    const date = new Date(today);
    for (const d of sorted) {
      const expected = date.toISOString().split('T')[0];
      if (d === expected) {
        streak++;
        date.setDate(date.getDate() - 1);
      } else break;
    }
    return streak;
  }
}
