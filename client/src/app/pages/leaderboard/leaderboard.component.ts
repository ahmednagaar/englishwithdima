import { Component, OnInit, signal, computed } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { TranslateModule } from '@ngx-translate/core';
import { HttpClient } from '@angular/common/http';
import { AuthService } from '../../core/services/auth.service';
import { environment } from '../../../environments/environment';

interface LeaderboardEntry {
  rank: number;
  userId: string;
  displayName: string;
  avatarUrl?: string;
  totalPoints: number;
  badgeCount: number;
}

@Component({
  selector: 'app-leaderboard',
  standalone: true,
  imports: [CommonModule, RouterLink, TranslateModule],
  template: `
    <section class="leaderboard-page">
      <div class="container">
        <div class="page-header animate-fade-in-up">
          <h1>🏆 لوحة المتصدرين</h1>
          <p>تنافس مع زملائك واحصل على المركز الأول!</p>
        </div>

        <!-- Grade Tabs -->
        <div class="grade-tabs animate-fade-in-up" style="animation-delay:.1s">
          <div class="tabs-scroll">
            @for (grade of grades; track grade.id) {
              <button class="grade-tab"
                [class.active]="selectedGradeId() === grade.id"
                (click)="selectGrade(grade.id)">
                {{ grade.label }}
              </button>
            }
          </div>
        </div>

        @if (loading()) {
          <!-- Skeleton -->
          <div class="podium-skeleton animate-fade-in-up">
            <div class="skeleton" style="width:100px;height:130px;border-radius:16px"></div>
            <div class="skeleton" style="width:100px;height:160px;border-radius:16px"></div>
            <div class="skeleton" style="width:100px;height:110px;border-radius:16px"></div>
          </div>
          <div style="margin-top:1.5rem">
            @for (i of [1,2,3,4,5]; track i) {
              <div class="row-skeleton">
                <div class="skeleton" style="width:36px;height:36px;border-radius:50%"></div>
                <div class="skeleton" style="width:44px;height:44px;border-radius:50%"></div>
                <div class="skeleton skeleton-title" style="width:60%"></div>
                <div class="skeleton" style="width:60px;height:20px;border-radius:8px"></div>
              </div>
            }
          </div>
        } @else {
          <!-- Podium (Top 3) -->
          @if (topThree().length > 0) {
            <div class="podium animate-fade-in-up" style="animation-delay:.15s">
              @if (topThree().length > 1) {
                <div class="podium-place second">
                  <span class="rank-medal">🥈</span>
                  <div class="podium-avatar">{{ getInitial(topThree()[1]) }}</div>
                  <span class="podium-name">{{ topThree()[1].displayName }}</span>
                  <span class="podium-points">{{ topThree()[1].totalPoints }} نقطة</span>
                  <span class="rank-badge">#2</span>
                </div>
              }
              <div class="podium-place first">
                <span class="crown">👑</span>
                <span class="rank-medal">🥇</span>
                <div class="podium-avatar gold">{{ getInitial(topThree()[0]) }}</div>
                <span class="podium-name">{{ topThree()[0].displayName }}</span>
                <span class="podium-points">{{ topThree()[0].totalPoints }} نقطة</span>
                <span class="rank-badge">#1</span>
              </div>
              @if (topThree().length > 2) {
                <div class="podium-place third">
                  <span class="rank-medal">🥉</span>
                  <div class="podium-avatar">{{ getInitial(topThree()[2]) }}</div>
                  <span class="podium-name">{{ topThree()[2].displayName }}</span>
                  <span class="podium-points">{{ topThree()[2].totalPoints }} نقطة</span>
                  <span class="rank-badge">#3</span>
                </div>
              }
            </div>
          }

          <!-- Rows 4+ -->
          @if (restEntries().length > 0) {
            <div class="leaderboard-list animate-fade-in-up" style="animation-delay:.25s">
              @for (entry of restEntries(); track entry.userId) {
                <div class="leaderboard-row" [class.current-user]="isCurrentUser(entry)">
                  <span class="rank-number">{{ entry.rank }}</span>
                  <div class="row-avatar">{{ getInitial(entry) }}</div>
                  <div class="row-info">
                    <span class="row-name">{{ entry.displayName }}</span>
                    @if (entry.badgeCount > 0) {
                      <span class="badge-count">🏅 {{ entry.badgeCount }}</span>
                    }
                  </div>
                  <span class="row-points">{{ entry.totalPoints }} نقطة</span>
                </div>
              }
            </div>
          }

          <!-- Empty -->
          @if (entries().length === 0) {
            <div class="empty-state">
              <span style="font-size:4rem">🏆</span>
              <h3>لا يوجد متصدرون بعد</h3>
              <p>كن أول من يحصل على نقاط في هذا الصف!</p>
              <a routerLink="/grades" class="btn btn-primary">ابدأ اختبارًا</a>
            </div>
          }
        }
      </div>
    </section>
  `,
  styles: [`
    .leaderboard-page { padding:2rem 0 5rem; }
    .page-header { text-align:center; margin-bottom:2rem;
      h1 { font-size:2rem; font-weight:900; background:linear-gradient(135deg,#f59e0b,#ef4444); -webkit-background-clip:text; -webkit-text-fill-color:transparent; background-clip:text; }
      p { color:#6b7280; margin-top:.25rem; }
    }

    /* Grade Tabs */
    .grade-tabs { margin-bottom:2rem; }
    .tabs-scroll {
      display:flex; gap:.5rem; overflow-x:auto; padding-bottom:8px;
      -webkit-overflow-scrolling:touch; scrollbar-width:none;
      &::-webkit-scrollbar { display:none; }
    }
    .grade-tab {
      white-space:nowrap; padding:.5rem 1rem; border-radius:25px;
      border:2px solid #e5e7eb; font-weight:600; font-size:.85rem;
      color:#6b7280; transition:all .2s; min-height:40px; background:white;
      &.active { background:linear-gradient(135deg,#6366f1,#a855f7); color:white; border-color:transparent; }
      &:hover:not(.active) { border-color:#a5b4fc; }
    }

    /* Podium */
    .podium {
      display:flex; align-items:flex-end; justify-content:center; gap:8px; padding:24px 16px 0;
    }
    .podium-place {
      display:flex; flex-direction:column; align-items:center;
      border-radius:16px 16px 0 0; padding:16px 12px; min-width:100px;
      &.first  { background:linear-gradient(135deg,#FFD700,#FFA500); height:180px; order:2; }
      &.second { background:linear-gradient(135deg,#C0C0C0,#A0A0A0); height:140px; order:1; }
      &.third  { background:linear-gradient(135deg,#CD7F32,#A0522D); height:120px; order:3; }
      @media(max-width:480px) {
        min-width:80px; padding:12px 8px;
        &.first { height:150px; }
        &.second { height:115px; }
        &.third { height:100px; }
      }
    }
    .crown { font-size:28px; margin-bottom:2px; animation:bounce 2s infinite; }
    .rank-medal { font-size:24px; }
    .podium-avatar {
      width:50px; height:50px; border-radius:50%; background:rgba(255,255,255,0.3);
      color:white; display:flex; align-items:center; justify-content:center;
      font-weight:900; font-size:1.2rem; border:3px solid white; margin:6px 0;
      &.gold { background:rgba(255,255,255,0.4); }
    }
    .podium-name { font-size:12px; font-weight:700; color:white; text-align:center; max-width:90px; overflow:hidden; text-overflow:ellipsis; white-space:nowrap; }
    .podium-points { font-size:11px; color:rgba(255,255,255,0.85); }
    .rank-badge { font-size:18px; font-weight:900; color:white; margin-top:auto; }

    @keyframes bounce {
      0%,100% { transform:translateY(0); }
      50% { transform:translateY(-6px); }
    }

    /* List */
    .leaderboard-list {
      background:white; border-radius:16px; overflow:hidden;
      box-shadow:0 2px 12px rgba(0,0,0,0.04); margin-top:1.5rem;
    }
    .leaderboard-row {
      display:flex; align-items:center; gap:12px;
      padding:14px 16px; border-bottom:1px solid #f3f4f6;
      transition:background .2s;
      &:last-child { border-bottom:none; }
      &:hover { background:#f9fafb; }
      &.current-user {
        background:#eef2ff; border:2px solid #6366f1; border-radius:12px;
        margin:4px 8px; position:sticky; bottom:8px;
      }
    }
    .rank-number { font-size:18px; font-weight:700; min-width:36px; color:#9ca3af; text-align:center; }
    .row-avatar {
      width:44px; height:44px; border-radius:50%;
      background:linear-gradient(135deg,#6366f1,#a855f7); color:white;
      display:flex; align-items:center; justify-content:center;
      font-weight:700; font-size:.9rem; flex-shrink:0;
    }
    .row-info { flex:1; display:flex; flex-direction:column; gap:2px; }
    .row-name { font-size:15px; font-weight:600; color:#1f2937; }
    .badge-count { font-size:12px; color:#6b7280; }
    .row-points { font-size:14px; font-weight:700; color:#6366f1; white-space:nowrap; }

    /* Skeletons */
    .podium-skeleton { display:flex; align-items:flex-end; justify-content:center; gap:12px; }
    .row-skeleton { display:flex; align-items:center; gap:12px; padding:14px 16px; }

    /* Empty */
    .empty-state {
      text-align:center; padding:4rem 2rem;
      h3 { font-size:1.3rem; font-weight:700; margin:.5rem 0; }
      p { color:#6b7280; margin-bottom:1.5rem; }
    }
  `]
})
export class LeaderboardComponent implements OnInit {
  entries = signal<LeaderboardEntry[]>([]);
  loading = signal(true);
  selectedGradeId = signal(1);

