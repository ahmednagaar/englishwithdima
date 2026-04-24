import { Component, OnInit, signal, computed } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { TranslateModule } from '@ngx-translate/core';
import { ApiService } from '../../../core/services/api.service';

@Component({
  selector: 'app-matching',
  standalone: true,
  imports: [CommonModule, RouterLink, TranslateModule],
  template: `
    <section class="game-page">
      <div class="container">
        @if (loading()) {
          <div class="loading-spinner"></div>
        } @else if (gameOver()) {
          <div class="result-card animate-scale-in">
            <span class="result-emoji">{{ score() >= 80 ? '🏆' : score() >= 50 ? '⭐' : '💪' }}</span>
            <h2>{{ 'GAMES.GAME_OVER' | translate }}</h2>
            <div class="final-score">{{ score() }}</div>
            <p>{{ 'GAMES.MATCHED_PAIRS' | translate:{ matched: matchedPairs(), total: totalPairs() } }}</p>
            <div class="result-btns">
              <button class="btn btn-primary" (click)="restartGame()">{{ 'GAMES.PLAY_AGAIN' | translate }}</button>
              <a routerLink="/games" class="btn btn-secondary">{{ 'COMMON.BACK' | translate }}</a>
            </div>
          </div>
        } @else {
          <div class="game-header">
            <h1>🔗 {{ 'GAMES.MATCHING' | translate }}</h1>
            <div class="game-info">
              <span class="game-score">{{ 'GAMES.SCORE' | translate }}: {{ score() }}</span>
              <span class="game-time">⏱️ {{ formatTime(elapsed()) }}</span>
            </div>
          </div>

          <div class="matching-board">
            <div class="column left-col">
              <h3>{{ 'GAMES.WORDS' | translate }}</h3>
              @for (item of leftItems(); track item.id) {
                <div class="match-item" [class.selected]="selectedLeft() === item.id" [class.matched]="item.matched" (click)="selectLeft(item)">
                  {{ item.text }}
                </div>
              }
            </div>
            <div class="column right-col">
              <h3>{{ 'GAMES.MEANINGS' | translate }}</h3>
              @for (item of rightItems(); track item.id) {
                <div class="match-item right" [class.selected]="selectedRight() === item.id" [class.matched]="item.matched" [class.wrong]="item.wrong" (click)="selectRight(item)">
                  {{ item.text }}
                </div>
              }
            </div>
          </div>
        }
      </div>
    </section>
  `,
  styles: [`
    .game-page { padding:2rem 0 5rem; }
    .game-header { display:flex; justify-content:space-between; align-items:center; margin-bottom:2rem; h1 { font-size:1.5rem; font-weight:800; } }
    .game-info { display:flex; gap:1.5rem; }
    .game-score { font-weight:800; color:#6366f1; font-size:1.1rem; }
    .game-time { font-weight:600; color:#6b7280; }
    .matching-board { display:grid; grid-template-columns:1fr 1fr; gap:2rem; max-width:800px; margin:0 auto; @media(max-width:600px){ gap:1rem; } }
    .column { h3 { font-size:.9rem; font-weight:700; color:#6b7280; text-align:center; margin-bottom:1rem; text-transform:uppercase; letter-spacing:1px; } }
    .match-item { background:white; border:2px solid #e5e7eb; border-radius:14px; padding:1rem 1.25rem; text-align:center; font-weight:600; font-size:1rem; cursor:pointer; transition:all .3s; color:#1f2937; user-select:none;
      & + & { margin-top:.6rem; }
      &:hover:not(.matched) { border-color:#a5b4fc; background:#f5f3ff; transform:scale(1.02); }
      &.selected { border-color:#6366f1; background:#eef2ff; box-shadow:0 0 0 4px rgba(99,102,241,.15); }
      &.matched { border-color:#10b981; background:#d1fae5; color:#059669; opacity:.7; pointer-events:none; }
      &.wrong { border-color:#ef4444; background:#fee2e2; animation:shake .4s; }
      &.right { border-color:#e5e7eb; &.selected { border-color:#a855f7; background:#faf5ff; box-shadow:0 0 0 4px rgba(168,85,247,.15); } }
    }
    @keyframes shake { 0%,100%{transform:translateX(0)} 25%{transform:translateX(-6px)} 75%{transform:translateX(6px)} }
    .result-card { max-width:400px; margin:4rem auto; text-align:center; background:white; border-radius:24px; padding:3rem; box-shadow:0 10px 40px rgba(0,0,0,.08);
      .result-emoji { font-size:5rem; display:block; margin-bottom:1rem; animation:float 2s ease-in-out infinite; }
      h2 { font-size:1.5rem; font-weight:800; color:#1f2937; margin-bottom:1rem; }
      .final-score { font-size:3rem; font-weight:900; color:#6366f1; }
      p { color:#6b7280; margin-bottom:1.5rem; }
      .result-btns { display:flex; gap:1rem; justify-content:center; }
    }
  `]
})
export class MatchingComponent implements OnInit {
  loading = signal(true);
  gameOver = signal(false);
  score = signal(0);
  elapsed = signal(0);
  selectedLeft = signal<number | null>(null);
  selectedRight = signal<number | null>(null);
  matchedPairs = signal(0);
  totalPairs = signal(0);
  leftItems = signal<any[]>([]);
  rightItems = signal<any[]>([]);
  private timer: any;
  private sessionId = 0;

