import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { TranslateModule } from '@ngx-translate/core';
import { ApiService } from '../../../core/services/api.service';

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [CommonModule, RouterLink, TranslateModule],
  template: `
    <section class="dashboard-page">
      <div class="container">
        <div class="dash-header animate-fade-in-up">
          <h1>{{ 'NAV.DASHBOARD' | translate }}</h1>
          <p>{{ 'ADMIN.DESC' | translate }}</p>
        </div>

        <!-- Stats Overview -->
        <div class="stats-grid animate-fade-in-up">
          @for (stat of stats(); track stat.key) {
            <div class="stat-card" [style.border-top-color]="stat.color">
              <span class="stat-icon">{{ stat.icon }}</span>
              <div class="stat-value">{{ stat.value }}</div>
              <div class="stat-label">{{ 'ADMIN.' + stat.key | translate }}</div>
            </div>
          }
        </div>

        <!-- Quick Actions -->
        <h2 class="section-subtitle-dash">{{ 'ADMIN.QUICK_ACTIONS' | translate }}</h2>
        <div class="actions-grid animate-fade-in-up" style="animation-delay:.15s">
          @for (action of quickActions; track action.key) {
            <a [routerLink]="action.route" class="action-card" [style.--accent]="action.color">
              <div class="action-icon" [style.background]="action.color + '15'" [style.color]="action.color">{{ action.icon }}</div>
              <h3>{{ 'ADMIN.' + action.key | translate }}</h3>
              <p>{{ 'ADMIN.' + action.descKey | translate }}</p>
              <span class="action-arrow">←</span>
            </a>
          }
        </div>

        <!-- Recent Activity -->
        <h2 class="section-subtitle-dash" style="margin-top:2.5rem">{{ 'ADMIN.RECENT_ACTIVITY' | translate }}</h2>
        <div class="activity-card animate-fade-in-up" style="animation-delay:.25s">
          <div class="activity-list">
            @for (item of recentActivity; track item.key) {
              <div class="activity-item">
                <span class="activity-icon">{{ item.icon }}</span>
                <div class="activity-info">
                  <p>{{ 'ADMIN.' + item.key | translate }}</p>
                  <span class="activity-time">{{ 'ADMIN.' + item.timeKey | translate }}</span>
                </div>
              </div>
            }
          </div>
        </div>
      </div>
    </section>
  `,
  styles: [`
    .dashboard-page { padding:2rem 0 5rem; }
    .dash-header { margin-bottom:2rem; h1 { font-size:2rem; font-weight:900; background:linear-gradient(135deg,#6366f1,#a855f7); -webkit-background-clip:text; -webkit-text-fill-color:transparent; } p { color:#6b7280; margin-top:.25rem; } }
    .stats-grid { display:grid; grid-template-columns:repeat(4,1fr); gap:1.25rem; margin-bottom:2.5rem; @media(max-width:768px){ grid-template-columns:repeat(2,1fr); } }
    .stat-card { background:white; border-radius:16px; padding:1.5rem; border-top:4px solid; box-shadow:0 2px 12px rgba(0,0,0,.04); text-align:center; transition:all .3s;
      &:hover { transform:translateY(-3px); box-shadow:0 8px 25px rgba(0,0,0,.08); }
      .stat-icon { font-size:2rem; }
      .stat-value { font-size:2rem; font-weight:900; color:#1f2937; margin:.5rem 0 .25rem; }
      .stat-label { font-size:.85rem; color:#6b7280; font-weight:500; }
    }
    .section-subtitle-dash { font-size:1.2rem; font-weight:700; color:#1f2937; margin-bottom:1rem; }
    .actions-grid { display:grid; grid-template-columns:repeat(4,1fr); gap:1.25rem; @media(max-width:1024px){ grid-template-columns:repeat(2,1fr); } @media(max-width:600px){ grid-template-columns:1fr; } }
    .action-card { background:white; border-radius:16px; padding:1.5rem; text-decoration:none; box-shadow:0 2px 12px rgba(0,0,0,.04); transition:all .3s; position:relative; overflow:hidden;
      &:hover { transform:translateY(-4px); box-shadow:0 10px 30px rgba(0,0,0,.08); .action-arrow { opacity:1; transform:translateX(0); } }
      .action-icon { width:48px; height:48px; border-radius:12px; display:flex; align-items:center; justify-content:center; font-size:1.5rem; margin-bottom:.75rem; }
      h3 { font-size:1rem; font-weight:700; color:#1f2937; margin-bottom:.25rem; }
      p { font-size:.8rem; color:#6b7280; }
      .action-arrow { position:absolute; top:1.5rem; inset-inline-end:1.5rem; font-size:1.2rem; color:#6366f1; opacity:0; transform:translateX(5px); transition:all .3s; }
    }
    .activity-card { background:white; border-radius:16px; padding:1.5rem; box-shadow:0 2px 12px rgba(0,0,0,.04); }
    .activity-item { display:flex; align-items:flex-start; gap:.75rem; padding:.75rem 0; border-bottom:1px solid #f3f4f6;
      &:last-child { border-bottom:none; }
      .activity-icon { font-size:1.3rem; margin-top:.1rem; }
      p { font-size:.9rem; color:#374151; font-weight:500; }
      .activity-time { font-size:.75rem; color:#9ca3af; }
    }
  `]
})
export class DashboardComponent implements OnInit {
  stats = signal([
    { icon: '👨‍🎓', value: '—', key: 'STUDENTS', color: '#6366f1' },
    { icon: '📝', value: '—', key: 'QUESTIONS_LABEL', color: '#a855f7' },
    { icon: '📋', value: '—', key: 'TESTS_LABEL', color: '#06b6d4' },
    { icon: '🎮', value: '—', key: 'GAMES_LABEL', color: '#10b981' },
  ]);

