import { Component, OnInit, signal, effect } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink, Router } from '@angular/router';
import { TranslateModule, TranslateService } from '@ngx-translate/core';
import { ApiService } from '../../../core/services/api.service';
import { SeoService } from '../../../core/services/seo.service';
import { Grade } from '../../../core/models';

@Component({
  selector: 'app-game-hub',
  standalone: true,
  imports: [CommonModule, RouterLink, TranslateModule],
  template: `
    <section class="game-hub">
      <div class="container">
        <div class="page-header animate-fade-in-up">
          <h1 class="section-title">{{ 'GAMES.TITLE' | translate }}</h1>
          <p class="section-subtitle">{{ 'GAMES.SUBTITLE' | translate }}</p>
        </div>

        <!-- Grade Filter -->
        <div class="grade-filter animate-fade-in-up">
          <label>{{ 'GAMES.SELECT_GRADE' | translate }}</label>
          <div class="filter-chips">
            @for (grade of grades(); track grade.id) {
              <button class="chip" [class.active]="selectedGrade() === grade.id" (click)="selectGrade(grade.id)">{{ grade.nameAr }}</button>
            }
          </div>
        </div>

        <!-- Game Categories -->
        @if (loading()) {
          <div class="games-showcase">
            @for (i of [1,2,3,4]; track i) {
              <div class="game-skeleton">
                <div class="skeleton" style="height:160px;border-radius:20px"></div>
              </div>
            }
          </div>
        } @else {
          <div class="games-showcase">
            @for (game of gameTypes; track game.key; let i = $index) {
              <div class="game-section animate-fade-in-up" [style.animation-delay]="i * 0.15 + 's'">
                <div class="game-hero-card" [style.background]="game.gradient">
                  <div class="game-hero-content">
                    <span class="game-emoji">{{ game.icon }}</span>
                    <div>
                      <h2>{{ 'GAMES.' + game.key | translate }}</h2>
                      <p>{{ 'GAMES.' + game.key + '_DESC' | translate }}</p>
                      <div class="game-features">
                        @for (feat of getFeatures(game.key); track feat) {
                          <span class="game-feat">✓ {{ feat }}</span>
                        }
                      </div>
                    </div>
                  </div>
                  <button class="btn btn-play" (click)="playGame(game.key)" aria-label="العب الآن">
                    {{ 'GAMES.PLAY' | translate }} <i class="fas fa-play"></i>
                  </button>
                </div>
              </div>
            }
          </div>
        }
      </div>
    </section>
  `,
  styles: [`
    .game-hub { padding:2rem 0 5rem; }
    .grade-filter { margin-bottom:2.5rem; label { font-weight:700; margin-bottom:.75rem; display:block; color:#374151; } }
    .filter-chips { display:flex; flex-wrap:wrap; gap:.5rem; }
    .chip { padding:.45rem 1rem; border-radius:20px; border:2px solid #e5e7eb; background:white; font-weight:600; font-size:.85rem; cursor:pointer; transition:all .2s; color:#4b5563;
      &:hover { border-color:#6366f1; color:#6366f1; }
      &.active { background:linear-gradient(135deg,#6366f1,#a855f7); color:white; border-color:transparent; }
    }
    .games-showcase { display:flex; flex-direction:column; gap:1.5rem; }
    .game-hero-card { border-radius:20px; padding:2rem 2.5rem; display:flex; align-items:center; justify-content:space-between; color:white; transition:all .3s; position:relative; overflow:hidden;
      &::before { content:''; position:absolute; top:-50%; inset-inline-end:-20%; width:200px; height:200px; background:rgba(255,255,255,.08); border-radius:50%; }
      &:hover { transform:translateY(-4px); box-shadow:0 15px 40px rgba(0,0,0,.2); }
      @media(max-width:768px) { flex-direction:column; text-align:center; gap:1.5rem; padding:2rem 1.5rem; }
    }
    .game-hero-content { display:flex; align-items:center; gap:1.5rem; @media(max-width:768px){ flex-direction:column; } }
    .game-emoji { font-size:4rem; filter:drop-shadow(0 4px 8px rgba(0,0,0,.2)); }
    .game-hero-card h2 { font-size:1.5rem; font-weight:800; margin-bottom:.25rem; }
    .game-hero-card p { font-size:.95rem; opacity:.9; margin-bottom:.75rem; }
    .game-features { display:flex; flex-wrap:wrap; gap:.5rem .75rem; }
    .game-feat { font-size:.8rem; opacity:.85; }
    .btn-play { background:rgba(255,255,255,.2); backdrop-filter:blur(10px); color:white; border:2px solid rgba(255,255,255,.3); padding:.75rem 2rem; border-radius:25px; font-weight:700; font-size:1rem; text-decoration:none; transition:all .3s; white-space:nowrap; cursor:pointer;
      i { margin-inline-start:.4rem; }
      &:hover { background:rgba(255,255,255,.3); transform:scale(1.05); }
    }
  `]
})
export class GameHubComponent implements OnInit {
  grades = signal<Grade[]>([]);
  selectedGrade = signal<number>(1);
  loading = signal(true);
  matchingGames = signal<any[]>([]);
  dragDropGames = signal<any[]>([]);
  flipCardGames = signal<any[]>([]);