  constructor(private api: ApiService, private route: ActivatedRoute, private router: Router) {}

  ngOnInit() {
    const gameId = +this.route.snapshot.params['id'];
    this.api.startMatchingGame(gameId).subscribe({
      next: res => {
        this.loading.set(false);
        if (res.success) {
          this.sessionId = res.data.sessionId;

          // Backend returns leftItems & rightItems (pre-shuffled with pairId)
          if (res.data.leftItems?.length) {
            const left = res.data.leftItems;
            const right = res.data.rightItems;
            this.totalPairs.set(left.length);
            this.leftItems.set(left.map((item: any, i: number) => ({ id: i, text: item.text, imageUrl: item.imageUrl, pairId: item.pairId, matched: false })));
            this.rightItems.set(right.map((item: any, i: number) => ({ id: i, text: item.text, imageUrl: item.imageUrl, pairId: item.pairId, matched: false, wrong: false })));
          }
          // Fallback: pairs array (legacy or demo)
          else if (res.data.pairs?.length) {
            const pairs = res.data.pairs;
            this.totalPairs.set(pairs.length);
            this.leftItems.set(pairs.map((p: any, i: number) => ({ id: i, text: p.questionText, pairId: i, matched: false })));
            const shuffled = [...pairs].sort(() => Math.random() - 0.5);
            this.rightItems.set(shuffled.map((p: any, i: number) => ({ id: i, text: p.answerText, pairId: pairs.indexOf(p), matched: false, wrong: false })));
          }

          this.startTimer();
        }
      },
      error: () => this.loading.set(false)
    });
  }

  selectLeft(item: any) { if (!item.matched) this.selectedLeft.set(item.id); this.checkMatch(); }
  selectRight(item: any) { if (!item.matched) { item.wrong = false; this.selectedRight.set(item.id); } this.checkMatch(); }

  private checkMatch() {
    const l = this.selectedLeft(), r = this.selectedRight();
    if (l === null || r === null) return;
    const left = this.leftItems().find(i => i.id === l)!;
    const right = this.rightItems().find(i => i.id === r)!;

    if (left.pairId === right.pairId) {
      left.matched = true; right.matched = true;
      this.score.set(this.score() + 100);
      this.matchedPairs.set(this.matchedPairs() + 1);
      if (this.matchedPairs() === this.totalPairs()) this.endGame();
    } else {
      right.wrong = true;
      setTimeout(() => { right.wrong = false; }, 600);
    }
    this.selectedLeft.set(null);
    this.selectedRight.set(null);
  }

  private endGame() { clearInterval(this.timer); this.gameOver.set(true); }
  restartGame() { location.reload(); }
  private startTimer() { this.timer = setInterval(() => this.elapsed.set(this.elapsed() + 1), 1000); }
  formatTime(s: number): string { return `${Math.floor(s / 60)}:${(s % 60).toString().padStart(2, '0')}`; }
}