  quickActions = [
    { icon: '➕', key: 'ADD_QUESTION', descKey: 'ADD_QUESTION_DESC', route: '/admin/questions', color: '#6366f1' },
    { icon: '📋', key: 'CREATE_TEST', descKey: 'CREATE_TEST_DESC', route: '/admin/tests', color: '#a855f7' },
    { icon: '🎮', key: 'MANAGE_GAMES', descKey: 'MANAGE_GAMES_DESC', route: '/admin/games', color: '#06b6d4' },
    { icon: '📊', key: 'VIEW_REPORTS', descKey: 'VIEW_REPORTS_DESC', route: '/admin', color: '#f59e0b' },
  ];

  recentActivity = [
    { icon: '📝', key: 'WELCOME_MSG', timeKey: 'JUST_NOW' },
    { icon: '🚀', key: 'READY_MSG', timeKey: 'GETTING_STARTED' },
    { icon: '💡', key: 'TIP_MSG', timeKey: 'TIP' },
  ];

  constructor(private api: ApiService) {}

  ngOnInit() {
    // Fetch real counts for dashboard stats
    this.api.getQuestions().subscribe(r => {
      if (r.success) {
        this.updateStat('QUESTIONS_LABEL', r.data?.length?.toString() || '0');
      }
    });

    this.api.getTests().subscribe(r => {
      if (r.success) {
        this.updateStat('TESTS_LABEL', r.data?.length?.toString() || '0');
      }
    });

    // Count games
    let gameCount = 0;
    this.api.getMatchingGames().subscribe(r => {
      if (r.success) { gameCount += r.data?.length || 0; this.updateStat('GAMES_LABEL', gameCount.toString()); }
    });
    this.api.getDragDropGames().subscribe(r => {
      if (r.success) { gameCount += r.data?.length || 0; this.updateStat('GAMES_LABEL', gameCount.toString()); }
    });
    this.api.getFlipCardGames().subscribe(r => {
      if (r.success) { gameCount += r.data?.length || 0; this.updateStat('GAMES_LABEL', gameCount.toString()); }
    });
  }

  private updateStat(key: string, value: string) {
    this.stats.set(this.stats().map(s => s.key === key ? { ...s, value } : s));
  }
}