  gameTypes = [
    { key: 'MATCHING', icon: '🔗', gradient: 'linear-gradient(135deg,#6366f1,#818cf8)' },
    { key: 'WHEEL', icon: '🎡', gradient: 'linear-gradient(135deg,#a855f7,#c084fc)' },
    { key: 'DRAGDROP', icon: '✋', gradient: 'linear-gradient(135deg,#06b6d4,#22d3ee)' },
    { key: 'FLIPCARD', icon: '🃏', gradient: 'linear-gradient(135deg,#10b981,#34d399)' },
  ];

  constructor(private api: ApiService, private seo: SeoService, private translate: TranslateService, private router: Router) {}

  ngOnInit() {
    this.seo.setPage({
      titleDefault: 'الألعاب التعليمية',
      descriptionDefault: 'العب ألعاب تعليمية تفاعلية لتعلم الإنجليزية — التوصيل، عجلة المعرفة، السحب والإفلات، والبطاقات المقلوبة.',
      keywords: 'ألعاب إنجليزي, ألعاب تعليمية, لعبة التوصيل, لعبة العجلة, سحب وإفلات, بطاقات, تعلم الإنجليزية'
    });

    this.api.getGrades().subscribe(res => {
      if (res.success) {
        this.grades.set(res.data);
        if (res.data.length > 0) this.selectedGrade.set(res.data[0].id);
      }
      this.loading.set(false);
    });

    this.loadGamesForGrade();
  }

  selectGrade(gradeId: number) {
    this.selectedGrade.set(gradeId);
    this.loadGamesForGrade();
  }

  private loadGamesForGrade() {
    const filters = { gradeId: this.selectedGrade() };
    this.api.getMatchingGames(filters).subscribe(r => { if (r.success) this.matchingGames.set(r.data); });
    this.api.getDragDropGames(filters).subscribe(r => { if (r.success) this.dragDropGames.set(r.data); });
    this.api.getFlipCardGames(filters).subscribe(r => { if (r.success) this.flipCardGames.set(r.data); });
  }

  playGame(key: string) {
    switch (key) {
      case 'MATCHING':
        if (this.matchingGames().length > 0) {
          this.router.navigate(['/games/matching', this.matchingGames()[0].id]);
        } else {
          this.router.navigate(['/games/matching', 1]); // fallback
        }
        break;
      case 'WHEEL':
        this.router.navigate(['/games/wheel']);
        break;
      case 'DRAGDROP':
        if (this.dragDropGames().length > 0) {
          this.router.navigate(['/games/dragdrop', this.dragDropGames()[0].id]);
        } else {
          this.router.navigate(['/games/dragdrop', 1]); // fallback
        }
        break;
      case 'FLIPCARD':
        if (this.flipCardGames().length > 0) {
          this.router.navigate(['/games/flipcard', this.flipCardGames()[0].id]);
        } else {
          this.router.navigate(['/games/flipcard', 1]); // fallback
        }
        break;
    }
  }

  getFeatures(key: string): string[] {
    const featuresStr = this.translate.instant('GAMES.' + key + '_FEATURES');
    return featuresStr ? featuresStr.split(',') : [];
  }
}