  grades = [
    { id: 1, label: 'الصف 1 ابتدائي' }, { id: 2, label: 'الصف 2 ابتدائي' },
    { id: 3, label: 'الصف 3 ابتدائي' }, { id: 4, label: 'الصف 4 ابتدائي' },
    { id: 5, label: 'الصف 5 ابتدائي' }, { id: 6, label: 'الصف 6 ابتدائي' },
    { id: 7, label: 'الصف 1 إعدادي' }, { id: 8, label: 'الصف 2 إعدادي' },
    { id: 9, label: 'الصف 3 إعدادي' },
  ];

  topThree = computed(() => this.entries().slice(0, 3));
  restEntries = computed(() => this.entries().slice(3));

  constructor(
    private http: HttpClient,
    private auth: AuthService
  ) {}

  ngOnInit() {
    // Default to user's grade
    const user = this.auth.currentUser();
    if (user?.gradeId) {
      this.selectedGradeId.set(user.gradeId);
    }
    this.loadLeaderboard();
  }

  selectGrade(id: number) {
    this.selectedGradeId.set(id);
    this.loadLeaderboard();
  }

  getInitial(entry: LeaderboardEntry): string {
    return entry.displayName?.charAt(0)?.toUpperCase() || '?';
  }

  isCurrentUser(entry: LeaderboardEntry): boolean {
    return entry.userId === this.auth.currentUser()?.userId;
  }

  private loadLeaderboard() {
    this.loading.set(true);
    // Try actual API endpoint — gracefully fallback to empty if it doesn't exist yet
    this.http.get<any>(`${environment.apiUrl}/leaderboard?gradeId=${this.selectedGradeId()}`).subscribe({
      next: (res: any) => {
        if (res.success && Array.isArray(res.data)) {
          this.entries.set(res.data.map((e: any, i: number) => ({ ...e, rank: i + 1 })));
        } else {
          this.entries.set([]);
        }
        this.loading.set(false);
      },
      error: () => {
        // API endpoint not ready yet — show empty state
        this.entries.set([]);
        this.loading.set(false);
      }
    });
  }
}
